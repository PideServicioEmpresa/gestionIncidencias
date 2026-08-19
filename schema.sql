--
-- PostgreSQL database dump
--

\restrict UuL4QAQuGQrxctrbyg06bZEaY659j858zlsbD1jPlFCJObKampPemUYD4DP6kt6

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.10

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA public;


--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'Schema principal de Pide Servicio v1.0. PKs: UUID NOT NULL DEFAULT gen_random_uuid(). Naming: snake_case minúsculas, español, tablas en plural. Audit estándar (tablas mutables):   created_at TIMESTAMPTZ, updated_at TIMESTAMPTZ, deleted_at TIMESTAMPTZ,   created_by UUID→usuarios, updated_by UUID→usuarios, deleted_by UUID→usuarios,   version INTEGER, activo BOOLEAN. Borrado lógico: deleted_at IS NOT NULL (nunca DELETE físico). Timestamps: TIMESTAMPTZ en UTC. Conversión de timezone en aplicación. FKs: ON DELETE SET NULL en created_by/updated_by/deleted_by. ON DELETE CASCADE solo en relaciones de ownership (ej: tecnico_sucursales). Ref: docs/database/NAMING_CONVENTIONS.md';


--
-- Name: canal_notificacion_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.canal_notificacion_tipo AS ENUM (
    'IN_APP',
    'EMAIL',
    'PUSH'
);


--
-- Name: estado_entrega_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.estado_entrega_tipo AS ENUM (
    'PENDIENTE',
    'ENVIADO',
    'FALLIDO'
);


--
-- Name: estado_laboral_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.estado_laboral_tipo AS ENUM (
    'ACTIVO',
    'VACACIONES',
    'LICENCIA',
    'SUSPENDIDO',
    'RETIRADO'
);


--
-- Name: evidencia_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.evidencia_tipo AS ENUM (
    'INICIAL',
    'FINAL'
);


--
-- Name: prioridad_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.prioridad_tipo AS ENUM (
    'CRITICA',
    'ALTA',
    'MEDIA',
    'BAJA'
);


--
-- Name: rol_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.rol_tipo AS ENUM (
    'SUPERADMIN',
    'ADMIN',
    'SUPERVISOR',
    'TECNICO',
    'TRABAJADOR',
    'USUARIO'
);


--
-- Name: ticket_estado_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.ticket_estado_tipo AS ENUM (
    'NUEVO',
    'SIN_ASIGNAR',
    'ASIGNADO',
    'EN_PROCESO',
    'EN_ESPERA',
    'PENDIENTE_VALIDACION',
    'REABIERTO',
    'CERRADO',
    'CANCELADO'
);


--
-- Name: tipo_dato_parametro_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.tipo_dato_parametro_tipo AS ENUM (
    'TEXTO',
    'ENTERO',
    'BOOLEANO',
    'JSON'
);


--
-- Name: tipo_evento_historial_tipo; Type: TYPE; Schema: public; Owner: -
--

CREATE TYPE public.tipo_evento_historial_tipo AS ENUM (
    'CREADO',
    'ESTADO_CAMBIADO',
    'ASIGNADO',
    'REASIGNADO',
    'COMENTADO',
    'EVIDENCIA_SUBIDA',
    'PRIORIDAD_CAMBIADA',
    'AREA_CAMBIADA',
    'SLA_MODIFICADO_MANUAL',
    'DATOS_ACTUALIZADOS',
    'SUCURSAL_CAMBIADA'
);


--
-- Name: calcular_fecha_limite(uuid, timestamp with time zone, integer, smallint, smallint, smallint[]); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.calcular_fecha_limite(p_empresa_id uuid, p_inicio timestamp with time zone, p_horas_laborales integer, p_hora_inicio_lab smallint DEFAULT 8, p_hora_fin_lab smallint DEFAULT 18, p_dias_laborales smallint[] DEFAULT '{1,2,3,4,5}'::smallint[]) RETURNS timestamp with time zone
    LANGUAGE plpgsql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
DECLARE
    v_actual        TIMESTAMPTZ := p_inicio;
    v_horas_rest    INTEGER     := p_horas_laborales;
    v_dow           SMALLINT;
    v_horas_dia     INTEGER;
    v_inicio_lab    TIMESTAMPTZ;
    v_fin_lab       TIMESTAMPTZ;
    v_max_iter      INTEGER     := 365;
BEGIN
    WHILE v_horas_rest > 0 AND v_max_iter > 0 LOOP
        v_max_iter := v_max_iter - 1;

        -- ISO dow: 1=lunes ... 7=domingo
        v_dow := EXTRACT(ISODOW FROM v_actual)::SMALLINT;

        -- Verificar día laboral
        IF NOT (v_dow = ANY(p_dias_laborales)) THEN
            v_actual := DATE_TRUNC('day', v_actual) + INTERVAL '1 day'
                      + (p_hora_inicio_lab || ' hours')::INTERVAL;
            CONTINUE;
        END IF;

        -- Verificar si es feriado
        IF EXISTS (
            SELECT 1 FROM feriados
            WHERE fecha = DATE(v_actual AT TIME ZONE 'UTC')
              AND deleted_at IS NULL
              AND (empresa_id = p_empresa_id OR empresa_id IS NULL)
        ) THEN
            v_actual := DATE_TRUNC('day', v_actual) + INTERVAL '1 day'
                      + (p_hora_inicio_lab || ' hours')::INTERVAL;
            CONTINUE;
        END IF;

        -- Calcular ventana laboral del día
        v_inicio_lab := DATE_TRUNC('day', v_actual) + (p_hora_inicio_lab || ' hours')::INTERVAL;
        v_fin_lab    := DATE_TRUNC('day', v_actual) + (p_hora_fin_lab    || ' hours')::INTERVAL;

        -- Si estamos antes del inicio de jornada, avanzar al inicio
        IF v_actual < v_inicio_lab THEN
            v_actual := v_inicio_lab;
        END IF;

        -- Si estamos al final o después de la jornada, saltar al día siguiente
        IF v_actual >= v_fin_lab THEN
            v_actual := DATE_TRUNC('day', v_actual) + INTERVAL '1 day'
                      + (p_hora_inicio_lab || ' hours')::INTERVAL;
            CONTINUE;
        END IF;

        -- Horas disponibles en este día
        v_horas_dia := EXTRACT(EPOCH FROM (v_fin_lab - v_actual))::INTEGER / 3600;

        IF v_horas_rest <= v_horas_dia THEN
            RETURN v_actual + (v_horas_rest || ' hours')::INTERVAL;
        ELSE
            v_horas_rest := v_horas_rest - v_horas_dia;
            v_actual := DATE_TRUNC('day', v_actual) + INTERVAL '1 day'
                      + (p_hora_inicio_lab || ' hours')::INTERVAL;
        END IF;
    END LOOP;

    -- Fallback: devolver tiempo UTC simple (no se debería llegar aquí)
    RETURN p_inicio + (p_horas_laborales || ' hours')::INTERVAL;
END;
$$;


