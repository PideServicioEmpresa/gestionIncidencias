-- Relación M-N entre usuarios y especialidades. Sin jerarquía: no hay "principal".
CREATE TABLE IF NOT EXISTS usuario_especialidades (
    id              uuid        DEFAULT gen_random_uuid() PRIMARY KEY,
    usuario_id      uuid        NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    especialidad_id uuid        NOT NULL REFERENCES especialidades(id),
    activo          boolean     NOT NULL DEFAULT true,
    created_at      timestamptz NOT NULL DEFAULT now(),
    created_by      uuid,
    updated_at      timestamptz,
    updated_by      uuid,
    CONSTRAINT uq_usuario_especialidades UNIQUE (usuario_id, especialidad_id)
);

CREATE INDEX IF NOT EXISTS idx_usuario_especialidades_usuario
    ON usuario_especialidades (usuario_id);
CREATE INDEX IF NOT EXISTS idx_usuario_especialidades_especialidad
    ON usuario_especialidades (especialidad_id);
