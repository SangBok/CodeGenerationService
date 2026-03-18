import type { NodeProps } from 'reactflow'
import { Handle, Position } from 'reactflow'

type Data = {
  btType: string
  name?: string
}

export function BtNode(props: NodeProps<Data>) {
  const { data, selected, id } = props
  return (
    <div
      style={{
        padding: 10,
        borderRadius: 8,
        border: selected ? '2px solid #3b82f6' : '1px solid #334155',
        background: '#0b1220',
        color: '#e2e8f0',
        minWidth: 180,
      }}
    >
      <div style={{ fontSize: 12, opacity: 0.9 }}>{data.btType}</div>
      <div style={{ fontSize: 14, fontWeight: 600 }}>{data.name || id}</div>
      <Handle type="target" position={Position.Top} />
      <Handle type="source" position={Position.Bottom} />
    </div>
  )
}