--
-- Name: create_audit_log(uuid, character varying, character varying, character varying, character varying, character varying, uuid, character varying, jsonb, jsonb, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.create_audit_log(p_actor_id uuid, p_actor_nombre character varying, p_actor_rol character varying, p_accion character varying, p_modulo character varying, p_entidad_tipo character varying, p_entidad_id uuid, p_entidad_codigo character varying DEFAULT NULL::character varying, p_valor_anterior jsonb DEFAULT NULL::jsonb, p_valor_nuevo jsonb DEFAULT NULL::jsonb, p_sucursal_id uuid DEFAULT NULL::uuid) RETURNS void
    LANGUAGE plpgsql SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
BEGIN
    INSERT INTO audit_logs (
        actor_id,       actor_nombre,   actor_rol,
        accion,         modulo,
        entidad_tipo,   entidad_id,     entidad_codigo,
        valor_anterior, valor_nuevo,
        sucursal_id
    ) VALUES (
        p_actor_id,       p_actor_nombre,   p_actor_rol,
        p_accion,         p_modulo,
        p_entidad_tipo,   p_entidad_id,     p_entidad_codigo,
        p_valor_anterior, p_valor_nuevo,
        p_sucursal_id
    );
END;
$$;


--
-- Name: FUNCTION create_audit_log(p_actor_id uuid, p_actor_nombre character varying, p_actor_rol character varying, p_accion character varying, p_modulo character varying, p_entidad_tipo character varying, p_entidad_id uuid, p_entidad_codigo character varying, p_valor_anterior jsonb, p_valor_nuevo jsonb, p_sucursal_id uuid); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.create_audit_log(p_actor_id uuid, p_actor_nombre character varying, p_actor_rol character varying, p_accion character varying, p_modulo character varying, p_entidad_tipo character varying, p_entidad_id uuid, p_entidad_codigo character varying, p_valor_anterior jsonb, p_valor_nuevo jsonb, p_sucursal_id uuid) IS 'Inserta una entrada en audit_logs. SECURITY DEFINER: se ejecuta con privilegios del propietario (postgres, BYPASSRLS) para poder escribir en la tabla bajo FORCE ROW LEVEL SECURITY. Invocar desde triggers y Edge Functions que necesiten registrar acciones del sistema.';


--
-- Name: custom_access_token_hook(jsonb); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.custom_access_token_hook(event jsonb) RETURNS jsonb
    LANGUAGE plpgsql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
DECLARE
    claims     jsonb;
    user_rec   record;
BEGIN
    claims := event -> 'claims';

    BEGIN
        SELECT
            u.id,
            u.auth_id,
            u.nombre || ' ' || u.apellido AS nombre_completo,
            lower(u.rol::text)            AS rol,
            u.empresa_id
        INTO user_rec
        FROM public.usuarios u
        WHERE u.auth_id = (event ->> 'user_id')::uuid
          AND u.deleted_at IS NULL;

        IF FOUND THEN
            claims := jsonb_set(claims, '{auth_id}',
                to_jsonb(user_rec.auth_id::text));
            claims := jsonb_set(claims, '{user_id}',
                to_jsonb(user_rec.id::text));
            claims := jsonb_set(claims, '{rol}',
                to_jsonb(user_rec.rol));
            claims := jsonb_set(claims, '{nombre_completo}',
                to_jsonb(user_rec.nombre_completo));
            claims := jsonb_set(claims, '{empresa_id}',
                to_jsonb(COALESCE(
                    user_rec.empresa_id,
                    '00000000-0000-0000-0000-000000000000'
                )::text));
        END IF;

    EXCEPTION WHEN OTHERS THEN
        RETURN event;
    END;

    RETURN jsonb_set(event, '{claims}', claims);
END;
$$;


--
-- Name: fn_usuario_sucursales_guard_principal(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.fn_usuario_sucursales_guard_principal() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$
BEGIN
  IF NEW.es_principal = true THEN
    UPDATE usuario_sucursales
    SET    es_principal = false,
           updated_at   = now()
    WHERE  usuario_id   = NEW.usuario_id
      AND  id          <> NEW.id
      AND  es_principal = true;
  END IF;
  RETURN NEW;
END;
$$;


--
-- Name: get_current_user_empresa_id(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_current_user_empresa_id() RETURNS uuid
    LANGUAGE sql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
    SELECT empresa_id FROM usuarios WHERE auth_id = auth.uid()
$$;


--
-- Name: get_current_user_id(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_current_user_id() RETURNS uuid
    LANGUAGE sql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
    SELECT id FROM usuarios WHERE auth_id = auth.uid()
$$;


--
-- Name: get_current_user_rol(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_current_user_rol() RETURNS text
    LANGUAGE sql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
    SELECT rol::TEXT FROM usuarios WHERE auth_id = auth.uid()
$$;


--
-- Name: get_current_user_sucursales(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.get_current_user_sucursales() RETURNS uuid[]
    LANGUAGE sql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
    SELECT COALESCE(ARRAY_AGG(sucursal_id), ARRAY[]::UUID[])
    FROM tecnico_sucursales
    WHERE tecnico_id = get_current_user_id()
      AND activa = true
$$;


--
-- Name: is_admin_or_above(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.is_admin_or_above() RETURNS boolean
    LANGUAGE sql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
    SELECT get_current_user_rol() IN ('SUPERADMIN', 'ADMIN', 'SUPERVISOR')
$$;


--
-- Name: is_superadmin(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.is_superadmin() RETURNS boolean
    LANGUAGE sql STABLE SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
    SELECT get_current_user_rol() = 'SUPERADMIN'
$$;


--
-- Name: trg_fn_historial_check_rechazo(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_historial_check_rechazo() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
DECLARE
    v_es_otro BOOLEAN;
BEGIN
    -- Solo aplica cuando hay rejection_reason_id
    IF NEW.rejection_reason_id IS NOT NULL THEN
        SELECT es_otro INTO v_es_otro
        FROM motivos_rechazo
        WHERE id = NEW.rejection_reason_id;

        IF v_es_otro AND (NEW.rejection_comment IS NULL OR TRIM(NEW.rejection_comment) = '') THEN
            RAISE EXCEPTION
                'historial.rechazo_invalido: el motivo "Otro" requiere '
                'rejection_comment obligatorio (rejection_reason_id: %)',
                NEW.rejection_reason_id;
        END IF;
    END IF;

    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_historial_check_rechazo(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_historial_check_rechazo() IS 'Trigger BEFORE INSERT en ticket_historial. Valida la regla de negocio: cuando rejection_reason_id apunta a un motivo con es_otro = true, el campo rejection_comment es obligatorio. Implementa la decisión project_decisions_rechazo.md.';


--
-- Name: trg_fn_set_updated_at(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_set_updated_at() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
BEGIN
    NEW.updated_at := NOW();
    NEW.version    := OLD.version + 1;
    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_set_updated_at(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_set_updated_at() IS 'Trigger BEFORE UPDATE reutilizable. Actualiza updated_at = NOW() e incrementa version en cada UPDATE. Aplicar a todas las tablas mutables:   CREATE TRIGGER trg_{tabla}_before_update_set_updated_at   BEFORE UPDATE ON {tabla}   FOR EACH ROW EXECUTE FUNCTION trg_fn_set_updated_at(); Requiere columnas: updated_at TIMESTAMPTZ, version INTEGER. NO aplica a tablas append-only (ticket_historial, audit_logs, ticket_asignaciones). Ref: docs/database/NAMING_CONVENTIONS.md §8.';


--
-- Name: trg_fn_set_updated_at_only(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_set_updated_at_only() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
BEGIN
    NEW.updated_at := NOW();
    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_set_updated_at_only(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_set_updated_at_only() IS 'Trigger BEFORE UPDATE para catálogos sin columna version. Solo actualiza updated_at = NOW(). Para tablas con version INTEGER usar trg_fn_set_updated_at() (migración 001). Aplica a: roles, permisos, categorias, motivos_cancelacion, motivos_rechazo, parametros_sistema. Ref: docs/database/NAMING_CONVENTIONS.md §8.';


--
-- Name: trg_fn_sla_calc(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_sla_calc() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
DECLARE
    v_sla           sla_configuraciones%ROWTYPE;
    v_ahora         TIMESTAMPTZ := NOW();
BEGIN
    -- Buscar SLA específico (empresa × tipo_servicio × prioridad)
    SELECT * INTO v_sla
    FROM sla_configuraciones
    WHERE empresa_id        = NEW.empresa_id
      AND tipo_servicio_id  = NEW.tipo_servicio_id
      AND prioridad         = NEW.prioridad_efectiva
      AND activo            = TRUE
      AND deleted_at        IS NULL
    LIMIT 1;

    -- Si no hay SLA específico, usar SLA por defecto de la empresa
    IF v_sla.id IS NULL THEN
        SELECT * INTO v_sla
        FROM sla_configuraciones
        WHERE empresa_id        = NEW.empresa_id
          AND tipo_servicio_id  IS NULL
          AND prioridad         = NEW.prioridad_efectiva
          AND activo            = TRUE
          AND deleted_at        IS NULL
        LIMIT 1;
    END IF;

    IF v_sla.id IS NOT NULL THEN
        NEW.sla_id := v_sla.id;

        NEW.fecha_limite_primera_atencion := calcular_fecha_limite(
            NEW.empresa_id,
            v_ahora,
            v_sla.horas_primera_atencion,
            v_sla.horas_laborales_inicio,
            v_sla.horas_laborales_fin,
            v_sla.dias_laborales
        );

        NEW.fecha_limite_resolucion := calcular_fecha_limite(
            NEW.empresa_id,
            v_ahora,
            v_sla.horas_resolucion,
            v_sla.horas_laborales_inicio,
            v_sla.horas_laborales_fin,
            v_sla.dias_laborales
        );
    END IF;

    RETURN NEW;
END;
$$;


--
-- Name: trg_fn_tecnico_sucursales_guard_principal(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_tecnico_sucursales_guard_principal() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
BEGIN
    -- Solo actúa cuando la fila que se inserta/actualiza es es_principal = true.
    -- Para INSERT: NEW.id no está en la tabla aún, el WHERE id != NEW.id
    --   descarta correctamente todas las filas existentes del técnico.
    -- Para UPDATE: id != NEW.id excluye la fila en edición.
    IF NEW.es_principal = true THEN
        UPDATE tecnico_sucursales
        SET    es_principal = false
        WHERE  tecnico_id   = NEW.tecnico_id
          AND  id           != NEW.id
          AND  es_principal = true;
    END IF;
    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_tecnico_sucursales_guard_principal(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_tecnico_sucursales_guard_principal() IS 'Trigger BEFORE INSERT OR UPDATE en tabla tecnico_sucursales. Garantiza que solo exista un registro con es_principal = true por tecnico_id en cualquier momento. Cuando se marca una sucursal como principal, desactiva automáticamente es_principal en todas las demás filas del mismo Técnico. Sin recursión: el UPDATE interno establece es_principal = false, lo que no dispara la rama activa del trigger. Ref: docs/database/MODEL-PHYSICAL.md §7.23 / Decisión ARQ-009.';


--
-- Name: trg_fn_tickets_after_insert(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_tickets_after_insert() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    SET search_path TO 'public'
    AS $$
BEGIN
    INSERT INTO ticket_historial (
        ticket_id,
        actor_id,
        tipo_evento,
        estado_nuevo,
        created_by
    ) VALUES (
        NEW.id,
        NEW.created_by,
        'CREADO',
        NEW.estado,
        NEW.created_by
    );

    RETURN NULL;
END;
$$;


--
-- Name: FUNCTION trg_fn_tickets_after_insert(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_tickets_after_insert() IS 'Trigger AFTER INSERT en tickets. Inserta el evento inicial CREADO en ticket_historial para que la línea de tiempo del ticket siempre tenga su punto de origen. SECURITY DEFINER: corre como postgres (BYPASSRLS).';


--
-- Name: trg_fn_tickets_guard_inmutable(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_tickets_guard_inmutable() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
BEGIN
    IF NEW.codigo IS DISTINCT FROM OLD.codigo THEN
        RAISE EXCEPTION
            'ticket.guard_inmutable: el campo codigo es inmutable (valor: %)',
            OLD.codigo;
    END IF;
    IF NEW.tipo_servicio_id IS DISTINCT FROM OLD.tipo_servicio_id THEN
        RAISE EXCEPTION
            'ticket.guard_inmutable: el tipo_servicio_id no puede modificarse';
    END IF;
    IF NEW.solicitante_id IS DISTINCT FROM OLD.solicitante_id THEN
        RAISE EXCEPTION
            'ticket.guard_inmutable: el solicitante_id no puede modificarse';
    END IF;
    IF NEW.empresa_id IS DISTINCT FROM OLD.empresa_id THEN
        RAISE EXCEPTION
            'ticket.guard_inmutable: la empresa_id no puede modificarse';
    END IF;
    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_tickets_guard_inmutable(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_tickets_guard_inmutable() IS 'Trigger BEFORE UPDATE en tickets. Rechaza cualquier intento de modificar campos inmutables: codigo, tipo_servicio_id, solicitante_id, empresa_id. La protección de sla_id se agregará en M-011.';


--
-- Name: trg_fn_tickets_validate_transition(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_tickets_validate_transition() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
DECLARE
    v_rol TEXT;
BEGIN
    IF NEW.estado = OLD.estado THEN RETURN NEW; END IF;
    IF auth.uid() IS NULL THEN RETURN NEW; END IF;
    v_rol := get_current_user_rol();
    IF NOT validate_ticket_transition(OLD.estado, NEW.estado, v_rol) THEN
        RAISE EXCEPTION 'ticket.transicion_invalida: el rol % no puede transicionar de % a %', v_rol, OLD.estado, NEW.estado;
    END IF;
    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_tickets_validate_transition(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_tickets_validate_transition() IS 'Función de trigger BEFORE UPDATE OF estado en tickets. Obtiene el rol del usuario vía get_current_user_rol() y delega en validate_ticket_transition() para validar que la transición sea legal. Lanza EXCEPTION si la transición no está permitida para el rol activo.';


--
-- Name: trg_fn_usuarios_after_insert(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_usuarios_after_insert() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
BEGIN
    -- Inserta con todos los defaults definidos en la tabla.
    -- Solo se pasa usuario_id; los booleanos toman sus defaults.
    INSERT INTO preferencias_notificacion (usuario_id)
    VALUES (NEW.id)
    ON CONFLICT DO NOTHING;

    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_usuarios_after_insert(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_usuarios_after_insert() IS 'Trigger AFTER INSERT en tabla usuarios. Crea automáticamente una fila en preferencias_notificacion con todos los valores por defecto del modelo para el usuario recién creado. Garantiza la existencia siempre del registro de preferencias. ON CONFLICT DO NOTHING lo hace idempotente en re-inserciones. Ref: docs/database/MODEL-PHYSICAL.md §7.20.';


--
-- Name: trg_fn_usuarios_guard_auth_id(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trg_fn_usuarios_guard_auth_id() RETURNS trigger
    LANGUAGE plpgsql
    SET search_path TO 'public'
    AS $$
BEGIN
    -- IS DISTINCT FROM maneja NULLs correctamente, aunque auth_id
    -- sea NOT NULL por constraint. Se usa por robustez defensiva.
    IF NEW.auth_id IS DISTINCT FROM OLD.auth_id THEN
        RAISE EXCEPTION
            'auth_id es inmutable post-creación. '
            'El vínculo con Supabase Auth no puede modificarse. '
            'Para cambiar la identidad, elimine el usuario y cree uno nuevo. '
            'usuario_id=%, auth_id_actual=%, auth_id_intentado=%',
            OLD.id, OLD.auth_id, NEW.auth_id
            USING ERRCODE = 'integrity_constraint_violation';
    END IF;
    RETURN NEW;
END;
$$;


--
-- Name: FUNCTION trg_fn_usuarios_guard_auth_id(); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.trg_fn_usuarios_guard_auth_id() IS 'Trigger BEFORE UPDATE en tabla usuarios. Rechaza cualquier intento de modificar la columna auth_id post-creación. auth_id es el vínculo inmutable entre el perfil de aplicación y la identidad de Supabase Auth. Modificarlo rompería sesiones activas. Lanza EXCEPTION con ERRCODE integrity_constraint_violation. Ref: docs/database/MODEL-PHYSICAL.md §7.4.';


--
-- Name: validate_ticket_transition(public.ticket_estado_tipo, public.ticket_estado_tipo, text); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.validate_ticket_transition(p_estado_anterior public.ticket_estado_tipo, p_estado_nuevo public.ticket_estado_tipo, p_rol text) RETURNS boolean
    LANGUAGE plpgsql IMMUTABLE
    SET search_path TO 'public'
    AS $$
BEGIN
    -- Sin cambio de estado: siempre válido desde la perspectiva de transición
    IF p_estado_anterior = p_estado_nuevo THEN
        RETURN true;
    END IF;

    -- SUPERADMIN, ADMIN y SUPERVISOR: transiciones libres dentro del grafo válido
    IF p_rol IN ('SUPERADMIN', 'ADMIN', 'SUPERVISOR') THEN
        RETURN CASE
            WHEN p_estado_anterior = 'NUEVO'
                 AND p_estado_nuevo = 'SIN_ASIGNAR'              THEN true
            WHEN p_estado_anterior = 'SIN_ASIGNAR'
                 AND p_estado_nuevo = 'ASIGNADO'                 THEN true
            WHEN p_estado_anterior = 'ASIGNADO'
                 AND p_estado_nuevo = 'EN_PROCESO'               THEN true
            WHEN p_estado_anterior = 'EN_PROCESO'
                 AND p_estado_nuevo = 'EN_ESPERA'                THEN true
            WHEN p_estado_anterior = 'EN_PROCESO'
                 AND p_estado_nuevo = 'PENDIENTE_VALIDACION'     THEN true
            WHEN p_estado_anterior = 'EN_ESPERA'
                 AND p_estado_nuevo = 'EN_PROCESO'               THEN true
            WHEN p_estado_anterior = 'PENDIENTE_VALIDACION'
                 AND p_estado_nuevo = 'CERRADO'                  THEN true
            WHEN p_estado_anterior = 'PENDIENTE_VALIDACION'
                 AND p_estado_nuevo = 'REABIERTO'                THEN true
            WHEN p_estado_anterior = 'REABIERTO'
                 AND p_estado_nuevo = 'ASIGNADO'                 THEN true
            WHEN p_estado_anterior IN ('NUEVO','SIN_ASIGNAR','ASIGNADO','EN_PROCESO')
                 AND p_estado_nuevo = 'CANCELADO'                THEN true
            ELSE false
        END;
    END IF;

    -- TECNICO: solo puede transicionar tickets que le están asignados
    IF p_rol = 'TECNICO' THEN
        RETURN CASE
            WHEN p_estado_anterior = 'ASIGNADO'
                 AND p_estado_nuevo = 'EN_PROCESO'               THEN true
            WHEN p_estado_anterior = 'EN_PROCESO'
                 AND p_estado_nuevo = 'EN_ESPERA'                THEN true
            WHEN p_estado_anterior = 'EN_PROCESO'
                 AND p_estado_nuevo = 'PENDIENTE_VALIDACION'     THEN true
            WHEN p_estado_anterior = 'EN_ESPERA'
                 AND p_estado_nuevo = 'EN_PROCESO'               THEN true
            ELSE false
        END;
    END IF;

    -- TRABAJADOR y USUARIO (solicitante): validan o cancelan en estados iniciales
    IF p_rol IN ('TRABAJADOR', 'USUARIO') THEN
        RETURN CASE
            WHEN p_estado_anterior = 'PENDIENTE_VALIDACION'
                 AND p_estado_nuevo = 'CERRADO'                  THEN true
            WHEN p_estado_anterior = 'PENDIENTE_VALIDACION'
                 AND p_estado_nuevo = 'REABIERTO'                THEN true
            WHEN p_estado_anterior IN ('NUEVO','SIN_ASIGNAR')
                 AND p_estado_nuevo = 'CANCELADO'                THEN true
            ELSE false
        END;
    END IF;

    RETURN false;
END;
$$;


--
-- Name: FUNCTION validate_ticket_transition(p_estado_anterior public.ticket_estado_tipo, p_estado_nuevo public.ticket_estado_tipo, p_rol text); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.validate_ticket_transition(p_estado_anterior public.ticket_estado_tipo, p_estado_nuevo public.ticket_estado_tipo, p_rol text) IS 'Función pura (IMMUTABLE) que valida si la transición de estado es legal para el rol dado. Sin efectos secundarios ni consultas a BD. Implementa la matriz de transiciones del SDD §5. Llamada por trg_fn_tickets_validate_transition.';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: areas; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.areas (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    sucursal_id uuid NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion character varying(500),
    responsable_id uuid,
    activa boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid
);


--
-- Name: TABLE areas; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.areas IS 'Subdivisiones funcionales o departamentos dentro de una sucursal. Clasifican los tickets por departamento responsable y sirven de criterio de asignación y filtro en reportes. Cambiar el área de un ticket en SIN_ASIGNAR es libre; en otros estados requiere privilegios Admin/SuperAdmin con auditoría en ticket_historial. Ref: docs/database/MODEL-PHYSICAL.md §7.3.';


--
-- Name: COLUMN areas.sucursal_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.sucursal_id IS 'FK activa → sucursales.id ON DELETE RESTRICT (fk_areas_sucursales). No se puede eliminar una sucursal que tiene áreas asociadas.';


--
-- Name: COLUMN areas.nombre; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.nombre IS 'Nombre del área. Único por sucursal (validado por uq_areas_sucursal_nombre). El índice es parcial WHERE deleted_at IS NULL: permite reutilizar el nombre tras borrado lógico.';


--
-- Name: COLUMN areas.responsable_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.responsable_id IS 'Usuario responsable del área. Opcional. FK diferida → usuarios.id ON DELETE SET NULL (M-0031).';


--
-- Name: COLUMN areas.activa; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.activa IS 'Soft disable del área. false = área no disponible para nuevos tickets.';


--
-- Name: COLUMN areas.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.created_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: COLUMN areas.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.updated_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: COLUMN areas.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.areas.deleted_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: audit_logs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.audit_logs (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    actor_id uuid,
    actor_nombre character varying(200) NOT NULL,
    actor_rol character varying(30) NOT NULL,
    accion character varying(100) NOT NULL,
    modulo character varying(50) NOT NULL,
    entidad_tipo character varying(50) NOT NULL,
    entidad_id uuid NOT NULL,
    entidad_codigo character varying(50),
    valor_anterior jsonb,
    valor_nuevo jsonb,
    ip inet,
    user_agent text,
    sucursal_id uuid,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);

ALTER TABLE ONLY public.audit_logs FORCE ROW LEVEL SECURITY;


--
-- Name: TABLE audit_logs; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.audit_logs IS 'Registro inmutable de cada acción crítica del sistema. Solo INSERT vía service_role — nunca UPDATE ni DELETE. Implementa auditoría completa: quién, qué, cuándo, sobre qué entidad y desde qué contexto (IP, user-agent, sucursal). actor_nombre y actor_rol están desnormalizados al momento de la acción: el log conserva los valores históricos aunque el actor cambie posteriormente. entidad_id sin FK: puede referenciar cualquier tabla del sistema. Sin particionamiento en MVP (DUDA-ARQ-009 resuelta). Ref: docs/database/MODEL-PHYSICAL.md §7.21.';


--
-- Name: COLUMN audit_logs.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.id IS 'PK UUID. Generada automáticamente (gen_random_uuid()) en cada INSERT. Identifica unívocamente cada entrada de auditoría del sistema.';


--
-- Name: COLUMN audit_logs.actor_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.actor_id IS 'FK opcional → usuarios(id) ON DELETE SET NULL. Usuario que ejecutó la acción auditada. NULL en acciones ejecutadas por el sistema sin actor humano identificado (ej: cierre automático por SLA, cron de limpieza, trigger de BD). SET NULL: si el actor es eliminado físicamente, el log se conserva. La identidad histórica se preserva en actor_nombre y actor_rol (desnormalizados).';


--
-- Name: COLUMN audit_logs.actor_nombre; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.actor_nombre IS 'Nombre completo del actor DESNORMALIZADO al momento de la acción. Se captura del registro de usuario en el momento exacto del INSERT. Inmutable después del INSERT: si el usuario cambia su nombre, este campo conserva el nombre histórico correcto. Permite leer el log sin joins adicionales a la tabla usuarios.';


--
-- Name: COLUMN audit_logs.actor_rol; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.actor_rol IS 'Rol del actor DESNORMALIZADO al momento de la acción. Valores posibles: SUPERADMIN, ADMIN, SUPERVISOR, TECNICO, TRABAJADOR, USUARIO, SISTEMA. Se captura del JWT o del registro del usuario en el momento del INSERT. Inmutable después del INSERT: si el usuario cambia de rol posteriormente, este campo refleja el rol con el que ejecutó la acción auditada. VARCHAR(30) para acomodar valores futuros sin migración de ENUM.';


--
-- Name: COLUMN audit_logs.accion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.accion IS 'Identificador de la acción auditada en formato modulo.accion. Ejemplos: "ticket.cancelado", "ticket.asignado", "usuario.creado", "sla.modificado_manual", "area.cambiada". Coincide con los códigos del catálogo de permisos (permisos.codigo). Soportado por idx_audit_accion para filtros por tipo de acción en reportes.';


--
-- Name: COLUMN audit_logs.modulo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.modulo IS 'Módulo del sistema donde ocurrió la acción. Ejemplos: "tickets", "usuarios", "empresas", "sucursales", "sla". Soportado por el índice idx_audit_modulo_fecha (modulo, created_at DESC) para consultas del tipo "muéstrame todos los eventos del módulo tickets hoy".';


--
-- Name: COLUMN audit_logs.entidad_tipo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.entidad_tipo IS 'Tipo de entidad sobre la que se ejecutó la acción. Ejemplos: "ticket", "usuario", "empresa", "sucursal", "categoria". Primer componente de la referencia polimórfica entidad_tipo + entidad_id. Soportado por el índice idx_audit_entidad (entidad_tipo, entidad_id).';


--
-- Name: COLUMN audit_logs.entidad_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.entidad_id IS 'ID UUID de la entidad afectada. SIN FOREIGN KEY. No tiene FK porque puede referenciar cualquier tabla del sistema (tickets, usuarios, empresas, sucursales, áreas, categorías, etc.). Una FK polimórfica requeriría mecanismos complejos no justificados en MVP. El Backend resuelve la referencia usando entidad_tipo + entidad_id al construir el detalle del log para la UI de auditoría. Soportado por el índice idx_audit_entidad (entidad_tipo, entidad_id).';


--
-- Name: COLUMN audit_logs.entidad_codigo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.entidad_codigo IS 'Código legible de la entidad afectada al momento de la acción. NULL cuando la entidad no tiene código de negocio visible (ej: sucursales, categorías, áreas). Ejemplos: "PS-000001" para tickets. Desnormalizado: se captura al momento del INSERT para que el log sea autocontenido.';


--
-- Name: COLUMN audit_logs.valor_anterior; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.valor_anterior IS 'JSONB con el estado PREVIO de los campos afectados por la acción. NULL en acciones de creación (no hay estado previo). Contiene SOLO los campos que cambiaron, no el objeto completo. Ejemplo cambio de prioridad: {"prioridad_admin": "MEDIA"}. Ejemplo asignación: {"tecnico_id": null, "estado": "SIN_ASIGNAR"}. Par con valor_nuevo para reconstruir el historial de cambios de cualquier entidad.';


--
-- Name: COLUMN audit_logs.valor_nuevo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.valor_nuevo IS 'JSONB con el estado POSTERIOR de los campos afectados por la acción. NULL en acciones de eliminación lógica que no generan valor nuevo. Contiene SOLO los campos que cambiaron, no el objeto completo. Ejemplo cambio de prioridad: {"prioridad_admin": "ALTA"}. Ejemplo asignación: {"tecnico_id": "uuid-del-tecnico", "estado": "ASIGNADO"}. Par con valor_anterior para reconstruir el historial de cambios de cualquier entidad.';


--
-- Name: COLUMN audit_logs.ip; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.ip IS 'Dirección IP del cliente que ejecutó la acción. Tipo INET (no VARCHAR). INET permite almacenar IPv4 e IPv6 correctamente y habilita operadores de red (<<, >>, subnet matching) para filtros de seguridad eficientes. NULL cuando la acción fue ejecutada por el sistema (sin petición HTTP del cliente). Capturada del header X-Forwarded-For o REMOTE_ADDR en la Edge Function.';


--
-- Name: COLUMN audit_logs.user_agent; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.user_agent IS 'User-Agent HTTP del cliente que ejecutó la acción. TEXT sin límite de longitud. NULL cuando la acción fue ejecutada por el sistema sin petición HTTP del cliente. Permite identificar el tipo de cliente (navegador, app móvil) y detectar patrones de uso anómalos en auditorías de seguridad.';


--
-- Name: COLUMN audit_logs.sucursal_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.sucursal_id IS 'FK opcional → sucursales(id) ON DELETE SET NULL. Sucursal en cuyo contexto se ejecutó la acción. NULL para acciones globales (SuperAdmin sin contexto de sucursal) o acciones de sistema sin actor humano. SET NULL: si la sucursal es eliminada físicamente, el log se conserva. Soportado por idx_audit_sucursal_fecha para el filtro RLS del Admin por sucursal.';


--
-- Name: COLUMN audit_logs.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.audit_logs.created_at IS 'Timestamp de creación del registro de auditoría. Generado en INSERT (NOW()). Completamente inmutable: sin UPDATE, sin trigger sobre esta tabla. Eje temporal de los índices de audit_logs:   idx_audit_fecha_desc      (created_at DESC) — timeline global.   idx_audit_modulo_fecha    (modulo, created_at DESC) — timeline por módulo.   idx_audit_sucursal_fecha  (sucursal_id, created_at DESC) — timeline por sucursal.';


--
-- Name: categorias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.categorias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid,
    nombre character varying(200) NOT NULL,
    descripcion character varying(500),
    activa boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid
);


--
-- Name: TABLE categorias; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.categorias IS 'Catálogo de categorías de ticket (clasificación de segundo nivel). empresa_id NULL = global (MVP). UUID = privada por empresa (futuro). Ref: docs/database/MODEL-PHYSICAL.md §7.9.';


--
-- Name: COLUMN categorias.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.categorias.empresa_id IS 'NULL = categoría global. UUID = privada de esa empresa. FK → empresas.id ON DELETE SET NULL (diferida a M-0031).';


--
-- Name: correos_guardados; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.correos_guardados (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    usuario_id uuid NOT NULL,
    correo text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


--
-- Name: empresa_correos_copia; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.empresa_correos_copia (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid NOT NULL,
    correo text NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT empresa_correos_copia_correo_check CHECK ((correo ~ '^[^@\s]+@[^@\s]+\.[^@\s]+$'::text))
);


--
-- Name: empresas; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.empresas (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    nombre_comercial character varying(200) NOT NULL,
    razon_social character varying(300) NOT NULL,
    identificacion_fiscal character varying(50) NOT NULL,
    logo_url text,
    color_primario character(7),
    color_secundario character(7),
    zona_horaria character varying(60) DEFAULT 'America/Mexico_City'::character varying NOT NULL,
    activa boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_empresas_color_primario CHECK (((color_primario IS NULL) OR (color_primario ~ '^#[0-9A-Fa-f]{6}$'::text))),
    CONSTRAINT chk_empresas_color_secundario CHECK (((color_secundario IS NULL) OR (color_secundario ~ '^#[0-9A-Fa-f]{6}$'::text)))
);


--
-- Name: TABLE empresas; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.empresas IS 'Raíz del modelo multi-tenant de Pide Servicio. Cada registro representa un cliente (empresa) del sistema. Toda entidad operativa pertenece directa o transitivamente a una empresa. El campo empresa_id en tablas operativas es el eje central de RLS. Ref: docs/database/MODEL-PHYSICAL.md §7.1.';


--
-- Name: COLUMN empresas.identificacion_fiscal; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.identificacion_fiscal IS 'RFC, RUT, NIT u equivalente fiscal según el país. Único globalmente. Validado por constraint uq_empresas_fiscal.';


--
-- Name: COLUMN empresas.logo_url; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.logo_url IS 'URL del logotipo almacenado en Supabase Storage (bucket logos). NULL = sin logo configurado.';


--
-- Name: COLUMN empresas.color_primario; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.color_primario IS 'Color de marca primario en formato #RRGGBB. NULL = sin branding configurado. Validado por constraint chk_empresas_color_primario.';


--
-- Name: COLUMN empresas.color_secundario; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.color_secundario IS 'Color de marca secundario en formato #RRGGBB. NULL = sin branding configurado. Validado por constraint chk_empresas_color_secundario.';


--
-- Name: COLUMN empresas.zona_horaria; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.zona_horaria IS 'Zona horaria IANA de la empresa (ej: America/Mexico_City, America/Bogota). Los timestamps en BD son siempre UTC. La conversión a zona local ocurre en la capa de presentación.';


--
-- Name: COLUMN empresas.activa; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.activa IS 'Soft disable de la empresa. false = empresa deshabilitada, sin acceso. PENDIENTE: el trigger que impide desactivar si existen tickets activos se agregará en la migración de la tabla tickets.';


--
-- Name: COLUMN empresas.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.created_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: COLUMN empresas.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.updated_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: COLUMN empresas.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.empresas.deleted_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: feriados; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.feriados (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid,
    pais_iso character(2),
    fecha date NOT NULL,
    nombre text NOT NULL,
    recurrente boolean DEFAULT false NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    deleted_at timestamp with time zone,
    CONSTRAINT chk_feriado_scope CHECK (((empresa_id IS NOT NULL) OR (pais_iso IS NOT NULL)))
);


--
-- Name: TABLE feriados; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.feriados IS 'Días no laborables. empresa_id o pais_iso deben ser no nulos.';


--
-- Name: COLUMN feriados.recurrente; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.feriados.recurrente IS 'TRUE = se repite cada año en la misma fecha de mes/día.';


--
-- Name: motivos_cancelacion; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.motivos_cancelacion (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid,
    texto character varying(300) NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid
);


--
-- Name: TABLE motivos_cancelacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.motivos_cancelacion IS 'Catálogo de razones para cancelar un ticket. Selección obligatoria al pasar ticket a estado CANCELADO. empresa_id NULL = global. UUID = específico de empresa. Ref: docs/database/MODEL-PHYSICAL.md §7.11.';


--
-- Name: COLUMN motivos_cancelacion.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.motivos_cancelacion.empresa_id IS 'NULL = motivo global. UUID = privado de esa empresa. FK → empresas.id ON DELETE SET NULL (diferida a M-0031).';


--
-- Name: motivos_rechazo; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.motivos_rechazo (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid,
    codigo character varying(30) NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion character varying(500),
    es_otro boolean DEFAULT false NOT NULL,
    orden integer NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_motivos_rechazo_orden CHECK ((orden > 0))
);


--
-- Name: TABLE motivos_rechazo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.motivos_rechazo IS 'Catálogo de razones para rechazar la solución de un ticket. es_otro = true → comentario libre obligatorio en ticket_historial. Un solo es_otro por alcance (empresa_id). Ref: docs/database/MODEL-PHYSICAL.md §7.12.';


--
-- Name: COLUMN motivos_rechazo.codigo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.motivos_rechazo.codigo IS 'Código corto único por alcance. Ej: NO_RESUELTO, OTRO. NULL-safe uniqueness vía índice parcial.';


--
-- Name: COLUMN motivos_rechazo.es_otro; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.motivos_rechazo.es_otro IS 'Si true, el campo rejection_comment en ticket_historial es obligatorio. Solo un motivo con es_otro = true por alcance (validado por índice parcial único).';


--
-- Name: notificaciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.notificaciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    destinatario_id uuid NOT NULL,
    tipo_evento character varying(100) NOT NULL,
    titulo character varying(100) NOT NULL,
    cuerpo character varying(500) NOT NULL,
    prioridad character varying(20) DEFAULT 'medium'::character varying NOT NULL,
    canal public.canal_notificacion_tipo NOT NULL,
    estado_entrega public.estado_entrega_tipo DEFAULT 'PENDIENTE'::public.estado_entrega_tipo NOT NULL,
    leida boolean DEFAULT false NOT NULL,
    leida_en timestamp with time zone,
    ticket_id uuid,
    metadata jsonb,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    CONSTRAINT chk_notif_prioridad CHECK (((prioridad)::text = ANY ((ARRAY['low'::character varying, 'medium'::character varying, 'high'::character varying, 'critical'::character varying])::text[])))
);


--
-- Name: TABLE notificaciones; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.notificaciones IS 'Registro de alertas generadas por el sistema para los usuarios. Fuente de verdad del centro de notificaciones de la PWA. canal IN_APP: consultado directamente por el Frontend (leida = false). canal EMAIL / PUSH: procesados por Edge Functions tras el INSERT. Sin deleted_at: las notificaciones se marcan como leídas, no se borran lógicamente. preferencias_notificacion (creada en M-005) determina si se genera la notificación y por qué canal antes del INSERT en esta tabla. Ref: docs/database/MODEL-PHYSICAL.md §7.19.';


--
-- Name: COLUMN notificaciones.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.id IS 'PK UUID. Generada automáticamente (gen_random_uuid()) en cada INSERT. Identifica unívocamente cada notificación del sistema.';


--
-- Name: COLUMN notificaciones.destinatario_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.destinatario_id IS 'FK activa → usuarios(id) ON DELETE CASCADE. Usuario que debe recibir esta notificación. CASCADE: si el usuario es eliminado físicamente, sus notificaciones también. La eliminación de usuarios es siempre lógica (deleted_at) en el sistema; el CASCADE actúa como salvaguarda de integridad referencial. Primer componente del índice compuesto idx_notif_destinatario_leida.';


--
-- Name: COLUMN notificaciones.tipo_evento; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.tipo_evento IS 'Identificador del evento de negocio que generó esta notificación. Formato libre de puntos: dominio.accion. Ejemplos: "ticket.asignado", "ticket.cerrado", "ticket.comentario_nuevo", "usuario.creado", "sla.vencido". Extensible sin migrar: nuevos tipos de evento se agregan sin ALTER TABLE. El Frontend usa este valor para determinar el icono y el destino de navegación.';


--
-- Name: COLUMN notificaciones.titulo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.titulo IS 'Título corto de la notificación. Máx. 100 caracteres. Usado como asunto del correo en canal EMAIL y como título del push en PUSH. En canal IN_APP se muestra en la cabecera del ítem en el panel de notificaciones.';


--
-- Name: COLUMN notificaciones.cuerpo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.cuerpo IS 'Cuerpo descriptivo de la notificación. Máx. 500 caracteres. Puede contener el detalle del evento (ej: nombre del técnico asignado, código del ticket). En canal EMAIL puede ser el texto de resumen; el cuerpo HTML completo lo genera la Edge Function usando tipo_evento y metadata.';


--
-- Name: COLUMN notificaciones.prioridad; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.prioridad IS 'Nivel de urgencia visual de la notificación en la UI. Valores (minúsculas): low, medium, high, critical. Controlado por chk_notif_prioridad. Default: "medium". Determina el color del indicador, el sonido (si aplica) y el orden en el panel. No equivale a la prioridad del ticket; es la prioridad de presentación de la alerta.';


--
-- Name: COLUMN notificaciones.canal; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.canal IS 'Canal de entrega de la notificación. ENUM canal_notificacion_tipo (M-002). IN_APP: leída directamente desde esta tabla por el Frontend. EMAIL:  despachada por Edge Function al proveedor de correo configurado. PUSH:   enviada por Edge Function vía Web Push (service worker, VAPID). Una notificación tiene exactamente un canal; multi-canal = múltiples filas.';


--
-- Name: COLUMN notificaciones.estado_entrega; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.estado_entrega IS 'Estado del proceso de entrega al canal externo. ENUM estado_entrega_tipo (M-002). PENDIENTE: en cola, aún no procesada por la Edge Function. ENVIADO:   confirmación de entrega exitosa al canal (API del proveedor o push broker). FALLIDO:   error definitivo; no se reintentará (agotados los reintentos del sistema). Para canal IN_APP se marca ENVIADO al insertarse (no hay canal externo que procesar). Soportado por idx_notif_estado_entrega (partial WHERE estado_entrega = PENDIENTE).';


--
-- Name: COLUMN notificaciones.leida; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.leida IS 'Indica si el destinatario ha visto la notificación en la PWA. false: no leída — se muestra el indicador de notificación pendiente en la UI. true:  leída — el usuario interactuó con la notificación o la marcó como leída. Segundo componente del índice idx_notif_destinatario_leida (destinatario_id, leida, created_at DESC): filtra no leídas del panel.';


--
-- Name: COLUMN notificaciones.leida_en; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.leida_en IS 'Timestamp del momento en que el destinatario marcó la notificación como leída. NULL = notificación aún no leída (leida = false). NOT NULL = fecha y hora exacta del marcado como leída. Seteado exclusivamente por el Backend en el UPDATE leida = true. El trigger trg_notificaciones_before_update actualiza updated_at automáticamente en el mismo statement.';


--
-- Name: COLUMN notificaciones.ticket_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.ticket_id IS 'FK opcional → tickets(id) ON DELETE SET NULL. NULL cuando la notificación no está asociada a un ticket específico (ej: notificaciones de sistema, bienvenida de usuario). NOT NULL: permite al Frontend navegar directamente al ticket desde la notificación. SET NULL: si el ticket es eliminado físicamente, la notificación se conserva. Soportado por idx_notif_ticket.';


--
-- Name: COLUMN notificaciones.metadata; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.metadata IS 'JSONB de datos adicionales contextuales de la notificación. Contenido variable según tipo_evento. Ejemplos:   ticket.asignado: {"tecnico_nombre": "Juan", "ticket_codigo": "PS-000001"}   sla.vencido:     {"minutos_retraso": 30, "prioridad": "ALTA"}   push:            {"url": "/tickets/PS-000001", "badge": 3} La Edge Function de EMAIL usa metadata para construir el cuerpo HTML del correo. La Edge Function de PUSH incluye metadata en el payload del push broker.';


--
-- Name: COLUMN notificaciones.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.created_at IS 'Timestamp de inserción de la notificación. Generado en INSERT (NOW()). Tercer componente del índice idx_notif_destinatario_leida (destinatario_id, leida, created_at DESC): ordena las notificaciones del panel del más reciente al más antiguo.';


--
-- Name: COLUMN notificaciones.updated_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.updated_at IS 'Timestamp de la última modificación de la fila. Actualizado automáticamente por trg_notificaciones_before_update usando trg_fn_set_updated_at_only() (M-002). Se actualiza al marcar como leída (leida = true / leida_en) y al cambiar estado_entrega (PENDIENTE → ENVIADO | FALLIDO).';


--
-- Name: COLUMN notificaciones.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.created_by IS 'FK opcional → usuarios(id) ON DELETE SET NULL. NULL cuando la notificación fue generada por el sistema sin actor humano (ej: cron de SLA, trigger automático de cambio de estado). NOT NULL cuando un usuario (Admin, Técnico) generó la acción que desencadenó la notificación. SET NULL si el usuario creador es eliminado físicamente.';


--
-- Name: COLUMN notificaciones.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.notificaciones.updated_by IS 'FK opcional → usuarios(id) ON DELETE SET NULL. NULL cuando la actualización fue ejecutada por el sistema (Edge Function, cron). NOT NULL cuando un usuario marcó la notificación como leída o la actualizó. SET NULL si el usuario que actualizó es eliminado físicamente.';


--
-- Name: parametros_sistema; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.parametros_sistema (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid,
    clave character varying(100) NOT NULL,
    valor text NOT NULL,
    tipo_dato public.tipo_dato_parametro_tipo NOT NULL,
    descripcion character varying(300),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by uuid
);


--
-- Name: TABLE parametros_sistema; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.parametros_sistema IS 'Configuración clave-valor del sistema. empresa_id NULL = global. UUID = override por empresa. Sin deleted_at: parámetros se actualizan, no se eliminan lógicamente. Sin created_by: configurados en deployment, cambios via updated_by. Ref: docs/database/MODEL-PHYSICAL.md §7.13.';


--
-- Name: COLUMN parametros_sistema.tipo_dato; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.parametros_sistema.tipo_dato IS 'Tipo para deserializar correctamente el campo valor (TEXT). TEXTO → string literal. ENTERO → parseInt. BOOLEANO → "true"|"false". JSON → JSON.parse().';


--
-- Name: permisos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.permisos (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    codigo character varying(100) NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion character varying(500),
    modulo character varying(50) NOT NULL,
    recurso character varying(50) NOT NULL,
    accion character varying(50) NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    CONSTRAINT chk_permisos_codigo_formato CHECK (((codigo)::text ~ '^[a-z_]+\.[a-z_]+\.[a-z_]+$'::text))
);


--
-- Name: TABLE permisos; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.permisos IS 'Catálogo RBAC de acciones del sistema. Formato de codigo: modulo.recurso.accion (ej: tickets.ticket.crear). Ref: docs/database/MODEL-PHYSICAL.md §7.6.';


--
-- Name: COLUMN permisos.codigo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.permisos.codigo IS 'Único globalmente. Formato: modulo.recurso.accion (lowercase, guiones bajos permitidos).';


--
-- Name: COLUMN permisos.modulo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.permisos.modulo IS 'Módulo del sistema. Ej: tickets, usuarios, empresas.';


--
-- Name: COLUMN permisos.recurso; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.permisos.recurso IS 'Entidad afectada. Ej: ticket, usuario, empresa.';


--
-- Name: COLUMN permisos.accion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.permisos.accion IS 'Verbo de la acción. Ej: ver, crear, editar, cancelar.';


--
-- Name: preferencias_notificacion; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.preferencias_notificacion (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    usuario_id uuid NOT NULL,
    canal_in_app boolean DEFAULT true NOT NULL,
    canal_email boolean DEFAULT true NOT NULL,
    canal_push boolean DEFAULT false NOT NULL,
    evento_ticket_asignado boolean DEFAULT true NOT NULL,
    evento_ticket_actualizado boolean DEFAULT true NOT NULL,
    evento_comentario_nuevo boolean DEFAULT true NOT NULL,
    evento_sla_vencido boolean DEFAULT true NOT NULL,
    evento_ticket_reabierto boolean DEFAULT true NOT NULL,
    resumen_diario boolean DEFAULT false NOT NULL,
    modo_silencioso boolean DEFAULT false NOT NULL,
    silencio_hora_inicio time without time zone,
    silencio_hora_fin time without time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by uuid,
    CONSTRAINT chk_preferencias_silencio CHECK (((modo_silencioso = false) OR ((silencio_hora_inicio IS NOT NULL) AND (silencio_hora_fin IS NOT NULL))))
);


--
-- Name: TABLE preferencias_notificacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.preferencias_notificacion IS 'Configuración individual de canales y eventos de notificación por usuario. Creada automáticamente por trigger (trg_usuarios_after_insert) al registrar un usuario. Relación 1:1 con usuarios. Sin borrado lógico: el ciclo de vida está atado al usuario. Sin version: baja concurrencia de escritura (usuario solo modifica sus propias prefs). Ref: docs/database/MODEL-PHYSICAL.md §7.20.';


--
-- Name: COLUMN preferencias_notificacion.usuario_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.preferencias_notificacion.usuario_id IS 'FK 1:1 → usuarios(id) ON DELETE CASCADE. Clave de acceso primaria: siempre se consulta por usuario_id. El índice UNIQUE sobre esta columna hace eficiente este acceso.';


--
-- Name: COLUMN preferencias_notificacion.canal_in_app; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.preferencias_notificacion.canal_in_app IS 'Habilita notificaciones dentro de la PWA (centro de notificaciones/campana). Default true: canal principal siempre activo al crear el usuario.';


--
-- Name: COLUMN preferencias_notificacion.canal_email; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.preferencias_notificacion.canal_email IS 'Habilita notificaciones por correo electrónico. Default true: activo al crear el usuario. El usuario puede desactivarlo.';


--
-- Name: COLUMN preferencias_notificacion.canal_push; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.preferencias_notificacion.canal_push IS 'Habilita notificaciones push web (requiere permiso del navegador). Default false: el usuario debe activarlo explícitamente y conceder permiso.';


--
-- Name: COLUMN preferencias_notificacion.modo_silencioso; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.preferencias_notificacion.modo_silencioso IS 'Suprime todas las notificaciones en el rango [silencio_hora_inicio, silencio_hora_fin]. Si es true, silencio_hora_inicio y silencio_hora_fin son obligatorios (validado por constraint chk_preferencias_silencio). Las horas cruzan medianoche en turnos nocturnos; la aplicación maneja el wrap.';


--
-- Name: COLUMN preferencias_notificacion.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.preferencias_notificacion.updated_by IS 'FK → usuarios(id) ON DELETE SET NULL. Usuario que realizó la última modificación de preferencias. NULL cuando fue creado por el trigger del sistema (INSERT inicial).';


--
-- Name: role_permissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.role_permissions (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    rol_codigo public.rol_tipo NOT NULL,
    permiso_id uuid NOT NULL,
    empresa_id uuid,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid
);


--
-- Name: TABLE role_permissions; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.role_permissions IS 'Tabla puente RBAC. Asigna permisos a roles. (D-013) empresa_id NULL = asignación global (MVP). empresa_id UUID = override por empresa (futuro). Una asignación se crea o desactiva, nunca se modifica. Sin updated_at, updated_by, deleted_at por diseño. Ref: docs/database/MODEL-PHYSICAL.md §7.22.';


--
-- Name: COLUMN role_permissions.rol_codigo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.role_permissions.rol_codigo IS 'FK → roles.codigo (ENUM rol_tipo). Rol al que se asigna el permiso: SUPERADMIN, ADMIN, SUPERVISOR, TECNICO, TRABAJADOR, USUARIO.';


--
-- Name: COLUMN role_permissions.permiso_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.role_permissions.permiso_id IS 'FK → permisos.id ON DELETE CASCADE. Permiso asignado al rol. Si el permiso se elimina, la asignación desaparece automáticamente por CASCADE.';


--
-- Name: COLUMN role_permissions.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.role_permissions.empresa_id IS 'NULL = asignación global (todos los tenants). UUID = override exclusivo para esa empresa. FK → empresas.id ON DELETE CASCADE diferida a M-0031. En MVP v1.0 siempre es NULL.';


--
-- Name: COLUMN role_permissions.activo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.role_permissions.activo IS 'false = asignación desactivada sin eliminar el registro. Permite auditar el historial de cambios RBAC.';


--
-- Name: COLUMN role_permissions.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.role_permissions.created_by IS 'UUID del usuario que creó la asignación. NULL para seeds de bootstrap (sistema). FK → usuarios.id ON DELETE SET NULL diferida a M-0031.';


--
-- Name: roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.roles (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    codigo public.rol_tipo NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion character varying(500),
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid
);


--
-- Name: TABLE roles; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.roles IS 'Catálogo RBAC de roles del sistema. Exactamente 6 registros (uno por valor de rol_tipo). Ref: docs/database/MODEL-PHYSICAL.md §7.5.';


--
-- Name: COLUMN roles.codigo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.roles.codigo IS 'Vincula directamente con ENUM rol_tipo. UNIQUE.';


--
-- Name: COLUMN roles.activo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.roles.activo IS 'Desactivar un rol no elimina el ENUM. Previene asignaciones pero no bloquea usuarios existentes.';


--
-- Name: sla_configuraciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.sla_configuraciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid NOT NULL,
    tipo_servicio_id uuid,
    prioridad public.prioridad_tipo NOT NULL,
    horas_primera_atencion integer NOT NULL,
    horas_resolucion integer NOT NULL,
    horas_laborales_inicio smallint DEFAULT 8 NOT NULL,
    horas_laborales_fin smallint DEFAULT 18 NOT NULL,
    dias_laborales smallint[] DEFAULT '{1,2,3,4,5}'::smallint[] NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_sla_horario_coherente CHECK ((horas_laborales_inicio < horas_laborales_fin)),
    CONSTRAINT chk_sla_horas_coherentes CHECK ((horas_primera_atencion <= horas_resolucion)),
    CONSTRAINT sla_configuraciones_horas_laborales_fin_check CHECK (((horas_laborales_fin >= 1) AND (horas_laborales_fin <= 24))),
    CONSTRAINT sla_configuraciones_horas_laborales_inicio_check CHECK (((horas_laborales_inicio >= 0) AND (horas_laborales_inicio <= 23))),
    CONSTRAINT sla_configuraciones_horas_primera_atencion_check CHECK ((horas_primera_atencion > 0)),
    CONSTRAINT sla_configuraciones_horas_resolucion_check CHECK ((horas_resolucion > 0))
);


--
-- Name: TABLE sla_configuraciones; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.sla_configuraciones IS 'Matriz SLA: empresa × tipo_servicio × prioridad → tiempos límite.';


--
-- Name: COLUMN sla_configuraciones.tipo_servicio_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sla_configuraciones.tipo_servicio_id IS 'NULL = configuración por defecto de la empresa para cualquier tipo de servicio.';


--
-- Name: COLUMN sla_configuraciones.horas_primera_atencion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sla_configuraciones.horas_primera_atencion IS 'Horas laborales hasta primera atención requerida.';


--
-- Name: COLUMN sla_configuraciones.horas_resolucion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sla_configuraciones.horas_resolucion IS 'Horas laborales hasta resolución completa.';


--
-- Name: COLUMN sla_configuraciones.dias_laborales; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sla_configuraciones.dias_laborales IS 'ISO: 1=lunes, 7=domingo.';


--
-- Name: sucursales; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.sucursales (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion character varying(500),
    direccion character varying(400),
    responsable_id uuid,
    activa boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    codigo character varying(20)
);


--
-- Name: TABLE sucursales; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.sucursales IS 'Unidades geográficas u organizacionales dentro de una empresa. Los tickets, áreas y usuarios están asociados a una sucursal específica. Determina el alcance operativo de Supervisores y, parcialmente, de Técnicos. Ref: docs/database/MODEL-PHYSICAL.md §7.2.';


--
-- Name: COLUMN sucursales.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.empresa_id IS 'FK activa → empresas.id ON DELETE RESTRICT (fk_sucursales_empresas). No se puede eliminar una empresa que tiene sucursales asociadas.';


--
-- Name: COLUMN sucursales.nombre; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.nombre IS 'Nombre de la sucursal. Único por empresa (validado por uq_sucursales_empresa_nombre). El índice es parcial WHERE deleted_at IS NULL: permite reutilizar el nombre tras borrado lógico.';


--
-- Name: COLUMN sucursales.responsable_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.responsable_id IS 'Usuario responsable de la sucursal. Opcional. FK diferida → usuarios.id ON DELETE SET NULL (M-0031).';


--
-- Name: COLUMN sucursales.activa; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.activa IS 'Soft disable de la sucursal. false = sucursal deshabilitada. PENDIENTE: trigger "no desactivar si única activa de la empresa" y "no desactivar si tickets activos" — migraciones posteriores.';


--
-- Name: COLUMN sucursales.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.created_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: COLUMN sucursales.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.updated_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: COLUMN sucursales.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.sucursales.deleted_by IS 'FK diferida → usuarios.id ON DELETE SET NULL. Se agrega en M-0031.';


--
-- Name: tecnico_sucursales; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tecnico_sucursales (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    tecnico_id uuid NOT NULL,
    sucursal_id uuid NOT NULL,
    es_principal boolean DEFAULT false NOT NULL,
    activa boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid
);


--
-- Name: TABLE tecnico_sucursales; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.tecnico_sucursales IS 'Tabla puente muchos-a-muchos entre Técnicos y Sucursales. Implementa la asignación geográfica de Técnicos a sucursales (Decisión ARQ-009). Un Técnico puede operar en múltiples sucursales; exactamente una es la principal. Sin deleted_at: el soft disable usa activa = false para preservar historial. El invariante de única sucursal principal se garantiza por trigger. Ref: docs/database/MODEL-PHYSICAL.md §7.23.';


--
-- Name: COLUMN tecnico_sucursales.tecnico_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tecnico_sucursales.tecnico_id IS 'FK → usuarios(id) ON DELETE CASCADE. Debe referenciar un usuario con rol = TECNICO. La validación de rol es responsabilidad del Backend/Edge Function (no de trigger).';


--
-- Name: COLUMN tecnico_sucursales.sucursal_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tecnico_sucursales.sucursal_id IS 'FK → sucursales(id) ON DELETE CASCADE. Sucursal donde el Técnico está habilitado para operar y recibir asignaciones.';


--
-- Name: COLUMN tecnico_sucursales.es_principal; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tecnico_sucursales.es_principal IS 'Indica si esta es la sucursal principal del Técnico. Solo puede haber una sucursal principal por Técnico. Invariante garantizado por trigger trg_tecnico_sucursales_guard_principal. La sucursal principal determina el dashboard y zona de trabajo por defecto.';


--
-- Name: COLUMN tecnico_sucursales.activa; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tecnico_sucursales.activa IS 'Soft disable del vínculo técnico-sucursal. false = el Técnico no puede recibir asignaciones en esta sucursal. El registro permanece para preservar trazabilidad de asignaciones históricas. No equivale a deleted_at: no hay borrado lógico en esta tabla.';


--
-- Name: COLUMN tecnico_sucursales.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tecnico_sucursales.created_by IS 'FK → usuarios(id) ON DELETE SET NULL. Usuario Admin/SuperAdmin que creó el vínculo de asignación.';


--
-- Name: COLUMN tecnico_sucursales.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tecnico_sucursales.updated_by IS 'FK → usuarios(id) ON DELETE SET NULL. Usuario Admin/SuperAdmin que realizó la última modificación del vínculo.';


--
-- Name: ticket_asignaciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ticket_asignaciones (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    ticket_id uuid NOT NULL,
    tecnico_id uuid NOT NULL,
    asignador_id uuid NOT NULL,
    es_reasignacion boolean DEFAULT false NOT NULL,
    tecnico_anterior_id uuid,
    motivo_reasignacion character varying(500),
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid NOT NULL,
    CONSTRAINT chk_asignaciones_reasignacion CHECK (((es_reasignacion = false) OR (tecnico_anterior_id IS NOT NULL)))
);

ALTER TABLE ONLY public.ticket_asignaciones FORCE ROW LEVEL SECURITY;


--
-- Name: TABLE ticket_asignaciones; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.ticket_asignaciones IS 'APPEND-ONLY. Sin UPDATE ni DELETE. Sin updated_at/updated_by/deleted_at/deleted_by/version. Historial cronológico de todas las asignaciones de Técnico a un Ticket. Cada INSERT registra una asignación inicial o reasignación con su responsable y motivo. Permite auditar la cadena completa de responsabilidad técnica del ticket. Ref: docs/database/MODEL-PHYSICAL.md §7.15.';


--
-- Name: COLUMN ticket_asignaciones.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.id IS 'PK UUID. Generada automáticamente (gen_random_uuid()) en cada INSERT. Identifica unívocamente cada evento de asignación o reasignación en el sistema.';


--
-- Name: COLUMN ticket_asignaciones.ticket_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.ticket_id IS 'FK activa → tickets(id) ON DELETE RESTRICT. Ticket al que se refiere esta asignación. RESTRICT: el historial de asignaciones debe preservarse; un ticket no puede eliminarse si tiene asignaciones registradas.';


--
-- Name: COLUMN ticket_asignaciones.tecnico_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.tecnico_id IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Técnico al que se asignó el ticket en esta operación. Solo usuarios con rol TECNICO y estado laboral ACTIVO pueden recibir asignaciones (validado por el Backend, no por esta FK).';


--
-- Name: COLUMN ticket_asignaciones.asignador_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.asignador_id IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Admin o SuperAdmin que ejecutó la asignación o reasignación. RESTRICT: la trazabilidad de quién asignó es requisito de auditoría.';


--
-- Name: COLUMN ticket_asignaciones.es_reasignacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.es_reasignacion IS 'false = primera asignación del ticket (estado SIN_ASIGNAR → ASIGNADO). true  = reasignación posterior; tecnico_anterior_id es NOT NULL (garantizado por chk_asignaciones_reasignacion). Permite filtrar exclusivamente reasignaciones via idx_asignaciones_reasignaciones.';


--
-- Name: COLUMN ticket_asignaciones.tecnico_anterior_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.tecnico_anterior_id IS 'FK activa → usuarios(id) ON DELETE SET NULL. Técnico que fue reemplazado en una reasignación. NULL en la primera asignación del ticket (es_reasignacion = false). SET NULL: si el técnico anterior es dado de baja, la fila se conserva con referencia anulada para mantener el registro histórico de la reasignación. NOT NULL obligatorio cuando es_reasignacion = true (chk_asignaciones_reasignacion).';


--
-- Name: COLUMN ticket_asignaciones.motivo_reasignacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.motivo_reasignacion IS 'Justificación textual del cambio de técnico. Máx. 500 caracteres. NULL en asignaciones iniciales. En reasignaciones puede ser obligatorio por política de empresa (validado por el Backend, no por restricción de BD). Ej: "Técnico en vacaciones", "Reasignado por carga de trabajo", "Especialista requerido".';


--
-- Name: COLUMN ticket_asignaciones.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.created_at IS 'Timestamp de la asignación. Inmutable. Generado en INSERT (NOW()). Columna central para los índices de cronología de asignaciones: idx_asignaciones_ticket ordena por (ticket_id, created_at DESC).';


--
-- Name: COLUMN ticket_asignaciones.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_asignaciones.created_by IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que generó esta fila. Igual a asignador_id en la práctica. Columna estándar del sistema para trazabilidad de creación. RESTRICT: el creador debe ser identificable (consistente con asignador_id).';


--
-- Name: ticket_codigo_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.ticket_codigo_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    MAXVALUE 999999
    CACHE 1;


--
-- Name: SEQUENCE ticket_codigo_seq; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON SEQUENCE public.ticket_codigo_seq IS 'Secuencia para generar códigos de ticket visibles. Formato: PS-XXXXXX (PS-000001 a PS-999999). Usada por trigger trg_tickets_before_insert_set_codigo en la tabla tickets. Sin CYCLE: agotamiento falla explícitamente. Sin CACHE: sin gaps por restart. Ref: docs/database/MODEL-PHYSICAL.md §7.14 tickets.codigo.';


--
-- Name: ticket_comentarios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ticket_comentarios (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    ticket_id uuid NOT NULL,
    autor_id uuid NOT NULL,
    cuerpo character varying(2000) NOT NULL,
    es_interno boolean DEFAULT false NOT NULL,
    editado_en timestamp with time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by uuid,
    created_by uuid NOT NULL,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_comentarios_cuerpo CHECK ((length((cuerpo)::text) >= 1))
);


--
-- Name: TABLE ticket_comentarios; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.ticket_comentarios IS 'Hilo de mensajes y notas del ticket. es_interno = false → visible a todos los participantes del ticket. es_interno = true  → visible solo al personal interno (RLS post-migración). Permite edición del cuerpo: editado_en y updated_by gestionados por el Backend. Borrado lógico: deleted_at / deleted_by (solo SuperAdmin, regla de negocio). Sin updated_at: editado_en cubre la trazabilidad de ediciones de contenido. Sin version: no especificado en el modelo físico §7.16. Sin triggers: trg_fn_set_updated_at y trg_fn_set_updated_at_only no aplican (requieren updated_at o version que esta tabla no tiene). Ref: docs/database/MODEL-PHYSICAL.md §7.16.';


--
-- Name: COLUMN ticket_comentarios.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.id IS 'PK UUID. Generada automáticamente (gen_random_uuid()) en cada INSERT. Identifica unívocamente cada comentario del sistema.';


--
-- Name: COLUMN ticket_comentarios.ticket_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.ticket_id IS 'FK activa → tickets(id) ON DELETE RESTRICT. Ticket al que pertenece este comentario. RESTRICT: el ticket no puede eliminarse de la BD mientras tenga comentarios.';


--
-- Name: COLUMN ticket_comentarios.autor_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.autor_id IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que escribió el comentario. RESTRICT: el autor debe ser identificable para garantizar la trazabilidad del hilo. En la práctica igual a created_by; se mantiene separado para expresar la autoría del contenido (quien lo escribió) con independencia de la auditoría de fila.';


--
-- Name: COLUMN ticket_comentarios.cuerpo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.cuerpo IS 'Contenido del comentario. Máx. 2000 caracteres. chk_comentarios_cuerpo garantiza LENGTH >= 1 (no puede ser cadena vacía). La validación de cadenas de solo espacios vive en el Backend. El Backend actualiza este campo junto con editado_en y updated_by al editar.';


--
-- Name: COLUMN ticket_comentarios.es_interno; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.es_interno IS 'Controla la visibilidad del comentario en el hilo del ticket. false (público): visible a todos los participantes, incluido el solicitante. true  (interno): visible únicamente al personal interno (Técnico, Admin, SuperAdmin). El filtro de visibilidad se aplica en las políticas RLS (migración posterior). Soportado por idx_comentarios_internos (partial index WHERE es_interno = true).';


--
-- Name: COLUMN ticket_comentarios.editado_en; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.editado_en IS 'Timestamp de la última edición del cuerpo del comentario. NULL = comentario nunca editado desde su creación. NOT NULL = fecha y hora de la última modificación del contenido. Seteado exclusivamente por el Backend en el UPDATE de cuerpo. Sustituye a updated_at, que no existe en esta tabla por diseño del modelo físico. Solo el autor puede editar su propio comentario (regla de negocio, no en BD).';


--
-- Name: COLUMN ticket_comentarios.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.created_at IS 'Timestamp de creación del comentario. Generado en INSERT (NOW()). Eje temporal del índice idx_comentarios_ticket (ticket_id, created_at ASC): ordena el hilo del ticket cronológicamente del más antiguo al más reciente.';


--
-- Name: COLUMN ticket_comentarios.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.updated_by IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que realizó la última edición del cuerpo del comentario. NULL cuando el comentario nunca fue editado (editado_en IS NULL). SET NULL: si el editor es dado de baja, el comentario se preserva. Actualizado por el Backend junto con cuerpo y editado_en en el mismo UPDATE.';


--
-- Name: COLUMN ticket_comentarios.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.created_by IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que generó esta fila. Columna estándar del sistema para trazabilidad. En la práctica igual a autor_id. RESTRICT: el creador debe ser identificable.';


--
-- Name: COLUMN ticket_comentarios.deleted_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.deleted_at IS 'Timestamp del borrado lógico. NULL = comentario activo y visible en el hilo. NOT NULL = comentario eliminado; no se muestra al listar el hilo del ticket. Solo SuperAdmin puede borrar comentarios (regla de negocio en Backend y RLS). Soportado por idx_comentarios_internos (partial WHERE deleted_at IS NULL).';


--
-- Name: COLUMN ticket_comentarios.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_comentarios.deleted_by IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que ejecutó el borrado lógico del comentario. NULL cuando deleted_at IS NULL (comentario activo). SET NULL: si quien borró el comentario es dado de baja, el registro se conserva.';


--
-- Name: ticket_evidencias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ticket_evidencias (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    ticket_id uuid NOT NULL,
    autor_id uuid NOT NULL,
    tipo public.evidencia_tipo NOT NULL,
    nombre_original character varying(255) NOT NULL,
    tipo_mime character varying(100) NOT NULL,
    tamano_bytes bigint NOT NULL,
    url_almacenamiento text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid NOT NULL,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_evidencias_tamano CHECK ((tamano_bytes > 0))
);


--
-- Name: TABLE ticket_evidencias; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.ticket_evidencias IS 'Archivos adjuntos del ticket clasificados por momento del ciclo de vida. tipo INICIAL: documenta el problema (al crear ticket o en estados tempranos). tipo FINAL: documenta la resolución (al completar, EN_PROCESO → PENDIENTE_VALIDACION). Solo INSERT y soft-delete: las evidencias no se modifican una vez subidas. El archivo físico en Supabase Storage se conserva aunque deleted_at sea NOT NULL. url_almacenamiento guarda el path en Storage, no la URL pública. Sin updated_at, sin updated_by: sin UPDATE legítimo sobre columnas de contenido. Sin version: no especificado en el modelo físico §7.17. Sin triggers: sin UPDATE que disparar. Ref: docs/database/MODEL-PHYSICAL.md §7.17.';


--
-- Name: COLUMN ticket_evidencias.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.id IS 'PK UUID. Generada automáticamente (gen_random_uuid()) en cada INSERT. Identifica unívocamente cada archivo de evidencia del sistema.';


--
-- Name: COLUMN ticket_evidencias.ticket_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.ticket_id IS 'FK activa → tickets(id) ON DELETE RESTRICT. Ticket al que pertenece esta evidencia. RESTRICT: el ticket no puede eliminarse de la BD mientras tenga evidencias.';


--
-- Name: COLUMN ticket_evidencias.autor_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.autor_id IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que subió el archivo al ticket. RESTRICT: el autor debe ser identificable para garantizar la trazabilidad. En la práctica igual a created_by; se mantiene separado para expresar la autoría del contenido (quien lo subió) con independencia de la auditoría de fila.';


--
-- Name: COLUMN ticket_evidencias.tipo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.tipo IS 'Momento del ciclo de vida en que se subió la evidencia. ENUM evidencia_tipo (M-002). INICIAL: documenta el problema o estado antes de la resolución.          Se sube al crear el ticket o durante estados activos tempranos. FINAL:   documenta la solución aplicada.          Se sube al completar el ticket (EN_PROCESO → PENDIENTE_VALIDACION). Soportado por idx_evidencias_ticket (ticket_id, tipo) para agrupar por tipo e idx_evidencias_final_activas (partial index) para verificar existencia de FINAL.';


--
-- Name: COLUMN ticket_evidencias.nombre_original; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.nombre_original IS 'Nombre del archivo tal como fue subido por el usuario. Máx. 255 caracteres. Se conserva para presentar al usuario un nombre reconocible en la UI. No se usa como identificador único; url_almacenamiento es el identificador en Storage.';


--
-- Name: COLUMN ticket_evidencias.tipo_mime; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.tipo_mime IS 'Tipo MIME del archivo. Máx. 100 caracteres. Ejemplos: "image/jpeg", "image/png", "image/webp", "application/pdf". Los tipos permitidos se validan en el Backend contra parametros_sistema (clave: ticket.formatos_evidencia_permitidos). Permite al Frontend renderizar el archivo correctamente (imagen vs. documento).';


--
-- Name: COLUMN ticket_evidencias.tamano_bytes; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.tamano_bytes IS 'Tamaño del archivo en bytes. Estrictamente positivo (chk_evidencias_tamano). BIGINT para soportar archivos de hasta ~9.2 EB, bien por encima del límite de 25 MB definido en parametros_sistema (ticket.max_tamano_evidencia_mb). La validación del límite superior la aplica el Backend antes del INSERT.';


--
-- Name: COLUMN ticket_evidencias.url_almacenamiento; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.url_almacenamiento IS 'Path del archivo en Supabase Storage. TEXT sin límite de longitud. Formato esperado: "{bucket}/{carpeta}/{nombre_unico}" — no es una URL HTTP. El Backend construye la URL firmada en tiempo de consulta (createSignedUrl o getPublicUrl de la API de Storage). El archivo físico en Storage se conserva aunque deleted_at sea NOT NULL: el borrado lógico solo oculta la referencia en BD, nunca elimina el archivo.';


--
-- Name: COLUMN ticket_evidencias.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.created_at IS 'Timestamp de la carga de la evidencia. Generado en INSERT (NOW()). Inmutable. No existe UPDATE legítimo en esta tabla. Para ordenar evidencias cronológicamente dentro de un tipo, usar ORDER BY created_at en la consulta junto con el índice idx_evidencias_ticket (ticket_id, tipo).';


--
-- Name: COLUMN ticket_evidencias.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.created_by IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que generó esta fila. Columna estándar del sistema para trazabilidad. En la práctica igual a autor_id. RESTRICT: el creador debe ser identificable.';


--
-- Name: COLUMN ticket_evidencias.deleted_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.deleted_at IS 'Timestamp del borrado lógico. NULL = evidencia activa y visible en el ticket. NOT NULL = evidencia eliminada lógicamente; no se muestra al listar adjuntos. El archivo físico en Supabase Storage se conserva en todos los casos. Solo SuperAdmin puede borrar evidencias (regla de negocio en Backend y RLS). Soportado por idx_evidencias_final_activas (partial WHERE deleted_at IS NULL).';


--
-- Name: COLUMN ticket_evidencias.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_evidencias.deleted_by IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que ejecutó el borrado lógico de la evidencia. NULL cuando deleted_at IS NULL (evidencia activa). SET NULL: si quien borró la evidencia es dado de baja, el registro se conserva.';


--
-- Name: ticket_historial; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.ticket_historial (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    ticket_id uuid NOT NULL,
    actor_id uuid,
    tipo_evento public.tipo_evento_historial_tipo NOT NULL,
    estado_anterior public.ticket_estado_tipo,
    estado_nuevo public.ticket_estado_tipo,
    comentario_texto text,
    rejection_reason_id uuid,
    rejection_comment text,
    metadata jsonb,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid
);

ALTER TABLE ONLY public.ticket_historial FORCE ROW LEVEL SECURITY;


--
-- Name: TABLE ticket_historial; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.ticket_historial IS 'APPEND-ONLY. Sin UPDATE ni DELETE. Sin updated_at/updated_by/deleted_at/deleted_by/version. Registro inmutable de todos los eventos del ciclo de vida de un Ticket. Fuente de verdad para la línea de tiempo visible al usuario y para cálculos de SLA. Un INSERT por evento: creación, estados, asignaciones, comentarios, evidencias, prioridades, áreas, SLA. El tipo_evento (ENUM) determina qué columnas son relevantes. actor_id y created_by pueden ser NULL cuando el evento fue generado por el sistema. Validación de rechazo (rejection_reason_id/rejection_comment) en Backend; trigger trg_historial_check_rechazo pendiente (migración posterior). Ref: docs/database/MODEL-PHYSICAL.md §7.18.';


--
-- Name: COLUMN ticket_historial.id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.id IS 'PK UUID. Generada automáticamente (gen_random_uuid()) en cada INSERT. Identifica unívocamente cada evento en el historial del sistema.';


--
-- Name: COLUMN ticket_historial.ticket_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.ticket_id IS 'FK activa → tickets(id) ON DELETE RESTRICT. Ticket al que pertenece este evento. RESTRICT: el historial no puede quedar huérfano; el ticket no puede eliminarse mientras tenga eventos registrados.';


--
-- Name: COLUMN ticket_historial.actor_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.actor_id IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que ejecutó la acción que generó este evento. NULL cuando el evento fue disparado por el sistema (cron job, Edge Function, cierre automático por timeout, alerta SLA). SET NULL: el historial se preserva aunque el usuario sea dado de baja.';


--
-- Name: COLUMN ticket_historial.tipo_evento; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.tipo_evento IS 'Tipo de evento registrado. ENUM tipo_evento_historial_tipo (migración 002). Valores: CREADO, ESTADO_CAMBIADO, ASIGNADO, REASIGNADO, COMENTADO, EVIDENCIA_SUBIDA, PRIORIDAD_CAMBIADA, AREA_CAMBIADA, SLA_MODIFICADO_MANUAL. Determina qué columnas adicionales son relevantes en esta fila. Soportado por idx_historial_tipo_evento (filtro por tipo en un ticket).';


--
-- Name: COLUMN ticket_historial.estado_anterior; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.estado_anterior IS 'Estado del ticket ANTES del cambio. ENUM ticket_estado_tipo (migración 002). Relevante solo cuando tipo_evento = ''ESTADO_CAMBIADO''. NULL para todos los demás tipos de evento. Junto con estado_nuevo permite reconstruir la historia de transiciones.';


--
-- Name: COLUMN ticket_historial.estado_nuevo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.estado_nuevo IS 'Estado del ticket DESPUÉS del cambio. ENUM ticket_estado_tipo (migración 002). Relevante solo cuando tipo_evento = ''ESTADO_CAMBIADO''. NULL para todos los demás tipos de evento. Cuando estado_nuevo = ''REABIERTO'', rejection_reason_id debe ser NOT NULL (validado por Backend y trigger posterior).';


--
-- Name: COLUMN ticket_historial.comentario_texto; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.comentario_texto IS 'Texto libre adjunto al evento. Su semántica depende del tipo_evento: COMENTADO → cuerpo del mensaje (obligatorio por Backend). ESTADO_CAMBIADO → justificación del cambio de estado (opcional). Otros tipos → nota adicional opcional. TEXT sin límite de longitud para no truncar comentarios en casos extremos.';


--
-- Name: COLUMN ticket_historial.rejection_reason_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.rejection_reason_id IS 'FK activa → motivos_rechazo(id) ON DELETE RESTRICT. Motivo del catálogo por el que el solicitante rechaza la resolución. Relevante solo cuando tipo_evento = ''ESTADO_CAMBIADO'' y estado_nuevo = ''REABIERTO''. NULL en todos los demás eventos. RESTRICT: el motivo de rechazo debe existir en el catálogo para preservar la interpretabilidad del historial. Cuando referencia un motivo con es_otro = true, rejection_comment es obligatorio.';


--
-- Name: COLUMN ticket_historial.rejection_comment; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.rejection_comment IS 'Explicación libre del rechazo. Obligatoria cuando rejection_reason_id apunta a un motivo con es_otro = true (validado por Backend y trigger trg_historial_check_rechazo, migración posterior). Permite al solicitante expresar el motivo exacto cuando ninguna opción del catálogo describe su situación. NULL cuando rejection_reason_id IS NULL o motivo no es ''Otro''.';


--
-- Name: COLUMN ticket_historial.metadata; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.metadata IS 'Datos adicionales estructurados según el tipo de evento. JSONB nullable. Cada tipo de evento define su propio esquema de metadata:   PRIORIDAD_CAMBIADA:    {"prioridad_anterior": "ALTA", "prioridad_nueva": "CRITICA"}   AREA_CAMBIADA:         {"area_anterior_id": "uuid", "area_nueva_id": "uuid"}   EVIDENCIA_SUBIDA:      {"nombre": "foto.jpg", "tipo": "FINAL", "url": "..."}   ASIGNADO/REASIGNADO:   {"tecnico_id": "uuid", "tecnico_nombre": "..."}   SLA_MODIFICADO_MANUAL: {"fecha_anterior": "...", "fecha_nueva": "...", "motivo": "..."} Soporta búsquedas con idx_historial_metadata_gin (GIN). Preparado para metadatos de IA (predicciones, puntuaciones) en versiones futuras.';


--
-- Name: COLUMN ticket_historial.created_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.created_at IS 'Timestamp del evento. Inmutable. Generado en INSERT (NOW()). Eje temporal central de todos los índices de historial. ASC en idx_historial_ticket_fecha para renderizar la línea de tiempo del ticket en orden cronológico (más antiguo primero).';


--
-- Name: COLUMN ticket_historial.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.ticket_historial.created_by IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que generó esta fila. Igual a actor_id en la práctica. NULL cuando el evento fue generado por el sistema (actor_id también es NULL). SET NULL: el historial se conserva aunque el usuario sea dado de baja.';


--
-- Name: tickets; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tickets (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    codigo character varying(10) DEFAULT ('PS-'::text || lpad((nextval('public.ticket_codigo_seq'::regclass))::text, 6, '0'::text)) NOT NULL,
    titulo character varying(300),
    descripcion text,
    empresa_id uuid NOT NULL,
    sucursal_id uuid NOT NULL,
    area_id uuid NOT NULL,
    tipo_servicio_id uuid NOT NULL,
    categoria_id uuid NOT NULL,
    prioridad_solicitante public.prioridad_tipo NOT NULL,
    prioridad_admin public.prioridad_tipo,
    prioridad_efectiva public.prioridad_tipo NOT NULL,
    estado public.ticket_estado_tipo DEFAULT 'NUEVO'::public.ticket_estado_tipo NOT NULL,
    solicitante_id uuid NOT NULL,
    tecnico_id uuid,
    ubicacion character varying(300),
    tiempo_estimado_min integer,
    sla_id uuid,
    fecha_limite_primera_atencion timestamp with time zone,
    fecha_limite_resolucion timestamp with time zone,
    valoracion smallint,
    motivo_cancelacion_id uuid,
    fecha_creacion timestamp with time zone DEFAULT now() NOT NULL,
    fecha_asignacion timestamp with time zone,
    fecha_inicio_proceso timestamp with time zone,
    fecha_finalizacion_tecnico timestamp with time zone,
    fecha_validacion timestamp with time zone,
    fecha_cierre timestamp with time zone,
    fecha_cancelacion timestamp with time zone,
    version integer DEFAULT 1 NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid NOT NULL,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    correos_jefe text[] DEFAULT '{}'::text[] NOT NULL,
    CONSTRAINT chk_tickets_tiempo_estimado CHECK (((tiempo_estimado_min IS NULL) OR (tiempo_estimado_min > 0))),
    CONSTRAINT chk_tickets_valoracion CHECK (((valoracion IS NULL) OR ((valoracion >= 1) AND (valoracion <= 5))))
);


--
-- Name: TABLE tickets; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.tickets IS 'Entidad focal del sistema. Registra cada incidencia, solicitud o riesgo reportado por los usuarios de la organización. Centraliza workflow, clasificación, prioridades, SLA, hitos temporales y vínculos a todos los actores del ciclo de vida del ticket. Código PS-XXXXXX generado por DEFAULT + nextval(ticket_codigo_seq). Optimistic locking via columna version (alta concurrencia). Borrado lógico via deleted_at (nunca DELETE físico). Campos inmutables post-INSERT: codigo, tipo_servicio_id, solicitante_id, sla_id (trigger guard pendiente en migración posterior). Ref: docs/database/MODEL-PHYSICAL.md §7.14.';


--
-- Name: COLUMN tickets.codigo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.codigo IS 'Código de ticket visible al usuario. Formato PS-XXXXXX (PS-000001 a PS-999999). Generado automáticamente en INSERT por DEFAULT expression: PS-'' || LPAD(nextval(''ticket_codigo_seq'')::TEXT, 6, ''0''). UNIQUE global. INMUTABLE post-INSERT. Trigger guard de inmutabilidad pendiente (migración posterior). Los gaps de secuencia (rollbacks, reinicios) son aceptables por diseño.';


--
-- Name: COLUMN tickets.titulo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.titulo IS 'Resumen breve del problema o solicitud. Máx. 300 caracteres. Visible en listados, notificaciones y encabezado del ticket. Obligatorio al crear el ticket.';


--
-- Name: COLUMN tickets.descripcion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.descripcion IS 'Descripción detallada del problema o solicitud. Sin límite de longitud (TEXT). Obligatoria. El solicitante puede incluir pasos para reproducir, contexto y urgencia.';


--
-- Name: COLUMN tickets.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.empresa_id IS 'FK activa → empresas(id) ON DELETE RESTRICT. Denormalizado para optimizar las RLS policies (evita JOIN a sucursales). Debe coincidir con sucursales.empresa_id (validado por el Backend en INSERT). Eje central de aislamiento multi-tenant en las políticas de seguridad.';


--
-- Name: COLUMN tickets.sucursal_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.sucursal_id IS 'FK activa → sucursales(id) ON DELETE RESTRICT. Sucursal donde ocurre el problema o se origina la solicitud. RESTRICT: no se puede eliminar una sucursal con tickets activos.';


--
-- Name: COLUMN tickets.area_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.area_id IS 'FK activa → areas(id) ON DELETE RESTRICT. Área funcional responsable de atender el ticket. Editable libremente en estado SIN_ASIGNAR. En otros estados requiere privilegios de Admin con auditoría en ticket_historial (Decisión Fase 7: área solo editable en SIN_ASIGNAR, excepción Admin con auditoría).';


--
-- Name: COLUMN tickets.tipo_servicio_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.tipo_servicio_id IS 'FK activa → tipos_servicio(id) ON DELETE RESTRICT. Clasificador de primer nivel del ticket (catálogo por empresa). INMUTABLE post-INSERT: el tipo de servicio no puede cambiarse después de crear el ticket. Trigger guard de inmutabilidad pendiente (migración posterior).';


--
-- Name: COLUMN tickets.categoria_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.categoria_id IS 'FK activa → categorias(id) ON DELETE RESTRICT. Clasificador de segundo nivel. NOT NULL: obligatorio al crear el ticket (Decisión Fase 7: categoría obligatoria NOT NULL). Puede ser categoría global (empresa_id IS NULL) o privada de la empresa.';


--
-- Name: COLUMN tickets.prioridad_solicitante; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.prioridad_solicitante IS 'Prioridad declarada por el solicitante al crear el ticket. ENUM prioridad_tipo (migración 002): CRITICA, ALTA, MEDIA, BAJA. Sirve como baseline; el Admin puede overridear con prioridad_admin. Se preserva aunque cambie prioridad_admin, para análisis retrospectivo.';


--
-- Name: COLUMN tickets.prioridad_admin; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.prioridad_admin IS 'Override de prioridad establecido por el Admin/SuperAdmin. NULL = sin override activo. ENUM prioridad_tipo (migración 002): CRITICA, ALTA, MEDIA, BAJA. Al establecer: prioridad_efectiva := prioridad_admin. Al anular (NULL): prioridad_efectiva := prioridad_solicitante. Cada cambio genera registro en ticket_historial con tipo PRIORIDAD_CAMBIADA.';


--
-- Name: COLUMN tickets.prioridad_efectiva; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.prioridad_efectiva IS 'Prioridad operativa del ticket: COALESCE(prioridad_admin, prioridad_solicitante). ENUM prioridad_tipo (migración 002): CRITICA, ALTA, MEDIA, BAJA. Columna física (no GENERATED) para soportar índices directos (Decisión D-015). Calculada por el Backend en INSERT y actualizada al cambiar prioridad_admin. Sin DEFAULT: el Backend siempre la provee explícitamente en cada INSERT.';


--
-- Name: COLUMN tickets.estado; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.estado IS 'Estado actual del ticket en el workflow. ENUM ticket_estado_tipo (migración 002). Estados: NUEVO (inicial) → SIN_ASIGNAR → ASIGNADO → EN_PROCESO ↔ EN_ESPERA → PENDIENTE_VALIDACION → CERRADO | REABIERTO → ASIGNADO. CANCELADO alcanzable desde cualquier estado activo (terminal). CERRADO es el otro estado terminal. Los cambios de estado se realizan exclusivamente via Backend/Edge Functions (regla de arquitectura: ningún estado se modifica directo en BD).';


--
-- Name: COLUMN tickets.solicitante_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.solicitante_id IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que creó y es propietario del ticket. INMUTABLE post-INSERT: no puede reasignarse el solicitante. Trigger guard de inmutabilidad pendiente (migración posterior). En MVP: siempre igual a created_by.';


--
-- Name: COLUMN tickets.tecnico_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.tecnico_id IS 'FK activa → usuarios(id) ON DELETE SET NULL. Técnico asignado para resolver el ticket. NULL en estados NUEVO y SIN_ASIGNAR (sin técnico asignado aún). SET NULL: si el técnico es dado de baja, el ticket queda sin técnico (el Admin debe reasignar manualmente al detectarlo).';


--
-- Name: COLUMN tickets.ubicacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.ubicacion IS 'Descripción de la ubicación física donde ocurre el problema. Ej: "Edificio A, Piso 3, Oficina 305". Opcional. Máx. 300 caracteres.';


--
-- Name: COLUMN tickets.tiempo_estimado_min; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.tiempo_estimado_min IS 'Estimación del técnico en minutos para resolver el ticket. NULL = no estimado. Entero positivo > 0 (validado por chk_tickets_tiempo_estimado). Establecido por el técnico al iniciar el proceso (estado EN_PROCESO).';


--
-- Name: COLUMN tickets.sla_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.sla_id IS 'Referencia a la configuración SLA aplicada al crear el ticket. FK DIFERIDA → sla_configuraciones(id) ON DELETE SET NULL (se agrega en M-0031). La tabla sla_configuraciones no existe en esta migración. INMUTABLE post-INSERT: el SLA calculado no se modifica retroactivamente. Trigger guard de inmutabilidad pendiente (migración posterior). NULL si la empresa no tiene SLA configurado para el tipo_servicio + prioridad del ticket.';


--
-- Name: COLUMN tickets.fecha_limite_primera_atencion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_limite_primera_atencion IS 'Fecha y hora límite para la primera atención del ticket (calculada según SLA). Considerando horario laboral y feriados de la empresa (Decisión SLA: tiempo laboral real). Calculada por el Backend al crear el ticket. INMUTABLE post-INSERT. NULL si no hay SLA aplicable (sin configuración para tipo+prioridad).';


--
-- Name: COLUMN tickets.fecha_limite_resolucion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_limite_resolucion IS 'Fecha y hora límite para la resolución completa del ticket (calculada según SLA). Considerando horario laboral y feriados de la empresa. Calculada por el Backend al crear el ticket. INMUTABLE post-INSERT. NULL si no hay SLA aplicable. Usada por idx_tickets_sla_vencidos y idx_tickets_fecha_resolucion.';


--
-- Name: COLUMN tickets.valoracion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.valoracion IS 'Puntuación de satisfacción del solicitante al cerrar el ticket. Rango: 1 (muy insatisfecho) a 5 (muy satisfecho). NULL hasta que el solicitante valora (opcional al cerrar). Validado por chk_tickets_valoracion. Insumo para reportes de calidad.';


--
-- Name: COLUMN tickets.motivo_cancelacion_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.motivo_cancelacion_id IS 'FK activa → motivos_cancelacion(id) ON DELETE RESTRICT. Obligatorio cuando el ticket pasa a estado CANCELADO; NULL en otros estados. RESTRICT: el motivo debe existir en el catálogo. Puede ser motivo global (empresa_id IS NULL) o específico de la empresa.';


--
-- Name: COLUMN tickets.fecha_creacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_creacion IS 'Timestamp de creación del ticket (hito de negocio visible al usuario). INMUTABLE post-INSERT. Diferente de created_at: fecha_creacion es la fecha de negocio que se muestra en la interfaz; created_at es el timestamp técnico de inserción en BD.';


--
-- Name: COLUMN tickets.fecha_asignacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_asignacion IS 'Timestamp en que el ticket entró en estado ASIGNADO. Establecido por el Backend al asignar un técnico. NULL hasta entonces. Usado para calcular el tiempo de primera respuesta en reportes.';


--
-- Name: COLUMN tickets.fecha_inicio_proceso; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_inicio_proceso IS 'Timestamp en que el técnico inició el proceso (estado EN_PROCESO). Establecido por el Backend al iniciar. NULL hasta entonces. Delimita el inicio del tiempo de trabajo efectivo.';


--
-- Name: COLUMN tickets.fecha_finalizacion_tecnico; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_finalizacion_tecnico IS 'Timestamp en que el técnico marcó el ticket como resuelto (PENDIENTE_VALIDACION). Establecido por el Backend al completar. NULL hasta entonces.';


--
-- Name: COLUMN tickets.fecha_validacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_validacion IS 'Timestamp en que el solicitante realizó la validación de la resolución. Se establece tanto si aprueba (→ CERRADO) como si rechaza (→ REABIERTO). NULL hasta que el solicitante actúa sobre la validación.';


--
-- Name: COLUMN tickets.fecha_cierre; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_cierre IS 'Timestamp en que el ticket entró en estado CERRADO (terminal). Establecido por el Backend al cerrar (validación aprobada). NULL hasta entonces.';


--
-- Name: COLUMN tickets.fecha_cancelacion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.fecha_cancelacion IS 'Timestamp en que el ticket entró en estado CANCELADO (terminal). Establecido por el Backend al cancelar. NULL hasta entonces.';


--
-- Name: COLUMN tickets.version; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.version IS 'Contador de versión para optimistic locking. Incrementado automáticamente en cada UPDATE por trg_fn_set_updated_at(). El Backend envía la version actual en cada operación de escritura; si difiere de la almacenada, la transacción falla (conflicto de concurrencia). Esencial en tabla con alta concurrencia (técnicos, admins, solicitantes).';


--
-- Name: COLUMN tickets.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.created_by IS 'FK activa → usuarios(id) ON DELETE RESTRICT. Usuario que creó el ticket. En MVP: siempre igual a solicitante_id. RESTRICT: un ticket no puede quedar sin usuario creador identificable.';


--
-- Name: COLUMN tickets.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.updated_by IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que realizó la última modificación (puede ser el técnico, el admin o el solicitante según la operación).';


--
-- Name: COLUMN tickets.deleted_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.deleted_at IS 'Borrado lógico. NULL = ticket activo en workflow; IS NOT NULL = borrado. Nunca se ejecuta DELETE físico (regla global del sistema). Los tickets borrados lógicamente se excluyen de dashboards y listados pero permanecen en BD para auditoría e historial.';


--
-- Name: COLUMN tickets.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tickets.deleted_by IS 'FK activa → usuarios(id) ON DELETE SET NULL. Usuario que ejecutó el borrado lógico del ticket.';


--
-- Name: tipos_servicio; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tipos_servicio (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    empresa_id uuid NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion character varying(500),
    orden integer NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_tipos_servicio_orden CHECK ((orden > 0))
);


--
-- Name: TABLE tipos_servicio; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.tipos_servicio IS 'Catálogo de tipos de servicio configurado por cada empresa. Clasificador de primer nivel del ticket (el segundo nivel es categorias). Cada empresa define sus propios tipos; no hay tipos globales compartidos. Sin columna version: catálogo de baja concurrencia (modelo congelado §7.8). Borrado lógico mediante deleted_at. Unicidades implementadas como índices parciales. Ref: docs/database/MODEL-PHYSICAL.md §7.8.';


--
-- Name: COLUMN tipos_servicio.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.empresa_id IS 'FK activa → empresas(id) ON DELETE RESTRICT. Propietaria del catálogo. Cada empresa gestiona su propio conjunto de tipos. RESTRICT: no se puede eliminar una empresa con tipos de servicio registrados.';


--
-- Name: COLUMN tipos_servicio.nombre; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.nombre IS 'Nombre del tipo de servicio visible al solicitante al crear el ticket. Único por empresa entre registros no borrados lógicamente. Unicidad garantizada por uq_tipos_servicio_empresa_nombre (índice parcial).';


--
-- Name: COLUMN tipos_servicio.descripcion; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.descripcion IS 'Descripción opcional que ayuda al solicitante a elegir el tipo correcto. Se muestra como tooltip o texto auxiliar en el formulario de nuevo ticket.';


--
-- Name: COLUMN tipos_servicio.orden; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.orden IS 'Posición del tipo en la lista de selección del formulario de nuevo ticket. Entero positivo (> 0). Único por empresa entre registros no borrados. Unicidad garantizada por uq_tipos_servicio_empresa_orden (índice parcial).';


--
-- Name: COLUMN tipos_servicio.activo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.activo IS 'Disponibilidad del tipo para nuevos tickets. false = no aparece en formularios de creación. Los tickets existentes con este tipo no se ven afectados al desactivar.';


--
-- Name: COLUMN tipos_servicio.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.created_by IS 'FK diferida → usuarios(id) ON DELETE SET NULL (se agrega en M-0031). Usuario Admin que creó el tipo de servicio. NULL si fue creado por migración.';


--
-- Name: COLUMN tipos_servicio.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.updated_by IS 'FK diferida → usuarios(id) ON DELETE SET NULL (se agrega en M-0031). Usuario que realizó la última modificación del tipo de servicio.';


--
-- Name: COLUMN tipos_servicio.deleted_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.deleted_at IS 'Borrado lógico. NULL = tipo activo o desactivado; IS NOT NULL = borrado. Nunca se ejecuta DELETE físico en esta tabla (regla global del sistema). Los índices parciales uq_tipos_servicio_* excluyen registros con deleted_at IS NOT NULL.';


--
-- Name: COLUMN tipos_servicio.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.tipos_servicio.deleted_by IS 'FK diferida → usuarios(id) ON DELETE SET NULL (se agrega en M-0031). Usuario que ejecutó el borrado lógico del tipo de servicio.';


--
-- Name: usuario_sucursales; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.usuario_sucursales (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    usuario_id uuid NOT NULL,
    sucursal_id uuid NOT NULL,
    es_principal boolean DEFAULT false NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_at timestamp with time zone,
    updated_by uuid
);


--
-- Name: usuarios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.usuarios (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    auth_id uuid NOT NULL,
    empresa_id uuid NOT NULL,
    sucursal_id uuid NOT NULL,
    area_id uuid,
    nombre character varying(100) NOT NULL,
    apellido character varying(100) NOT NULL,
    correo character varying(320) NOT NULL,
    nombre_usuario character varying(50) NOT NULL,
    telefono character varying(20),
    rol public.rol_tipo NOT NULL,
    estado_laboral public.estado_laboral_tipo DEFAULT 'ACTIVO'::public.estado_laboral_tipo NOT NULL,
    activo boolean DEFAULT true NOT NULL,
    foto_url text,
    ultimo_acceso timestamp with time zone,
    version integer DEFAULT 1 NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by uuid,
    updated_by uuid,
    deleted_at timestamp with time zone,
    deleted_by uuid,
    CONSTRAINT chk_usuarios_nombre_usuario CHECK (((nombre_usuario)::text ~ '^[a-z0-9_]{3,50}$'::text)),
    CONSTRAINT chk_usuarios_telefono CHECK (((telefono IS NULL) OR ((telefono)::text ~ '^\+[1-9]\d{7,14}$'::text)))
);


--
-- Name: TABLE usuarios; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.usuarios IS 'Perfil de aplicación de cada persona que accede a Pide Servicio. Vinculado de forma inmutable a Supabase Auth mediante auth_id (1:1). Centraliza rol, estado laboral, afiliación organizacional y datos de contacto. NOTA: sucursal_id es NOT NULL para todos los roles incluyendo SuperAdmin. El acceso global del SuperAdmin se implementa en RLS, no mediante NULL. Borrado lógico mediante deleted_at (nunca DELETE físico). Optimistic locking mediante columna version (alta concurrencia en mobile). Ref: docs/database/MODEL-PHYSICAL.md §7.4 / Decisión ARQ-001.';


--
-- Name: COLUMN usuarios.auth_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.auth_id IS 'FK activa → auth.users(id) ON DELETE CASCADE. Vínculo inmutable con Supabase Auth. No puede modificarse post-creación (protegido por trigger trg_usuarios_guard_auth_id). CASCADE: eliminar la cuenta de Auth elimina el perfil de aplicación.';


--
-- Name: COLUMN usuarios.empresa_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.empresa_id IS 'FK activa → empresas(id) ON DELETE RESTRICT. Tenant al que pertenece el usuario. Eje central de Row Level Security. RESTRICT: no se puede eliminar una empresa con usuarios activos.';


--
-- Name: COLUMN usuarios.sucursal_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.sucursal_id IS 'FK activa → sucursales(id) ON DELETE RESTRICT. Sucursal de adscripción del usuario. NOT NULL para todos los roles. El SuperAdmin usa la sucursal de su empresa de gestión; su acceso global se implementa en las RLS policies (migración posterior). RESTRICT: no se puede eliminar una sucursal con usuarios activos.';


--
-- Name: COLUMN usuarios.area_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.area_id IS 'FK activa → areas(id) ON DELETE SET NULL. Área funcional del usuario. Nullable: Admin y SuperAdmin pueden no estar adscritos a un área específica. SET NULL: al eliminar el área, el usuario queda sin área (no se elimina).';


--
-- Name: COLUMN usuarios.correo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.correo IS 'Correo electrónico del usuario. Debe coincidir con auth.users.email. UNIQUE global: no puede haber dos perfiles con el mismo correo. Máx. 320 caracteres (RFC 5321: 64 local + @ + 255 dominio).';


--
-- Name: COLUMN usuarios.nombre_usuario; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.nombre_usuario IS 'Alias de sistema único globalmente. Solo [a-z0-9_], longitud 3-50. Validado por constraint chk_usuarios_nombre_usuario. Usado en URLs de perfil y menciones en comentarios de tickets.';


--
-- Name: COLUMN usuarios.telefono; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.telefono IS 'Teléfono en formato E.164 (ej: +525512345678). NULL = no registrado. Validado por constraint chk_usuarios_telefono. Usado para notificaciones SMS y verificación de identidad adicional.';


--
-- Name: COLUMN usuarios.rol; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.rol IS 'Rol del usuario en el sistema. ENUM rol_tipo (migración 002). Valores: SUPERADMIN, ADMIN, SUPERVISOR, TECNICO, TRABAJADOR, USUARIO. El RBAC granular (permisos específicos) usa role_permissions (migración 003). No es FK a tabla roles: el ENUM es el contrato de rol en el MVP.';


--
-- Name: COLUMN usuarios.estado_laboral; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.estado_laboral IS 'Estado laboral del usuario. ENUM estado_laboral_tipo (migración 002). Valores: ACTIVO, VACACIONES, LICENCIA, SUSPENDIDO, RETIRADO. Determina disponibilidad para asignación de tickets. Solo ACTIVO aparece como candidato en la lógica de asignación.';


--
-- Name: COLUMN usuarios.activo; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.activo IS 'Cuenta habilitada/deshabilitada. false = sin acceso aunque exista en Auth. Diferente de deleted_at: activo = false bloquea el acceso sin borrado lógico. Usado para suspensiones temporales sin eliminar el perfil.';


--
-- Name: COLUMN usuarios.foto_url; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.foto_url IS 'URL del avatar en Supabase Storage (bucket avatars). NULL = el frontend usa avatar generado (iniciales o placeholder). La URL apunta a un objeto público o firmado según configuración del bucket.';


--
-- Name: COLUMN usuarios.ultimo_acceso; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.ultimo_acceso IS 'Timestamp del último inicio de sesión exitoso. Actualizado por la Edge Function de autenticación al validar el token JWT. No actualizado por este trigger; es responsabilidad del Backend.';


--
-- Name: COLUMN usuarios.version; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.version IS 'Contador de versión para optimistic locking. Incrementado automáticamente en cada UPDATE por trg_fn_set_updated_at(). El Backend debe enviar la version actual en cada escritura; si difiere, la operación falla (conflicto de concurrencia).';


--
-- Name: COLUMN usuarios.created_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.created_by IS 'FK auto-referencial → usuarios(id) ON DELETE SET NULL. Usuario que creó este perfil. NULL en el SuperAdmin inicial del sistema.';


--
-- Name: COLUMN usuarios.updated_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.updated_by IS 'FK auto-referencial → usuarios(id) ON DELETE SET NULL. Usuario que realizó la última modificación.';


--
-- Name: COLUMN usuarios.deleted_at; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.deleted_at IS 'Borrado lógico. NULL = usuario activo; IS NOT NULL = borrado. Nunca se ejecuta DELETE físico en esta tabla (regla global del sistema).';


--
-- Name: COLUMN usuarios.deleted_by; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuarios.deleted_by IS 'FK auto-referencial → usuarios(id) ON DELETE SET NULL. Usuario que ejecutó el borrado lógico.';


--
-- Name: correos_guardados correos_guardados_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.correos_guardados
    ADD CONSTRAINT correos_guardados_pkey PRIMARY KEY (id);


--
-- Name: empresa_correos_copia empresa_correos_copia_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.empresa_correos_copia
    ADD CONSTRAINT empresa_correos_copia_pkey PRIMARY KEY (id);


--
-- Name: feriados feriados_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feriados
    ADD CONSTRAINT feriados_pkey PRIMARY KEY (id);


--
-- Name: areas pk_areas; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.areas
    ADD CONSTRAINT pk_areas PRIMARY KEY (id);


--
-- Name: audit_logs pk_audit_logs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.audit_logs
    ADD CONSTRAINT pk_audit_logs PRIMARY KEY (id);


--
-- Name: categorias pk_categorias; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.categorias
    ADD CONSTRAINT pk_categorias PRIMARY KEY (id);


--
-- Name: empresas pk_empresas; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.empresas
    ADD CONSTRAINT pk_empresas PRIMARY KEY (id);


--
-- Name: motivos_cancelacion pk_motivos_cancelacion; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.motivos_cancelacion
    ADD CONSTRAINT pk_motivos_cancelacion PRIMARY KEY (id);


--
-- Name: motivos_rechazo pk_motivos_rechazo; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.motivos_rechazo
    ADD CONSTRAINT pk_motivos_rechazo PRIMARY KEY (id);


--
-- Name: notificaciones pk_notificaciones; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT pk_notificaciones PRIMARY KEY (id);


--
-- Name: parametros_sistema pk_parametros_sistema; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.parametros_sistema
    ADD CONSTRAINT pk_parametros_sistema PRIMARY KEY (id);


--
-- Name: permisos pk_permisos; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.permisos
    ADD CONSTRAINT pk_permisos PRIMARY KEY (id);


--
-- Name: preferencias_notificacion pk_preferencias_notificacion; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.preferencias_notificacion
    ADD CONSTRAINT pk_preferencias_notificacion PRIMARY KEY (id);


--
-- Name: role_permissions pk_role_permissions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.role_permissions
    ADD CONSTRAINT pk_role_permissions PRIMARY KEY (id);


--
-- Name: roles pk_roles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT pk_roles PRIMARY KEY (id);


--
-- Name: sucursales pk_sucursales; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sucursales
    ADD CONSTRAINT pk_sucursales PRIMARY KEY (id);


--
-- Name: tecnico_sucursales pk_tecnico_sucursales; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tecnico_sucursales
    ADD CONSTRAINT pk_tecnico_sucursales PRIMARY KEY (id);


--
-- Name: ticket_asignaciones pk_ticket_asignaciones; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_asignaciones
    ADD CONSTRAINT pk_ticket_asignaciones PRIMARY KEY (id);


--
-- Name: ticket_comentarios pk_ticket_comentarios; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_comentarios
    ADD CONSTRAINT pk_ticket_comentarios PRIMARY KEY (id);


--
-- Name: ticket_evidencias pk_ticket_evidencias; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_evidencias
    ADD CONSTRAINT pk_ticket_evidencias PRIMARY KEY (id);


--
-- Name: ticket_historial pk_ticket_historial; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_historial
    ADD CONSTRAINT pk_ticket_historial PRIMARY KEY (id);


--
-- Name: tickets pk_tickets; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT pk_tickets PRIMARY KEY (id);


--
-- Name: tipos_servicio pk_tipos_servicio; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_servicio
    ADD CONSTRAINT pk_tipos_servicio PRIMARY KEY (id);


--
-- Name: usuario_sucursales pk_usuario_sucursales; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_sucursales
    ADD CONSTRAINT pk_usuario_sucursales PRIMARY KEY (id);


--
-- Name: usuarios pk_usuarios; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT pk_usuarios PRIMARY KEY (id);


--
-- Name: sla_configuraciones sla_configuraciones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sla_configuraciones
    ADD CONSTRAINT sla_configuraciones_pkey PRIMARY KEY (id);


--
-- Name: correos_guardados uq_correos_guardados; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.correos_guardados
    ADD CONSTRAINT uq_correos_guardados UNIQUE (usuario_id, correo);


--
-- Name: empresa_correos_copia uq_empresa_correo; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.empresa_correos_copia
    ADD CONSTRAINT uq_empresa_correo UNIQUE (empresa_id, correo);


--
-- Name: empresas uq_empresas_fiscal; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.empresas
    ADD CONSTRAINT uq_empresas_fiscal UNIQUE (identificacion_fiscal);


--
-- Name: permisos uq_permisos_codigo; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.permisos
    ADD CONSTRAINT uq_permisos_codigo UNIQUE (codigo);


--
-- Name: preferencias_notificacion uq_preferencias_usuario; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.preferencias_notificacion
    ADD CONSTRAINT uq_preferencias_usuario UNIQUE (usuario_id);


--
-- Name: roles uq_roles_codigo; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT uq_roles_codigo UNIQUE (codigo);


--
-- Name: tecnico_sucursales uq_tecnico_sucursal; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tecnico_sucursales
    ADD CONSTRAINT uq_tecnico_sucursal UNIQUE (tecnico_id, sucursal_id);


--
-- Name: tickets uq_tickets_codigo; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT uq_tickets_codigo UNIQUE (codigo);


--
-- Name: usuario_sucursales uq_usuario_sucursales_usuario_sucursal; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_sucursales
    ADD CONSTRAINT uq_usuario_sucursales_usuario_sucursal UNIQUE (usuario_id, sucursal_id);


--
-- Name: usuarios uq_usuarios_auth_id; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT uq_usuarios_auth_id UNIQUE (auth_id);


--
-- Name: usuarios uq_usuarios_correo; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT uq_usuarios_correo UNIQUE (correo);


--
-- Name: usuarios uq_usuarios_nombre_usuario; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT uq_usuarios_nombre_usuario UNIQUE (nombre_usuario);


--
-- Name: idx_areas_activas; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_areas_activas ON public.areas USING btree (sucursal_id) WHERE ((activa = true) AND (deleted_at IS NULL));


--
-- Name: idx_areas_sucursal; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_areas_sucursal ON public.areas USING btree (sucursal_id);


--
-- Name: idx_asignaciones_reasignaciones; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_asignaciones_reasignaciones ON public.ticket_asignaciones USING btree (ticket_id) WHERE (es_reasignacion = true);


--
-- Name: idx_asignaciones_tecnico; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_asignaciones_tecnico ON public.ticket_asignaciones USING btree (tecnico_id);


--
-- Name: idx_asignaciones_tecnico_ticket; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_asignaciones_tecnico_ticket ON public.ticket_asignaciones USING btree (tecnico_id, ticket_id);


--
-- Name: idx_asignaciones_ticket; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_asignaciones_ticket ON public.ticket_asignaciones USING btree (ticket_id, created_at DESC);


--
-- Name: idx_audit_accion; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_audit_accion ON public.audit_logs USING btree (accion);


--
-- Name: idx_audit_actor; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_audit_actor ON public.audit_logs USING btree (actor_id);


--
-- Name: idx_audit_entidad; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_audit_entidad ON public.audit_logs USING btree (entidad_tipo, entidad_id);


--
-- Name: idx_audit_fecha_desc; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_audit_fecha_desc ON public.audit_logs USING btree (created_at DESC);


--
-- Name: idx_audit_modulo_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_audit_modulo_fecha ON public.audit_logs USING btree (modulo, created_at DESC);


--
-- Name: idx_audit_sucursal_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_audit_sucursal_fecha ON public.audit_logs USING btree (sucursal_id, created_at DESC);


--
-- Name: idx_categorias_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_categorias_empresa ON public.categorias USING btree (empresa_id);


--
-- Name: idx_categorias_global; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_categorias_global ON public.categorias USING btree (nombre) WHERE ((empresa_id IS NULL) AND (activa = true) AND (deleted_at IS NULL));


--
-- Name: idx_comentarios_autor; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_comentarios_autor ON public.ticket_comentarios USING btree (autor_id);


--
-- Name: idx_comentarios_internos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_comentarios_internos ON public.ticket_comentarios USING btree (ticket_id) WHERE ((es_interno = true) AND (deleted_at IS NULL));


--
-- Name: idx_comentarios_ticket; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_comentarios_ticket ON public.ticket_comentarios USING btree (ticket_id, created_at);


--
-- Name: idx_empresas_activa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_empresas_activa ON public.empresas USING btree (id) WHERE ((activa = true) AND (deleted_at IS NULL));


--
-- Name: idx_evidencias_autor; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_evidencias_autor ON public.ticket_evidencias USING btree (autor_id);


--
-- Name: idx_evidencias_final_activas; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_evidencias_final_activas ON public.ticket_evidencias USING btree (ticket_id) WHERE ((tipo = 'FINAL'::public.evidencia_tipo) AND (deleted_at IS NULL));


--
-- Name: idx_evidencias_ticket; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_evidencias_ticket ON public.ticket_evidencias USING btree (ticket_id, tipo);


--
-- Name: idx_feriados_empresa_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_feriados_empresa_fecha ON public.feriados USING btree (empresa_id, fecha) WHERE (deleted_at IS NULL);


--
-- Name: idx_feriados_pais_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_feriados_pais_fecha ON public.feriados USING btree (pais_iso, fecha) WHERE (deleted_at IS NULL);


--
-- Name: idx_historial_actor; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_historial_actor ON public.ticket_historial USING btree (actor_id);


--
-- Name: idx_historial_estado_cambiado; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_historial_estado_cambiado ON public.ticket_historial USING btree (ticket_id, created_at) WHERE (tipo_evento = 'ESTADO_CAMBIADO'::public.tipo_evento_historial_tipo);


--
-- Name: idx_historial_metadata_gin; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_historial_metadata_gin ON public.ticket_historial USING gin (metadata) WHERE (metadata IS NOT NULL);


--
-- Name: idx_historial_prioridad; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_historial_prioridad ON public.ticket_historial USING btree (ticket_id, created_at) WHERE (tipo_evento = 'PRIORIDAD_CAMBIADA'::public.tipo_evento_historial_tipo);


--
-- Name: idx_historial_ticket_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_historial_ticket_fecha ON public.ticket_historial USING btree (ticket_id, created_at);


--
-- Name: idx_historial_tipo_evento; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_historial_tipo_evento ON public.ticket_historial USING btree (ticket_id, tipo_evento);


--
-- Name: idx_motivos_cancel_activos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_motivos_cancel_activos ON public.motivos_cancelacion USING btree (empresa_id) WHERE ((activo = true) AND (deleted_at IS NULL));


--
-- Name: idx_motivos_cancel_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_motivos_cancel_empresa ON public.motivos_cancelacion USING btree (empresa_id);


--
-- Name: idx_motivos_rechazo_activos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_motivos_rechazo_activos ON public.motivos_rechazo USING btree (empresa_id, orden) WHERE ((activo = true) AND (deleted_at IS NULL));


--
-- Name: idx_motivos_rechazo_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_motivos_rechazo_empresa ON public.motivos_rechazo USING btree (empresa_id);


--
-- Name: idx_notif_destinatario_leida; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_notif_destinatario_leida ON public.notificaciones USING btree (destinatario_id, leida, created_at DESC);


--
-- Name: idx_notif_estado_entrega; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_notif_estado_entrega ON public.notificaciones USING btree (canal) WHERE (estado_entrega = 'PENDIENTE'::public.estado_entrega_tipo);


--
-- Name: idx_notif_ticket; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_notif_ticket ON public.notificaciones USING btree (ticket_id);


--
-- Name: idx_params_empresa_clave; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_params_empresa_clave ON public.parametros_sistema USING btree (empresa_id, clave);


--
-- Name: idx_params_globales; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_params_globales ON public.parametros_sistema USING btree (clave) WHERE (empresa_id IS NULL);


--
-- Name: idx_permisos_modulo; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_permisos_modulo ON public.permisos USING btree (modulo) WHERE (activo = true);


--
-- Name: idx_role_perm_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_role_perm_empresa ON public.role_permissions USING btree (empresa_id) WHERE ((empresa_id IS NOT NULL) AND (activo = true));


--
-- Name: idx_role_perm_permiso; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_role_perm_permiso ON public.role_permissions USING btree (permiso_id) WHERE (activo = true);


--
-- Name: idx_role_perm_rol; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_role_perm_rol ON public.role_permissions USING btree (rol_codigo) WHERE (activo = true);


--
-- Name: idx_sla_empresa_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_sla_empresa_id ON public.sla_configuraciones USING btree (empresa_id) WHERE (deleted_at IS NULL);


--
-- Name: idx_sla_prioridad; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_sla_prioridad ON public.sla_configuraciones USING btree (empresa_id, prioridad) WHERE ((deleted_at IS NULL) AND (activo = true));


--
-- Name: idx_sla_tipo_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_sla_tipo_id ON public.sla_configuraciones USING btree (tipo_servicio_id) WHERE ((deleted_at IS NULL) AND (tipo_servicio_id IS NOT NULL));


--
-- Name: idx_sucursales_activas; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_sucursales_activas ON public.sucursales USING btree (empresa_id) WHERE ((activa = true) AND (deleted_at IS NULL));


--
-- Name: idx_sucursales_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_sucursales_empresa ON public.sucursales USING btree (empresa_id);


--
-- Name: idx_tecnico_sucursales_activas; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tecnico_sucursales_activas ON public.tecnico_sucursales USING btree (tecnico_id, sucursal_id) WHERE (activa = true);


--
-- Name: idx_tecnico_sucursales_sucursal; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tecnico_sucursales_sucursal ON public.tecnico_sucursales USING btree (sucursal_id);


--
-- Name: idx_tecnico_sucursales_tecnico; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tecnico_sucursales_tecnico ON public.tecnico_sucursales USING btree (tecnico_id);


--
-- Name: idx_tickets_activos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_activos ON public.tickets USING btree (empresa_id) WHERE ((estado <> ALL (ARRAY['CERRADO'::public.ticket_estado_tipo, 'CANCELADO'::public.ticket_estado_tipo])) AND (deleted_at IS NULL));


--
-- Name: idx_tickets_area_estado; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_area_estado ON public.tickets USING btree (area_id, estado);


--
-- Name: idx_tickets_empresa_estado; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_empresa_estado ON public.tickets USING btree (empresa_id, estado);


--
-- Name: idx_tickets_fecha_creacion; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_fecha_creacion ON public.tickets USING btree (fecha_creacion DESC);


--
-- Name: idx_tickets_fecha_resolucion; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_fecha_resolucion ON public.tickets USING btree (fecha_limite_resolucion);


--
-- Name: idx_tickets_prioridad; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_prioridad ON public.tickets USING btree (empresa_id, prioridad_efectiva);


--
-- Name: idx_tickets_sla_vencidos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_sla_vencidos ON public.tickets USING btree (fecha_limite_resolucion) WHERE ((estado <> ALL (ARRAY['CERRADO'::public.ticket_estado_tipo, 'CANCELADO'::public.ticket_estado_tipo])) AND (fecha_limite_resolucion IS NOT NULL) AND (deleted_at IS NULL));


--
-- Name: idx_tickets_solicitante; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_solicitante ON public.tickets USING btree (solicitante_id);


--
-- Name: idx_tickets_sucursal_estado; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_sucursal_estado ON public.tickets USING btree (sucursal_id, estado);


--
-- Name: idx_tickets_tecnico_estado; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tickets_tecnico_estado ON public.tickets USING btree (tecnico_id, estado);


--
-- Name: idx_tipos_servicio_activos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tipos_servicio_activos ON public.tipos_servicio USING btree (empresa_id, orden) WHERE ((activo = true) AND (deleted_at IS NULL));


--
-- Name: idx_tipos_servicio_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_tipos_servicio_empresa ON public.tipos_servicio USING btree (empresa_id);


--
-- Name: idx_usuario_sucursales_principal; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuario_sucursales_principal ON public.usuario_sucursales USING btree (usuario_id) WHERE ((es_principal = true) AND (activo = true));


--
-- Name: idx_usuario_sucursales_sucursal_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuario_sucursales_sucursal_id ON public.usuario_sucursales USING btree (sucursal_id);


--
-- Name: idx_usuario_sucursales_usuario_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuario_sucursales_usuario_id ON public.usuario_sucursales USING btree (usuario_id);


--
-- Name: idx_usuarios_activos; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuarios_activos ON public.usuarios USING btree (empresa_id) WHERE ((activo = true) AND (deleted_at IS NULL) AND (estado_laboral = 'ACTIVO'::public.estado_laboral_tipo));


--
-- Name: idx_usuarios_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuarios_empresa ON public.usuarios USING btree (empresa_id);


--
-- Name: idx_usuarios_rol_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuarios_rol_empresa ON public.usuarios USING btree (empresa_id, rol);


--
-- Name: idx_usuarios_sucursal; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuarios_sucursal ON public.usuarios USING btree (sucursal_id);


--
-- Name: ix_correos_guardados_usuario; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_correos_guardados_usuario ON public.correos_guardados USING btree (usuario_id);


--
-- Name: ix_empresa_correos_copia_empresa; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_empresa_correos_copia_empresa ON public.empresa_correos_copia USING btree (empresa_id) WHERE (activo = true);


--
-- Name: uq_areas_sucursal_nombre; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_areas_sucursal_nombre ON public.areas USING btree (sucursal_id, nombre) WHERE (deleted_at IS NULL);


--
-- Name: uq_categorias_empresa_nombre; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_categorias_empresa_nombre ON public.categorias USING btree (COALESCE(empresa_id, '00000000-0000-0000-0000-000000000000'::uuid), nombre) WHERE (deleted_at IS NULL);


--
-- Name: uq_feriado_empresa_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_feriado_empresa_fecha ON public.feriados USING btree (empresa_id, fecha) WHERE ((deleted_at IS NULL) AND (empresa_id IS NOT NULL));


--
-- Name: uq_feriado_pais_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_feriado_pais_fecha ON public.feriados USING btree (pais_iso, fecha) WHERE ((deleted_at IS NULL) AND (pais_iso IS NOT NULL));


--
-- Name: uq_motivos_rechazo_codigo; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_motivos_rechazo_codigo ON public.motivos_rechazo USING btree (COALESCE(empresa_id, '00000000-0000-0000-0000-000000000000'::uuid), codigo) WHERE (deleted_at IS NULL);


--
-- Name: uq_motivos_rechazo_es_otro; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_motivos_rechazo_es_otro ON public.motivos_rechazo USING btree (COALESCE(empresa_id, '00000000-0000-0000-0000-000000000000'::uuid)) WHERE ((es_otro = true) AND (activo = true) AND (deleted_at IS NULL));


--
-- Name: uq_parametros_sistema_clave; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_parametros_sistema_clave ON public.parametros_sistema USING btree (COALESCE(empresa_id, '00000000-0000-0000-0000-000000000000'::uuid), clave);


--
-- Name: uq_role_perm; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_role_perm ON public.role_permissions USING btree (rol_codigo, permiso_id, COALESCE(empresa_id, '00000000-0000-0000-0000-000000000000'::uuid));


--
-- Name: uq_sla_empresa_default_prioridad; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_sla_empresa_default_prioridad ON public.sla_configuraciones USING btree (empresa_id, prioridad) WHERE ((deleted_at IS NULL) AND (tipo_servicio_id IS NULL));


--
-- Name: uq_sla_empresa_tipo_prioridad; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_sla_empresa_tipo_prioridad ON public.sla_configuraciones USING btree (empresa_id, tipo_servicio_id, prioridad) WHERE ((deleted_at IS NULL) AND (tipo_servicio_id IS NOT NULL));


--
-- Name: uq_sucursales_empresa_nombre; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_sucursales_empresa_nombre ON public.sucursales USING btree (empresa_id, nombre) WHERE (deleted_at IS NULL);


--
-- Name: uq_tipos_servicio_empresa_nombre; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_tipos_servicio_empresa_nombre ON public.tipos_servicio USING btree (empresa_id, nombre) WHERE (deleted_at IS NULL);


--
-- Name: uq_tipos_servicio_empresa_orden; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX uq_tipos_servicio_empresa_orden ON public.tipos_servicio USING btree (empresa_id, orden) WHERE (deleted_at IS NULL);


--
-- Name: areas trg_areas_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_areas_before_update_set_updated_at BEFORE UPDATE ON public.areas FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: categorias trg_categorias_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_categorias_before_update_set_updated_at BEFORE UPDATE ON public.categorias FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: empresas trg_empresas_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_empresas_before_update_set_updated_at BEFORE UPDATE ON public.empresas FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: ticket_historial trg_historial_check_rechazo; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_historial_check_rechazo BEFORE INSERT ON public.ticket_historial FOR EACH ROW EXECUTE FUNCTION public.trg_fn_historial_check_rechazo();


--
-- Name: motivos_cancelacion trg_motivos_cancelacion_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_motivos_cancelacion_before_update_set_updated_at BEFORE UPDATE ON public.motivos_cancelacion FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: motivos_rechazo trg_motivos_rechazo_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_motivos_rechazo_before_update_set_updated_at BEFORE UPDATE ON public.motivos_rechazo FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: notificaciones trg_notificaciones_before_update; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_notificaciones_before_update BEFORE UPDATE ON public.notificaciones FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: parametros_sistema trg_parametros_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_parametros_before_update_set_updated_at BEFORE UPDATE ON public.parametros_sistema FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: permisos trg_permisos_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_permisos_before_update_set_updated_at BEFORE UPDATE ON public.permisos FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: preferencias_notificacion trg_preferencias_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_preferencias_before_update_set_updated_at BEFORE UPDATE ON public.preferencias_notificacion FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: roles trg_roles_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_roles_before_update_set_updated_at BEFORE UPDATE ON public.roles FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: sla_configuraciones trg_sla_configuraciones_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_sla_configuraciones_updated_at BEFORE UPDATE ON public.sla_configuraciones FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: sucursales trg_sucursales_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_sucursales_before_update_set_updated_at BEFORE UPDATE ON public.sucursales FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: tecnico_sucursales trg_tecnico_sucursales_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tecnico_sucursales_before_update_set_updated_at BEFORE UPDATE ON public.tecnico_sucursales FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: tecnico_sucursales trg_tecnico_sucursales_guard_principal; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tecnico_sucursales_guard_principal BEFORE INSERT OR UPDATE ON public.tecnico_sucursales FOR EACH ROW EXECUTE FUNCTION public.trg_fn_tecnico_sucursales_guard_principal();


--
-- Name: tickets trg_tickets_after_insert; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tickets_after_insert AFTER INSERT ON public.tickets FOR EACH ROW EXECUTE FUNCTION public.trg_fn_tickets_after_insert();


--
-- Name: tickets trg_tickets_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tickets_before_update_set_updated_at BEFORE UPDATE ON public.tickets FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at();


--
-- Name: tickets trg_tickets_guard_inmutable; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tickets_guard_inmutable BEFORE UPDATE ON public.tickets FOR EACH ROW EXECUTE FUNCTION public.trg_fn_tickets_guard_inmutable();


--
-- Name: tickets trg_tickets_sla_calc; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tickets_sla_calc BEFORE INSERT ON public.tickets FOR EACH ROW EXECUTE FUNCTION public.trg_fn_sla_calc();


--
-- Name: tickets trg_tickets_validate_transition; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tickets_validate_transition BEFORE UPDATE OF estado ON public.tickets FOR EACH ROW EXECUTE FUNCTION public.trg_fn_tickets_validate_transition();


--
-- Name: tipos_servicio trg_tipos_servicio_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_tipos_servicio_before_update_set_updated_at BEFORE UPDATE ON public.tipos_servicio FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at_only();


--
-- Name: usuario_sucursales trg_usuario_sucursales_guard_principal; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_usuario_sucursales_guard_principal AFTER INSERT OR UPDATE OF es_principal ON public.usuario_sucursales FOR EACH ROW WHEN ((new.es_principal = true)) EXECUTE FUNCTION public.fn_usuario_sucursales_guard_principal();


--
-- Name: usuarios trg_usuarios_after_insert; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_usuarios_after_insert AFTER INSERT ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.trg_fn_usuarios_after_insert();


--
-- Name: usuarios trg_usuarios_before_update_set_updated_at; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_usuarios_before_update_set_updated_at BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.trg_fn_set_updated_at();


--
-- Name: usuarios trg_usuarios_guard_auth_id; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_usuarios_guard_auth_id BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.trg_fn_usuarios_guard_auth_id();


--
-- Name: correos_guardados correos_guardados_usuario_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.correos_guardados
    ADD CONSTRAINT correos_guardados_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id) ON DELETE CASCADE;


--
-- Name: empresa_correos_copia empresa_correos_copia_empresa_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.empresa_correos_copia
    ADD CONSTRAINT empresa_correos_copia_empresa_id_fkey FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE CASCADE;


--
-- Name: feriados feriados_created_by_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feriados
    ADD CONSTRAINT feriados_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: feriados feriados_empresa_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feriados
    ADD CONSTRAINT feriados_empresa_id_fkey FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE CASCADE;


--
-- Name: areas fk_areas_sucursales; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.areas
    ADD CONSTRAINT fk_areas_sucursales FOREIGN KEY (sucursal_id) REFERENCES public.sucursales(id) ON DELETE RESTRICT;


--
-- Name: ticket_asignaciones fk_asignaciones_asignador; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_asignaciones
    ADD CONSTRAINT fk_asignaciones_asignador FOREIGN KEY (asignador_id) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_asignaciones fk_asignaciones_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_asignaciones
    ADD CONSTRAINT fk_asignaciones_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_asignaciones fk_asignaciones_tecnico; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_asignaciones
    ADD CONSTRAINT fk_asignaciones_tecnico FOREIGN KEY (tecnico_id) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_asignaciones fk_asignaciones_tecnico_anterior; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_asignaciones
    ADD CONSTRAINT fk_asignaciones_tecnico_anterior FOREIGN KEY (tecnico_anterior_id) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: ticket_asignaciones fk_asignaciones_ticket; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_asignaciones
    ADD CONSTRAINT fk_asignaciones_ticket FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE RESTRICT;


--
-- Name: audit_logs fk_audit_actor; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.audit_logs
    ADD CONSTRAINT fk_audit_actor FOREIGN KEY (actor_id) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: audit_logs fk_audit_sucursal; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.audit_logs
    ADD CONSTRAINT fk_audit_sucursal FOREIGN KEY (sucursal_id) REFERENCES public.sucursales(id) ON DELETE SET NULL;


--
-- Name: ticket_comentarios fk_comentarios_autor; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_comentarios
    ADD CONSTRAINT fk_comentarios_autor FOREIGN KEY (autor_id) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_comentarios fk_comentarios_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_comentarios
    ADD CONSTRAINT fk_comentarios_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_comentarios fk_comentarios_deleted_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_comentarios
    ADD CONSTRAINT fk_comentarios_deleted_by FOREIGN KEY (deleted_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: ticket_comentarios fk_comentarios_ticket; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_comentarios
    ADD CONSTRAINT fk_comentarios_ticket FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE RESTRICT;


--
-- Name: ticket_comentarios fk_comentarios_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_comentarios
    ADD CONSTRAINT fk_comentarios_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: ticket_evidencias fk_evidencias_autor; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_evidencias
    ADD CONSTRAINT fk_evidencias_autor FOREIGN KEY (autor_id) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_evidencias fk_evidencias_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_evidencias
    ADD CONSTRAINT fk_evidencias_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: ticket_evidencias fk_evidencias_deleted_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_evidencias
    ADD CONSTRAINT fk_evidencias_deleted_by FOREIGN KEY (deleted_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: ticket_evidencias fk_evidencias_ticket; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_evidencias
    ADD CONSTRAINT fk_evidencias_ticket FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE RESTRICT;


--
-- Name: ticket_historial fk_historial_actor; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_historial
    ADD CONSTRAINT fk_historial_actor FOREIGN KEY (actor_id) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: ticket_historial fk_historial_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_historial
    ADD CONSTRAINT fk_historial_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: ticket_historial fk_historial_rejection_reason; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_historial
    ADD CONSTRAINT fk_historial_rejection_reason FOREIGN KEY (rejection_reason_id) REFERENCES public.motivos_rechazo(id) ON DELETE RESTRICT;


--
-- Name: ticket_historial fk_historial_ticket; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.ticket_historial
    ADD CONSTRAINT fk_historial_ticket FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE RESTRICT;


--
-- Name: notificaciones fk_notif_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT fk_notif_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: notificaciones fk_notif_destinatario; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT fk_notif_destinatario FOREIGN KEY (destinatario_id) REFERENCES public.usuarios(id) ON DELETE CASCADE;


--
-- Name: notificaciones fk_notif_ticket; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT fk_notif_ticket FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE SET NULL;


--
-- Name: notificaciones fk_notif_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT fk_notif_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: preferencias_notificacion fk_preferencias_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.preferencias_notificacion
    ADD CONSTRAINT fk_preferencias_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: preferencias_notificacion fk_preferencias_usuario; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.preferencias_notificacion
    ADD CONSTRAINT fk_preferencias_usuario FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id) ON DELETE CASCADE;


--
-- Name: role_permissions fk_role_perm_permisos; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.role_permissions
    ADD CONSTRAINT fk_role_perm_permisos FOREIGN KEY (permiso_id) REFERENCES public.permisos(id) ON DELETE CASCADE;


--
-- Name: role_permissions fk_role_perm_roles; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.role_permissions
    ADD CONSTRAINT fk_role_perm_roles FOREIGN KEY (rol_codigo) REFERENCES public.roles(codigo) ON DELETE RESTRICT;


--
-- Name: sucursales fk_sucursales_empresas; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sucursales
    ADD CONSTRAINT fk_sucursales_empresas FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE RESTRICT;


--
-- Name: tecnico_sucursales fk_tecnico_sucursales_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tecnico_sucursales
    ADD CONSTRAINT fk_tecnico_sucursales_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: tecnico_sucursales fk_tecnico_sucursales_sucursal; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tecnico_sucursales
    ADD CONSTRAINT fk_tecnico_sucursales_sucursal FOREIGN KEY (sucursal_id) REFERENCES public.sucursales(id) ON DELETE CASCADE;


--
-- Name: tecnico_sucursales fk_tecnico_sucursales_tecnico; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tecnico_sucursales
    ADD CONSTRAINT fk_tecnico_sucursales_tecnico FOREIGN KEY (tecnico_id) REFERENCES public.usuarios(id) ON DELETE CASCADE;


--
-- Name: tecnico_sucursales fk_tecnico_sucursales_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tecnico_sucursales
    ADD CONSTRAINT fk_tecnico_sucursales_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: tickets fk_tickets_area; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_area FOREIGN KEY (area_id) REFERENCES public.areas(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_categoria; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_categoria FOREIGN KEY (categoria_id) REFERENCES public.categorias(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_deleted_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_deleted_by FOREIGN KEY (deleted_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: tickets fk_tickets_empresa; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_motivo_cancelacion; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_motivo_cancelacion FOREIGN KEY (motivo_cancelacion_id) REFERENCES public.motivos_cancelacion(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_sla_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_sla_id FOREIGN KEY (sla_id) REFERENCES public.sla_configuraciones(id) ON DELETE SET NULL DEFERRABLE INITIALLY DEFERRED;


--
-- Name: tickets fk_tickets_solicitante; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_solicitante FOREIGN KEY (solicitante_id) REFERENCES public.usuarios(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_sucursal; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_sucursal FOREIGN KEY (sucursal_id) REFERENCES public.sucursales(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_tecnico; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_tecnico FOREIGN KEY (tecnico_id) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: tickets fk_tickets_tipo_servicio; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_tipo_servicio FOREIGN KEY (tipo_servicio_id) REFERENCES public.tipos_servicio(id) ON DELETE RESTRICT;


--
-- Name: tickets fk_tickets_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT fk_tickets_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: tipos_servicio fk_tipos_servicio_empresa; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_servicio
    ADD CONSTRAINT fk_tipos_servicio_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE RESTRICT;


--
-- Name: usuario_sucursales fk_usuario_sucursales_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_sucursales
    ADD CONSTRAINT fk_usuario_sucursales_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id);


--
-- Name: usuario_sucursales fk_usuario_sucursales_sucursal; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_sucursales
    ADD CONSTRAINT fk_usuario_sucursales_sucursal FOREIGN KEY (sucursal_id) REFERENCES public.sucursales(id);


--
-- Name: usuario_sucursales fk_usuario_sucursales_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_sucursales
    ADD CONSTRAINT fk_usuario_sucursales_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id);


--
-- Name: usuario_sucursales fk_usuario_sucursales_usuario; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_sucursales
    ADD CONSTRAINT fk_usuario_sucursales_usuario FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);


--
-- Name: usuarios fk_usuarios_area; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_area FOREIGN KEY (area_id) REFERENCES public.areas(id) ON DELETE SET NULL;


--
-- Name: usuarios fk_usuarios_auth; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_auth FOREIGN KEY (auth_id) REFERENCES auth.users(id) ON DELETE CASCADE;


--
-- Name: usuarios fk_usuarios_created_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_created_by FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: usuarios fk_usuarios_deleted_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_deleted_by FOREIGN KEY (deleted_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: usuarios fk_usuarios_empresa; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_empresa FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE RESTRICT;


--
-- Name: usuarios fk_usuarios_sucursal; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_sucursal FOREIGN KEY (sucursal_id) REFERENCES public.sucursales(id) ON DELETE RESTRICT;


--
-- Name: usuarios fk_usuarios_updated_by; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT fk_usuarios_updated_by FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: sla_configuraciones sla_configuraciones_created_by_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sla_configuraciones
    ADD CONSTRAINT sla_configuraciones_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: sla_configuraciones sla_configuraciones_deleted_by_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sla_configuraciones
    ADD CONSTRAINT sla_configuraciones_deleted_by_fkey FOREIGN KEY (deleted_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: sla_configuraciones sla_configuraciones_empresa_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sla_configuraciones
    ADD CONSTRAINT sla_configuraciones_empresa_id_fkey FOREIGN KEY (empresa_id) REFERENCES public.empresas(id) ON DELETE RESTRICT;


--
-- Name: sla_configuraciones sla_configuraciones_tipo_servicio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sla_configuraciones
    ADD CONSTRAINT sla_configuraciones_tipo_servicio_id_fkey FOREIGN KEY (tipo_servicio_id) REFERENCES public.tipos_servicio(id) ON DELETE SET NULL;


--
-- Name: sla_configuraciones sla_configuraciones_updated_by_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.sla_configuraciones
    ADD CONSTRAINT sla_configuraciones_updated_by_fkey FOREIGN KEY (updated_by) REFERENCES public.usuarios(id) ON DELETE SET NULL;


--
-- Name: areas; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.areas ENABLE ROW LEVEL SECURITY;

--
-- Name: audit_logs; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.audit_logs ENABLE ROW LEVEL SECURITY;

--
-- Name: categorias; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.categorias ENABLE ROW LEVEL SECURITY;

--
-- Name: correos_guardados; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.correos_guardados ENABLE ROW LEVEL SECURITY;

--
-- Name: empresa_correos_copia; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.empresa_correos_copia ENABLE ROW LEVEL SECURITY;

--
-- Name: empresas; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.empresas ENABLE ROW LEVEL SECURITY;

--
-- Name: feriados; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.feriados ENABLE ROW LEVEL SECURITY;

--
-- Name: feriados feriados_insert; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY feriados_insert ON public.feriados FOR INSERT WITH CHECK (((empresa_id = public.get_current_user_empresa_id()) AND public.is_admin_or_above()));


--
-- Name: feriados feriados_select; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY feriados_select ON public.feriados FOR SELECT USING (((deleted_at IS NULL) AND ((empresa_id = public.get_current_user_empresa_id()) OR (empresa_id IS NULL))));


--
-- Name: feriados feriados_update; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY feriados_update ON public.feriados FOR UPDATE USING (((empresa_id = public.get_current_user_empresa_id()) AND public.is_admin_or_above()));


--
-- Name: motivos_cancelacion; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.motivos_cancelacion ENABLE ROW LEVEL SECURITY;

--
-- Name: motivos_rechazo; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.motivos_rechazo ENABLE ROW LEVEL SECURITY;

--
-- Name: notificaciones; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.notificaciones ENABLE ROW LEVEL SECURITY;

--
-- Name: parametros_sistema; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.parametros_sistema ENABLE ROW LEVEL SECURITY;

--
-- Name: permisos; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.permisos ENABLE ROW LEVEL SECURITY;

--
-- Name: usuario_sucursales pol_us_select; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY pol_us_select ON public.usuario_sucursales FOR SELECT TO authenticated USING (((usuario_id = ((auth.jwt() ->> 'user_id'::text))::uuid) OR ((auth.jwt() ->> 'rol'::text) = ANY (ARRAY['ADMIN'::text, 'SUPERADMIN'::text]))));


--
-- Name: usuario_sucursales pol_us_write; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY pol_us_write ON public.usuario_sucursales TO authenticated USING (((auth.jwt() ->> 'rol'::text) = ANY (ARRAY['ADMIN'::text, 'SUPERADMIN'::text]))) WITH CHECK (((auth.jwt() ->> 'rol'::text) = ANY (ARRAY['ADMIN'::text, 'SUPERADMIN'::text])));


--
-- Name: preferencias_notificacion; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.preferencias_notificacion ENABLE ROW LEVEL SECURITY;

--
-- Name: areas rls_areas_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_areas_all_superadmin ON public.areas TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: areas rls_areas_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_areas_insert_admin ON public.areas FOR INSERT TO authenticated WITH CHECK (((( SELECT sucursales.empresa_id
   FROM public.sucursales
  WHERE (sucursales.id = areas.sucursal_id)) = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: areas rls_areas_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_areas_select_empresa ON public.areas FOR SELECT TO authenticated USING (((( SELECT sucursales.empresa_id
   FROM public.sucursales
  WHERE (sucursales.id = areas.sucursal_id)) = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text, 'TRABAJADOR'::text, 'USUARIO'::text]))));


--
-- Name: areas rls_areas_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_areas_select_tecnico ON public.areas FOR SELECT TO authenticated USING (((sucursal_id = ANY (public.get_current_user_sucursales())) AND (public.get_current_user_rol() = 'TECNICO'::text)));


--
-- Name: areas rls_areas_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_areas_update_admin ON public.areas FOR UPDATE TO authenticated USING (((( SELECT sucursales.empresa_id
   FROM public.sucursales
  WHERE (sucursales.id = areas.sucursal_id)) = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])))) WITH CHECK (((( SELECT sucursales.empresa_id
   FROM public.sucursales
  WHERE (sucursales.id = areas.sucursal_id)) = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: audit_logs rls_audit_logs_select_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_audit_logs_select_admin ON public.audit_logs FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (sucursal_id IN ( SELECT sucursales.id
   FROM public.sucursales
  WHERE ((sucursales.empresa_id = public.get_current_user_empresa_id()) AND (sucursales.deleted_at IS NULL))))));


--
-- Name: audit_logs rls_audit_logs_select_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_audit_logs_select_superadmin ON public.audit_logs FOR SELECT TO authenticated USING (public.is_superadmin());


--
-- Name: categorias rls_categorias_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_categorias_all_superadmin ON public.categorias TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: categorias rls_categorias_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_categorias_insert_admin ON public.categorias FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: categorias rls_categorias_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_categorias_select_empresa ON public.categorias FOR SELECT TO authenticated USING ((((empresa_id IS NULL) OR (empresa_id = public.get_current_user_empresa_id())) AND (NOT public.is_superadmin())));


--
-- Name: categorias rls_categorias_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_categorias_update_admin ON public.categorias FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: empresas rls_empresas_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_empresas_all_superadmin ON public.empresas TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: empresas rls_empresas_select_autenticado; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_empresas_select_autenticado ON public.empresas FOR SELECT TO authenticated USING (((id = public.get_current_user_empresa_id()) OR public.is_superadmin()));


--
-- Name: empresas rls_empresas_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_empresas_update_admin ON public.empresas FOR UPDATE TO authenticated USING (((id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])))) WITH CHECK (((id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: motivos_cancelacion rls_motivos_cancelacion_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_cancelacion_all_superadmin ON public.motivos_cancelacion TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: motivos_cancelacion rls_motivos_cancelacion_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_cancelacion_insert_admin ON public.motivos_cancelacion FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: motivos_cancelacion rls_motivos_cancelacion_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_cancelacion_select_empresa ON public.motivos_cancelacion FOR SELECT TO authenticated USING ((((empresa_id IS NULL) OR (empresa_id = public.get_current_user_empresa_id())) AND (NOT public.is_superadmin())));


--
-- Name: motivos_cancelacion rls_motivos_cancelacion_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_cancelacion_update_admin ON public.motivos_cancelacion FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: motivos_rechazo rls_motivos_rechazo_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_rechazo_all_superadmin ON public.motivos_rechazo TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: motivos_rechazo rls_motivos_rechazo_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_rechazo_insert_admin ON public.motivos_rechazo FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: motivos_rechazo rls_motivos_rechazo_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_rechazo_select_empresa ON public.motivos_rechazo FOR SELECT TO authenticated USING ((((empresa_id IS NULL) OR (empresa_id = public.get_current_user_empresa_id())) AND (NOT public.is_superadmin())));


--
-- Name: motivos_rechazo rls_motivos_rechazo_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_motivos_rechazo_update_admin ON public.motivos_rechazo FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: notificaciones rls_notificaciones_select_propio; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_notificaciones_select_propio ON public.notificaciones FOR SELECT TO authenticated USING ((destinatario_id = public.get_current_user_id()));


--
-- Name: notificaciones rls_notificaciones_select_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_notificaciones_select_superadmin ON public.notificaciones FOR SELECT TO authenticated USING (public.is_superadmin());


--
-- Name: notificaciones rls_notificaciones_update_leida; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_notificaciones_update_leida ON public.notificaciones FOR UPDATE TO authenticated USING ((destinatario_id = public.get_current_user_id())) WITH CHECK ((destinatario_id = public.get_current_user_id()));


--
-- Name: notificaciones rls_notificaciones_update_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_notificaciones_update_superadmin ON public.notificaciones FOR UPDATE TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: parametros_sistema rls_parametros_sistema_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_parametros_sistema_all_superadmin ON public.parametros_sistema TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: parametros_sistema rls_parametros_sistema_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_parametros_sistema_select_empresa ON public.parametros_sistema FOR SELECT TO authenticated USING ((((empresa_id IS NULL) OR (empresa_id = public.get_current_user_empresa_id())) AND (NOT public.is_superadmin())));


--
-- Name: parametros_sistema rls_parametros_sistema_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_parametros_sistema_update_admin ON public.parametros_sistema FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: permisos rls_permisos_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_permisos_all_superadmin ON public.permisos TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: permisos rls_permisos_select_autenticado; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_permisos_select_autenticado ON public.permisos FOR SELECT TO authenticated USING (true);


--
-- Name: preferencias_notificacion rls_preferencias_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_preferencias_all_superadmin ON public.preferencias_notificacion TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: preferencias_notificacion rls_preferencias_select_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_preferencias_select_admin ON public.preferencias_notificacion FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT usuarios.empresa_id
   FROM public.usuarios
  WHERE (usuarios.id = preferencias_notificacion.usuario_id)) = public.get_current_user_empresa_id())));


--
-- Name: preferencias_notificacion rls_preferencias_select_propio; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_preferencias_select_propio ON public.preferencias_notificacion FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TECNICO'::text, 'TRABAJADOR'::text, 'USUARIO'::text])) AND (usuario_id = public.get_current_user_id())));


--
-- Name: preferencias_notificacion rls_preferencias_update_propio; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_preferencias_update_propio ON public.preferencias_notificacion FOR UPDATE TO authenticated USING ((usuario_id = public.get_current_user_id())) WITH CHECK ((usuario_id = public.get_current_user_id()));


--
-- Name: role_permissions rls_role_permissions_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_role_permissions_all_superadmin ON public.role_permissions TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: role_permissions rls_role_permissions_delete_admin_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_role_permissions_delete_admin_empresa ON public.role_permissions FOR DELETE TO authenticated USING (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: role_permissions rls_role_permissions_insert_admin_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_role_permissions_insert_admin_empresa ON public.role_permissions FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id()) AND (empresa_id IS NOT NULL)));


--
-- Name: role_permissions rls_role_permissions_select_autenticado; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_role_permissions_select_autenticado ON public.role_permissions FOR SELECT TO authenticated USING (((empresa_id IS NULL) OR (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: role_permissions rls_role_permissions_update_admin_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_role_permissions_update_admin_empresa ON public.role_permissions FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: roles rls_roles_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_roles_all_superadmin ON public.roles TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: roles rls_roles_select_autenticado; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_roles_select_autenticado ON public.roles FOR SELECT TO authenticated USING (true);


--
-- Name: sucursales rls_sucursales_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_sucursales_all_superadmin ON public.sucursales TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: sucursales rls_sucursales_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_sucursales_insert_admin ON public.sucursales FOR INSERT TO authenticated WITH CHECK (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: sucursales rls_sucursales_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_sucursales_select_empresa ON public.sucursales FOR SELECT TO authenticated USING (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text, 'TRABAJADOR'::text, 'USUARIO'::text]))));


--
-- Name: sucursales rls_sucursales_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_sucursales_select_tecnico ON public.sucursales FOR SELECT TO authenticated USING (((id = ANY (public.get_current_user_sucursales())) AND (public.get_current_user_rol() = 'TECNICO'::text)));


--
-- Name: sucursales rls_sucursales_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_sucursales_update_admin ON public.sucursales FOR UPDATE TO authenticated USING (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])))) WITH CHECK (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: tecnico_sucursales rls_tecnico_sucursales_all_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tecnico_sucursales_all_admin ON public.tecnico_sucursales TO authenticated USING (((public.get_current_user_rol() = 'ADMIN'::text) AND (( SELECT usuarios.empresa_id
   FROM public.usuarios
  WHERE (usuarios.id = tecnico_sucursales.tecnico_id)) = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = 'ADMIN'::text) AND (( SELECT usuarios.empresa_id
   FROM public.usuarios
  WHERE (usuarios.id = tecnico_sucursales.tecnico_id)) = public.get_current_user_empresa_id())));


--
-- Name: tecnico_sucursales rls_tecnico_sucursales_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tecnico_sucursales_all_superadmin ON public.tecnico_sucursales TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: tecnico_sucursales rls_tecnico_sucursales_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tecnico_sucursales_select_empresa ON public.tecnico_sucursales FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT usuarios.empresa_id
   FROM public.usuarios
  WHERE (usuarios.id = tecnico_sucursales.tecnico_id)) = public.get_current_user_empresa_id())));


--
-- Name: tecnico_sucursales rls_tecnico_sucursales_select_propio; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tecnico_sucursales_select_propio ON public.tecnico_sucursales FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND (tecnico_id = public.get_current_user_id())));


--
-- Name: ticket_asignaciones rls_ticket_asignaciones_select_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_asignaciones_select_admin ON public.ticket_asignaciones FOR SELECT TO authenticated USING ((((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_asignaciones.ticket_id)) = public.get_current_user_empresa_id())) OR public.is_superadmin()));


--
-- Name: ticket_asignaciones rls_ticket_asignaciones_select_solicitante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_asignaciones_select_solicitante ON public.ticket_asignaciones FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (( SELECT tickets.solicitante_id
   FROM public.tickets
  WHERE (tickets.id = ticket_asignaciones.ticket_id)) = public.get_current_user_id())));


--
-- Name: ticket_asignaciones rls_ticket_asignaciones_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_asignaciones_select_tecnico ON public.ticket_asignaciones FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND (tecnico_id = public.get_current_user_id())));


--
-- Name: ticket_comentarios rls_ticket_comentarios_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_all_superadmin ON public.ticket_comentarios TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: ticket_comentarios rls_ticket_comentarios_delete_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_delete_admin ON public.ticket_comentarios FOR DELETE TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_empresa_id())));


--
-- Name: ticket_comentarios rls_ticket_comentarios_insert_participante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_insert_participante ON public.ticket_comentarios FOR INSERT TO authenticated WITH CHECK (((autor_id = public.get_current_user_id()) AND (public.is_superadmin() OR ((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_empresa_id())) OR ((public.get_current_user_rol() = 'TECNICO'::text) AND ((( SELECT tickets.tecnico_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_id()) OR (( SELECT tickets.sucursal_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = ANY (public.get_current_user_sucursales())))) OR ((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (( SELECT tickets.solicitante_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_id())))));


--
-- Name: ticket_comentarios rls_ticket_comentarios_select_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_select_admin ON public.ticket_comentarios FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_empresa_id())));


--
-- Name: ticket_comentarios rls_ticket_comentarios_select_solicitante_publico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_select_solicitante_publico ON public.ticket_comentarios FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (es_interno = false) AND (( SELECT tickets.solicitante_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_id())));


--
-- Name: ticket_comentarios rls_ticket_comentarios_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_select_tecnico ON public.ticket_comentarios FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND ((( SELECT tickets.tecnico_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = public.get_current_user_id()) OR (( SELECT tickets.sucursal_id
   FROM public.tickets
  WHERE (tickets.id = ticket_comentarios.ticket_id)) = ANY (public.get_current_user_sucursales())))));


--
-- Name: ticket_comentarios rls_ticket_comentarios_update_autor; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_comentarios_update_autor ON public.ticket_comentarios FOR UPDATE TO authenticated USING ((autor_id = public.get_current_user_id())) WITH CHECK ((autor_id = public.get_current_user_id()));


--
-- Name: ticket_evidencias rls_ticket_evidencias_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_all_superadmin ON public.ticket_evidencias TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: ticket_evidencias rls_ticket_evidencias_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_insert_admin ON public.ticket_evidencias FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (autor_id = public.get_current_user_id()) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = public.get_current_user_empresa_id())));


--
-- Name: ticket_evidencias rls_ticket_evidencias_insert_solicitante_inicial; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_insert_solicitante_inicial ON public.ticket_evidencias FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (autor_id = public.get_current_user_id()) AND (tipo = 'INICIAL'::public.evidencia_tipo) AND (( SELECT tickets.solicitante_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = public.get_current_user_id()) AND (( SELECT tickets.estado
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = ANY (ARRAY['NUEVO'::public.ticket_estado_tipo, 'SIN_ASIGNAR'::public.ticket_estado_tipo]))));


--
-- Name: ticket_evidencias rls_ticket_evidencias_insert_tecnico_final; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_insert_tecnico_final ON public.ticket_evidencias FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = 'TECNICO'::text) AND (autor_id = public.get_current_user_id()) AND (tipo = 'FINAL'::public.evidencia_tipo) AND (( SELECT tickets.tecnico_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = public.get_current_user_id())));


