-- Catálogo de especialidades de técnicos (global o por empresa)
CREATE TABLE IF NOT EXISTS especialidades (
    id          uuid         DEFAULT gen_random_uuid() PRIMARY KEY,
    empresa_id  uuid         NULL REFERENCES empresas(id),
    nombre      varchar(200) NOT NULL,
    descripcion varchar(500),
    activo      boolean      NOT NULL DEFAULT true,
    created_at  timestamptz  NOT NULL DEFAULT now(),
    updated_at  timestamptz  NOT NULL DEFAULT now(),
    created_by  uuid,
    updated_by  uuid,
    deleted_at  timestamptz,
    deleted_by  uuid
);

-- Únicos parciales: uno para globales, otro para las propias de una empresa
CREATE UNIQUE INDEX IF NOT EXISTS uq_especialidades_global_nombre
    ON especialidades (nombre)
    WHERE empresa_id IS NULL AND deleted_at IS NULL;

CREATE UNIQUE INDEX IF NOT EXISTS uq_especialidades_empresa_nombre
    ON especialidades (empresa_id, nombre)
    WHERE empresa_id IS NOT NULL AND deleted_at IS NULL;

CREATE INDEX IF NOT EXISTS idx_especialidades_activas
    ON especialidades (nombre) WHERE activo = true AND deleted_at IS NULL;
