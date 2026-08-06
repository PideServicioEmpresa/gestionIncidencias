-- Correos guardados por usuario (preferencias personales para reutilizar en tickets)
CREATE TABLE IF NOT EXISTS correos_guardados (
    id          uuid        DEFAULT gen_random_uuid() PRIMARY KEY,
    usuario_id  uuid        NOT NULL REFERENCES usuarios(id) ON DELETE CASCADE,
    correo      text        NOT NULL,
    created_at  timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT  uq_correos_guardados UNIQUE (usuario_id, correo)
);

CREATE INDEX IF NOT EXISTS ix_correos_guardados_usuario
    ON correos_guardados(usuario_id);