--
-- Name: ticket_evidencias rls_ticket_evidencias_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_select_empresa ON public.ticket_evidencias FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = public.get_current_user_empresa_id())));


--
-- Name: ticket_evidencias rls_ticket_evidencias_select_solicitante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_select_solicitante ON public.ticket_evidencias FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (( SELECT tickets.solicitante_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = public.get_current_user_id())));


--
-- Name: ticket_evidencias rls_ticket_evidencias_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_evidencias_select_tecnico ON public.ticket_evidencias FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND ((( SELECT tickets.tecnico_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = public.get_current_user_id()) OR (( SELECT tickets.sucursal_id
   FROM public.tickets
  WHERE (tickets.id = ticket_evidencias.ticket_id)) = ANY (public.get_current_user_sucursales())))));


--
-- Name: ticket_historial rls_ticket_historial_select_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_historial_select_admin ON public.ticket_historial FOR SELECT TO authenticated USING ((((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (( SELECT tickets.empresa_id
   FROM public.tickets
  WHERE (tickets.id = ticket_historial.ticket_id)) = public.get_current_user_empresa_id())) OR public.is_superadmin()));


--
-- Name: ticket_historial rls_ticket_historial_select_solicitante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_historial_select_solicitante ON public.ticket_historial FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (( SELECT tickets.solicitante_id
   FROM public.tickets
  WHERE (tickets.id = ticket_historial.ticket_id)) = public.get_current_user_id())));


