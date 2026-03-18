import Ajv from 'ajv'

export type ValidationResult =
  | { ok: true }
  | { ok: false; errors: string[] }

export function validateWithSchema(schema: unknown, data: unknown): ValidationResult {
  const ajv = new Ajv({ allErrors: true, strict: false })
  const validate = ajv.compile(schema as any)
  const ok = validate(data)
  if (ok) return { ok: true }

  const errors =
    validate.errors?.map((e) => `${e.instancePath || '(root)'} ${e.message ?? ''}`.trim()) ??
    ['Unknown validation error']
  return { ok: false, errors }
}

