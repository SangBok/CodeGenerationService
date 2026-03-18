import type { BtDefinition, NodeCatalog } from './types'

export async function loadCatalog(): Promise<NodeCatalog> {
  const res = await fetch('/schemas/node-catalog.poc.json', { cache: 'no-store' })
  if (!res.ok) throw new Error(`Failed to load node catalog: ${res.status}`)
  return (await res.json()) as NodeCatalog
}

export async function loadSchema(): Promise<unknown> {
  const res = await fetch('/schemas/behavior-tree.schema.json', { cache: 'no-store' })
  if (!res.ok) throw new Error(`Failed to load schema: ${res.status}`)
  return (await res.json()) as unknown
}

export async function loadExample(): Promise<BtDefinition> {
  const res = await fetch('/examples/move-safe-pos.tree.json', { cache: 'no-store' })
  if (!res.ok) throw new Error(`Failed to load example: ${res.status}`)
  return (await res.json()) as BtDefinition
}