--
-- Name: ticket_historial rls_ticket_historial_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_ticket_historial_select_tecnico ON public.ticket_historial FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND ((( SELECT tickets.tecnico_id
   FROM public.tickets
  WHERE (tickets.id = ticket_historial.ticket_id)) = public.get_current_user_id()) OR (( SELECT tickets.sucursal_id
   FROM public.tickets
  WHERE (tickets.id = ticket_historial.ticket_id)) = ANY (public.get_current_user_sucursales())))));


--
-- Name: tickets rls_tickets_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_all_superadmin ON public.tickets TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: tickets rls_tickets_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_insert_admin ON public.tickets FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: tickets rls_tickets_insert_solicitante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_insert_solicitante ON public.tickets FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (solicitante_id = public.get_current_user_id()) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: tickets rls_tickets_select_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_select_admin ON public.tickets FOR SELECT TO authenticated USING (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: tickets rls_tickets_select_solicitante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_select_solicitante ON public.tickets FOR SELECT TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (solicitante_id = public.get_current_user_id())));


--
-- Name: tickets rls_tickets_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_select_tecnico ON public.tickets FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND ((tecnico_id = public.get_current_user_id()) OR (sucursal_id = ANY (public.get_current_user_sucursales())))));


--
-- Name: tickets rls_tickets_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_update_admin ON public.tickets FOR UPDATE TO authenticated USING (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])))) WITH CHECK (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: tickets rls_tickets_update_solicitante; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_update_solicitante ON public.tickets FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (solicitante_id = public.get_current_user_id()))) WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['TRABAJADOR'::text, 'USUARIO'::text])) AND (solicitante_id = public.get_current_user_id())));


