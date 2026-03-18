export type BtValue =
  | boolean
  | number
  | string
  | { $ref: string }

export type BtNode = {
  id: string
  type: string
  name?: string
  children?: string[]
  parameters?: Record<string, BtValue>
  guard?: string | null
}

export type BlackboardKey = {
  key: string
  valueType: 'bool' | 'int' | 'double' | 'string' | 'enum' | 'ref'
  description?: string
  defaultValue?: BtValue
}

export type BtDefinition = {
  schemaVersion: string
  treeId: string
  version: number
  name?: string
  description?: string
  rootNodeId: string
  nodes: BtNode[]
  blackboardSchema?: BlackboardKey[]
  bindings?: unknown
  metadata?: unknown
}

export type CatalogNodeDef = {
  type: string
  category: string
  description?: string
  ports?: { minChildren?: number; exactChildren?: number }
  parameters?: Record<
    string,
    {
      valueType: string
      required?: boolean
      min?: number
      enumValues?: string[]
    }
  >
}

export type NodeCatalog = {
  catalogVersion: string
  nodes: CatalogNodeDef[]
}

