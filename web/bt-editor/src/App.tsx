import { useCallback, useEffect, useMemo, useState } from 'react'
import ReactFlow, {
  addEdge,
  Background,
  Controls,
  type Connection,
  type Edge,
  type Node,
  type OnNodesChange,
  type OnEdgesChange,
  applyEdgeChanges,
  applyNodeChanges,
} from 'reactflow'
import 'reactflow/dist/style.css'
import './App.css'
import type { BtDefinition, BtNode, BtValue, CatalogNodeDef, NodeCatalog } from './bt/types'
import { loadCatalog, loadExample, loadSchema } from './bt/loaders'
import { validateWithSchema, type ValidationResult } from './bt/validate'
import { BtNode as BtNodeView } from './components/BtNode'

type FlowNodeData = {
  btType: string
  name?: string
  parameters?: Record<string, BtValue>
}

function App() {
  const [catalog, setCatalog] = useState<NodeCatalog | null>(null)
  const [schema, setSchema] = useState<unknown | null>(null)
  const [treeMeta, setTreeMeta] = useState({ treeId: 'NewTree', version: 1, schemaVersion: '1.0.0' })

  const [nodes, setNodes] = useState<Node<FlowNodeData>[]>([])
  const [edges, setEdges] = useState<Edge[]>([])
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null)
  const [selectedEdgeIds, setSelectedEdgeIds] = useState<Set<string>>(() => new Set())
  const [exportJson, setExportJson] = useState<string>('')
  const [validation, setValidation] = useState<ValidationResult>({ ok: true })

  useEffect(() => {
    ;(async () => {
      const [c, s, ex] = await Promise.all([loadCatalog(), loadSchema(), loadExample()])
      setCatalog(c)
      setSchema(s)
      loadFromDefinition(ex)
      setTreeMeta({ treeId: ex.treeId, version: ex.version, schemaVersion: ex.schemaVersion })
    })().catch((e) => {
      console.error(e)
      alert(String(e))
    })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const nodeTypes = useMemo(() => ({ btNode: BtNodeView }), [])

  const onNodesChange: OnNodesChange = useCallback(
    (changes) => setNodes((nds) => applyNodeChanges(changes, nds)),
    []
  )
  const onEdgesChange: OnEdgesChange = useCallback(
    (changes) => setEdges((eds) => applyEdgeChanges(changes, eds)),
    []
  )
  const onConnect = useCallback((params: Connection) => setEdges((eds) => addEdge(params, eds)), [])

  const selectedNode = useMemo(() => nodes.find((n) => n.id === selectedNodeId) ?? null, [nodes, selectedNodeId])

  const palette = useMemo(() => catalog?.nodes ?? [], [catalog])

  const deleteSelection = useCallback(() => {
    setNodes((prevNodes) => {
      if (!selectedNodeId) return prevNodes
      return prevNodes.filter((n) => n.id !== selectedNodeId)
    })
    setEdges((prevEdges) => {
      const edgeIdSet = selectedEdgeIds
      return prevEdges.filter((e) => {
        if (edgeIdSet.has(e.id)) return false
        if (selectedNodeId && (e.source === selectedNodeId || e.target === selectedNodeId)) return false
        return true
      })
    })
    setSelectedNodeId(null)
    setSelectedEdgeIds(new Set())
  }, [selectedEdgeIds, selectedNodeId])

  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      const key = e.key
      if (key !== 'Delete' && key !== 'Backspace') return

      const target = e.target as HTMLElement | null
      const tag = target?.tagName?.toLowerCase()
      const isTypingTarget = tag === 'input' || tag === 'textarea' || (target?.getAttribute?.('contenteditable') === 'true')
      if (isTypingTarget) return

      if (!selectedNodeId && selectedEdgeIds.size === 0) return
      e.preventDefault()
      deleteSelection()
    }

    window.addEventListener('keydown', onKeyDown)
    return () => window.removeEventListener('keydown', onKeyDown)
  }, [deleteSelection, selectedEdgeIds.size, selectedNodeId])

  const addNodeFromPalette = useCallback((def: CatalogNodeDef) => {
    const id = `${def.type.replaceAll('.', '_')}_${Math.random().toString(16).slice(2, 8)}`
    setNodes((prev) => [
      ...prev,
      {
        id,
        type: 'btNode',
        position: { x: 80 + prev.length * 20, y: 80 + prev.length * 20 },
        data: { btType: def.type, name: def.type.split('.').at(-1) ?? def.type, parameters: {} },
      },
    ])
  }, [])

  const setSelectedNodeParameters = useCallback((jsonText: string) => {
    if (!selectedNodeId) return
    let parsed: unknown
    try {
      parsed = jsonText.trim() ? JSON.parse(jsonText) : {}
    } catch (e) {
      alert(`Invalid JSON: ${String(e)}`)
      return
    }
    if (typeof parsed !== 'object' || parsed === null || Array.isArray(parsed)) {
      alert('Parameters must be a JSON object.')
      return
    }
    setNodes((prev) =>
      prev.map((n) =>
        n.id === selectedNodeId ? { ...n, data: { ...n.data, parameters: parsed as Record<string, BtValue> } } : n
      )
    )
  }, [selectedNodeId])

  const exportDefinition = useCallback((): BtDefinition => {
    const incoming = new Map<string, number>()
    for (const n of nodes) incoming.set(n.id, 0)
    for (const e of edges) incoming.set(e.target, (incoming.get(e.target) ?? 0) + 1)

    const roots = nodes.filter((n) => (incoming.get(n.id) ?? 0) === 0)
    const rootNode = roots[0]
    if (!rootNode) throw new Error('No root node found. Connect nodes so there is exactly one root.')

    const byId = new Map(nodes.map((n) => [n.id, n]))
    const childrenBySource = new Map<string, string[]>()
    for (const e of edges) {
      const list = childrenBySource.get(e.source) ?? []
      list.push(e.target)
      childrenBySource.set(e.source, list)
    }

    // order children by Y position for stable sequencing
    for (const [src, list] of childrenBySource.entries()) {
      list.sort((a, b) => (byId.get(a)?.position.y ?? 0) - (byId.get(b)?.position.y ?? 0))
      childrenBySource.set(src, list)
    }

    const btNodes: BtNode[] = nodes.map((n) => ({
      id: n.id,
      type: n.data.btType,
      name: n.data.name,
      children: childrenBySource.get(n.id),
      parameters: Object.keys(n.data.parameters ?? {}).length ? n.data.parameters : undefined,
    }))

    return {
      schemaVersion: treeMeta.schemaVersion,
      treeId: treeMeta.treeId,
      version: treeMeta.version,
      rootNodeId: rootNode.id,
      nodes: btNodes,
    }
  }, [edges, nodes, treeMeta.schemaVersion, treeMeta.treeId, treeMeta.version])

  const doExport = useCallback(() => {
    const def = exportDefinition()
    if (schema) {
      const result = validateWithSchema(schema, def)
      setValidation(result)
      if (!result.ok) {
        setExportJson(JSON.stringify(def, null, 2))
        return
      }
    }
    setValidation({ ok: true })
    setExportJson(JSON.stringify(def, null, 2))
  }, [exportDefinition, schema])

  const loadFromDefinition = useCallback((def: BtDefinition) => {
    // very small layout: BFS from root
    const byId = new Map(def.nodes.map((n) => [n.id, n]))
    const depth = new Map<string, number>([[def.rootNodeId, 0]])
    const order: string[] = []
    const q: string[] = [def.rootNodeId]
    while (q.length) {
      const id = q.shift()!
      order.push(id)
      const d = depth.get(id) ?? 0
      const n = byId.get(id)
      for (const c of n?.children ?? []) {
        if (!depth.has(c)) depth.set(c, d + 1)
        q.push(c)
      }
    }

    const yAtDepth = new Map<number, number>()
    const rfNodes: Node<FlowNodeData>[] = def.nodes.map((n) => {
      const d = depth.get(n.id) ?? 0
      const y = yAtDepth.get(d) ?? 0
      yAtDepth.set(d, y + 1)
      return {
        id: n.id,
        type: 'btNode',
        position: { x: 80 + d * 260, y: 80 + y * 120 },
        data: { btType: n.type, name: n.name ?? n.id, parameters: n.parameters ?? {} },
      }
    })

    const rfEdges: Edge[] = []
    for (const n of def.nodes) {
      for (const c of n.children ?? []) {
        rfEdges.push({ id: `${n.id}->${c}`, source: n.id, target: c })
      }
    }

    setNodes(rfNodes)
    setEdges(rfEdges)
    setSelectedNodeId(def.rootNodeId)
    setSelectedEdgeIds(new Set())
    setExportJson('')
    setValidation({ ok: true })
  }, [])

  const doImport = useCallback(() => {
    const text = exportJson.trim()
    if (!text) return
    let obj: unknown
    try {
      obj = JSON.parse(text)
    } catch (e) {
      alert(`Invalid JSON: ${String(e)}`)
      return
    }
    if (schema) {
      const result = validateWithSchema(schema, obj)
      setValidation(result)
      if (!result.ok) return
    }
    loadFromDefinition(obj as BtDefinition)
  }, [exportJson, loadFromDefinition, schema])

  return (
    <div className="appRoot">
      <header className="topBar">
        <div className="topBarTitle">BT Editor (PoC)</div>
        <div className="topBarMeta">
          <label>
            TreeId
            <input value={treeMeta.treeId} onChange={(e) => setTreeMeta((m) => ({ ...m, treeId: e.target.value }))} />
          </label>
          <label>
            Version
            <input
              type="number"
              value={treeMeta.version}
              onChange={(e) => setTreeMeta((m) => ({ ...m, version: Number(e.target.value) }))}
            />
          </label>
          <button onClick={doExport}>Export</button>
        </div>
      </header>

      <div className="body">
        <aside className="panel left">
          <div className="panelTitle">Palette</div>
          {!catalog ? (
            <div className="muted">Loading...</div>
          ) : (
            <div className="paletteList">
              {palette.map((p) => (
                <button key={p.type} className="paletteItem" onClick={() => addNodeFromPalette(p)}>
                  <div className="paletteType">{p.type}</div>
                  <div className="paletteDesc">{p.description ?? p.category}</div>
                </button>
              ))}
            </div>
          )}
        </aside>

        <main className="canvas">
          <ReactFlow
            nodes={nodes}
            edges={edges}
            nodeTypes={nodeTypes}
            onNodesChange={onNodesChange}
            onEdgesChange={onEdgesChange}
            onConnect={onConnect}
            onNodeClick={(_, n) => {
              setSelectedNodeId(n.id)
              setSelectedEdgeIds(new Set())
            }}
            onEdgeClick={(_, e) => {
              setSelectedNodeId(null)
              setSelectedEdgeIds(new Set([e.id]))
            }}
            onPaneClick={() => {
              setSelectedNodeId(null)
              setSelectedEdgeIds(new Set())
            }}
            fitView
          >
            <Background />
            <Controls />
          </ReactFlow>
        </main>

        <aside className="panel right">
          <div className="panelTitle">Inspector</div>
          {!selectedNode ? (
            <div className="muted">Select a node.</div>
          ) : (
            <>
              <div className="field">
                <div className="label">Id</div>
                <div className="value">{selectedNode.id}</div>
              </div>
              <div className="field">
                <div className="label">Type</div>
                <div className="value">{selectedNode.data.btType}</div>
              </div>
              <div className="field">
                <div className="label">Name</div>
                <input
                  value={selectedNode.data.name ?? ''}
                  onChange={(e) =>
                    setNodes((prev) =>
                      prev.map((n) => (n.id === selectedNode.id ? { ...n, data: { ...n.data, name: e.target.value } } : n))
                    )
                  }
                />
              </div>
              <div className="field">
                <div className="label">Parameters (JSON)</div>
                <textarea
                  rows={10}
                  defaultValue={JSON.stringify(selectedNode.data.parameters ?? {}, null, 2)}
                  onBlur={(e) => setSelectedNodeParameters(e.target.value)}
                />
                <div className="muted">Use refs like: {`{"positionIdx": {"$ref":"positionIdx"}}`}</div>
              </div>
              <div className="row">
                <button onClick={deleteSelection}>Delete selected (Del)</button>
              </div>
            </>
          )}

          <div className="panelTitle" style={{ marginTop: 16 }}>
            Import / Export JSON
          </div>
          <textarea
            rows={14}
            value={exportJson}
            onChange={(e) => setExportJson(e.target.value)}
            placeholder="Click Export to generate JSON, or paste JSON then click Import."
          />
          <div className="row">
            <button onClick={doImport}>Import</button>
            {!validation.ok && <div className="error">{validation.errors.join('\n')}</div>}
            {validation.ok && exportJson && <div className="ok">Schema OK</div>}
          </div>
        </aside>
      </div>
    </div>
  )
}

export default App