--
-- Name: tickets rls_tickets_update_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tickets_update_tecnico ON public.tickets FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND (tecnico_id = public.get_current_user_id()))) WITH CHECK (((public.get_current_user_rol() = 'TECNICO'::text) AND (tecnico_id = public.get_current_user_id())));


--
-- Name: tipos_servicio rls_tipos_servicio_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tipos_servicio_all_superadmin ON public.tipos_servicio TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: tipos_servicio rls_tipos_servicio_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tipos_servicio_insert_admin ON public.tipos_servicio FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: tipos_servicio rls_tipos_servicio_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tipos_servicio_select_empresa ON public.tipos_servicio FOR SELECT TO authenticated USING (((empresa_id = public.get_current_user_empresa_id()) AND (NOT public.is_superadmin())));


--
-- Name: tipos_servicio rls_tipos_servicio_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_tipos_servicio_update_admin ON public.tipos_servicio FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id()))) WITH CHECK (((public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text])) AND (empresa_id = public.get_current_user_empresa_id())));


--
-- Name: usuarios rls_usuarios_all_superadmin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_all_superadmin ON public.usuarios TO authenticated USING (public.is_superadmin()) WITH CHECK (public.is_superadmin());


--
-- Name: usuarios rls_usuarios_insert_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_insert_admin ON public.usuarios FOR INSERT TO authenticated WITH CHECK (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id()) AND ((rol)::text <> 'SUPERADMIN'::text)));


--
-- Name: usuarios rls_usuarios_select_empresa; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_select_empresa ON public.usuarios FOR SELECT TO authenticated USING (((empresa_id = public.get_current_user_empresa_id()) AND (public.get_current_user_rol() = ANY (ARRAY['ADMIN'::text, 'SUPERVISOR'::text]))));


--
-- Name: usuarios rls_usuarios_select_propio; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_select_propio ON public.usuarios FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'USUARIO'::text) AND (id = public.get_current_user_id())));


--
-- Name: usuarios rls_usuarios_select_tecnico; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_select_tecnico ON public.usuarios FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TECNICO'::text) AND ((sucursal_id = ANY (public.get_current_user_sucursales())) OR (id = public.get_current_user_id()))));


--
-- Name: usuarios rls_usuarios_select_trabajador; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_select_trabajador ON public.usuarios FOR SELECT TO authenticated USING (((public.get_current_user_rol() = 'TRABAJADOR'::text) AND (((empresa_id = public.get_current_user_empresa_id()) AND (rol = ANY (ARRAY['TECNICO'::public.rol_tipo, 'ADMIN'::public.rol_tipo, 'SUPERVISOR'::public.rol_tipo]))) OR (id = public.get_current_user_id()))));


--
-- Name: usuarios rls_usuarios_update_admin; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_update_admin ON public.usuarios FOR UPDATE TO authenticated USING (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id()) AND (id <> public.get_current_user_id()))) WITH CHECK (((public.get_current_user_rol() = 'ADMIN'::text) AND (empresa_id = public.get_current_user_empresa_id()) AND ((rol)::text <> 'SUPERADMIN'::text)));


--
-- Name: usuarios rls_usuarios_update_propio; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY rls_usuarios_update_propio ON public.usuarios FOR UPDATE TO authenticated USING ((id = public.get_current_user_id())) WITH CHECK (((id = public.get_current_user_id()) AND (empresa_id = public.get_current_user_empresa_id()) AND ((rol)::text = public.get_current_user_rol())));


--
-- Name: role_permissions; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.role_permissions ENABLE ROW LEVEL SECURITY;

--
-- Name: roles; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.roles ENABLE ROW LEVEL SECURITY;

--
-- Name: sla_configuraciones; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.sla_configuraciones ENABLE ROW LEVEL SECURITY;

--
-- Name: sla_configuraciones sla_insert; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY sla_insert ON public.sla_configuraciones FOR INSERT WITH CHECK (((empresa_id = public.get_current_user_empresa_id()) AND public.is_admin_or_above()));


--
-- Name: sla_configuraciones sla_select; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY sla_select ON public.sla_configuraciones FOR SELECT USING (((empresa_id = public.get_current_user_empresa_id()) AND (deleted_at IS NULL)));


--
-- Name: sla_configuraciones sla_update; Type: POLICY; Schema: public; Owner: -
--

CREATE POLICY sla_update ON public.sla_configuraciones FOR UPDATE USING (((empresa_id = public.get_current_user_empresa_id()) AND public.is_admin_or_above()));


--
-- Name: sucursales; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.sucursales ENABLE ROW LEVEL SECURITY;

--
-- Name: tecnico_sucursales; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.tecnico_sucursales ENABLE ROW LEVEL SECURITY;

--
-- Name: ticket_asignaciones; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.ticket_asignaciones ENABLE ROW LEVEL SECURITY;

--
-- Name: ticket_comentarios; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.ticket_comentarios ENABLE ROW LEVEL SECURITY;

--
-- Name: ticket_evidencias; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.ticket_evidencias ENABLE ROW LEVEL SECURITY;

--
-- Name: ticket_historial; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.ticket_historial ENABLE ROW LEVEL SECURITY;

--
-- Name: tickets; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.tickets ENABLE ROW LEVEL SECURITY;

--
-- Name: tipos_servicio; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.tipos_servicio ENABLE ROW LEVEL SECURITY;

--
-- Name: usuario_sucursales; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.usuario_sucursales ENABLE ROW LEVEL SECURITY;

--
-- Name: usuarios; Type: ROW SECURITY; Schema: public; Owner: -
--

ALTER TABLE public.usuarios ENABLE ROW LEVEL SECURITY;

--
-- PostgreSQL database dump complete
--

\unrestrict UuL4QAQuGQrxctrbyg06bZEaY659j858zlsbD1jPlFCJObKampPemUYD4DP6kt6

