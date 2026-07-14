namespace PideServicio.Persistence.Repositories;

using Dapper;
using Npgsql;
using PideServicio.Application.Common.Interfaces;
using PideServicio.Application.Common.Interfaces.Repositories;
using PideServicio.Application.Common.Models;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using PideServicio.Domain.ValueObjects;
using PideServicio.Persistence.Helpers;

/// <summary>
/// Implementación de ITicketRepository sobre PostgreSQL con Dapper.
/// Responsabilidades: CRUD del agregado Ticket, generación de código único vía secuencia,
/// listado paginado con filtros dinámicos y optimistic locking en ActualizarAsync.
/// Los ENUMs se pasan como texto y se castean en SQL para evitar configuración de tipos en el DataSource.
/// </summary>
public sealed class TicketRepository : ITicketRepository
{
    private readonly IDbConnectionFactory _db;
    private readonly ICurrentUserService _currentUser;

    public TicketRepository(IDbConnectionFactory db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // ── Shadow row ────────────────────────────────────────────────────────────
    // TotalCount: 0 cuando no está en el SELECT (queries unitarias); se rellena
    // por COUNT(*) OVER() en queries paginadas. Al ser property-based (no primary
    // constructor), Dapper solo asigna las propiedades que aparecen en el resultado.

    private sealed record TicketRow
    {
        public Guid Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Titulo { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public Guid EmpresaId { get; init; }
        public Guid SucursalId { get; init; }
        public Guid AreaId { get; init; }
        public Guid TipoServicioId { get; init; }
        public Guid CategoriaId { get; init; }
        public string PrioridadSolicitante { get; init; } = string.Empty;
        public string? PrioridadAdmin { get; init; }
        public string Estado { get; init; } = string.Empty;
        public Guid SolicitanteId { get; init; }
        public Guid? TecnicoId { get; init; }
        public string? Ubicacion { get; init; }
        public int? TiempoEstimadoMin { get; init; }
        public Guid? SlaId { get; init; }
        public DateTimeOffset? FechaLimitePrimeraAtencion { get; init; }
        public DateTimeOffset? FechaLimiteResolucion { get; init; }
        public short? Valoracion { get; init; }
        public Guid? MotivoCancelacionId { get; init; }
        public DateTimeOffset FechaCreacion { get; init; }
        public DateTimeOffset? FechaAsignacion { get; init; }
        public DateTimeOffset? FechaInicioProceso { get; init; }
        public DateTimeOffset? FechaFinalizacionTecnico { get; init; }
        public DateTimeOffset? FechaValidacion { get; init; }
        public DateTimeOffset? FechaCierre { get; init; }
        public DateTimeOffset? FechaCancelacion { get; init; }
        public int Version { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
        public Guid? CreatedBy { get; init; }
        public Guid? UpdatedBy { get; init; }
        public DateTimeOffset? DeletedAt { get; init; }
        public Guid? DeletedBy { get; init; }
        public long TotalCount { get; init; }
    }

    // Alias con comillas dobles para preservar el case exacto que espera el record.
    // Las columnas ENUM se castean a text; el mapper las convierte a enum con Enum.Parse.
    private const string SelectCols = """
        id                              AS "Id",
        codigo                          AS "Codigo",
        titulo                          AS "Titulo",
        descripcion                     AS "Descripcion",
        empresa_id                      AS "EmpresaId",
        sucursal_id                     AS "SucursalId",
        area_id                         AS "AreaId",
        tipo_servicio_id                AS "TipoServicioId",
        categoria_id                    AS "CategoriaId",
        prioridad_solicitante::text     AS "PrioridadSolicitante",
        prioridad_admin::text           AS "PrioridadAdmin",
        estado::text                    AS "Estado",
        solicitante_id                  AS "SolicitanteId",
        tecnico_id                      AS "TecnicoId",
        ubicacion                       AS "Ubicacion",
        tiempo_estimado_min             AS "TiempoEstimadoMin",
        sla_id                          AS "SlaId",
        fecha_limite_primera_atencion   AS "FechaLimitePrimeraAtencion",
        fecha_limite_resolucion         AS "FechaLimiteResolucion",
        valoracion                      AS "Valoracion",
        motivo_cancelacion_id           AS "MotivoCancelacionId",
        fecha_creacion                  AS "FechaCreacion",
        fecha_asignacion                AS "FechaAsignacion",
        fecha_inicio_proceso            AS "FechaInicioProceso",
        fecha_finalizacion_tecnico      AS "FechaFinalizacionTecnico",
        fecha_validacion                AS "FechaValidacion",
        fecha_cierre                    AS "FechaCierre",
        fecha_cancelacion               AS "FechaCancelacion",
        version                         AS "Version",
        created_at                      AS "CreatedAt",
        updated_at                      AS "UpdatedAt",
        created_by                      AS "CreatedBy",
        updated_by                      AS "UpdatedBy",
        deleted_at                      AS "DeletedAt",
        deleted_by                      AS "DeletedBy"
        """;

    // ─── Consultas ────────────────────────────────────────────────────────────

    public async Task<Ticket?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM tickets
            WHERE id = @id AND deleted_at IS NULL
            """;

        var row = await cn.QuerySingleOrDefaultAsync<TicketRow>(sql, new { id });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<Ticket?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        var sql = $"""
            SELECT {SelectCols}
            FROM tickets
            WHERE codigo = @codigo AND deleted_at IS NULL
            """;

        var row = await cn.QuerySingleOrDefaultAsync<TicketRow>(sql,
            new { codigo = codigo.Trim().ToUpperInvariant() });
        return row is null ? null : MapearEntidad(row);
    }

    public async Task<string> GenerarCodigoAsync(CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        // La secuencia ticket_codigo_seq fue creada en migration 002.
        const string sql = "SELECT 'PS-' || LPAD(nextval('ticket_codigo_seq')::TEXT, 6, '0')";
        return await cn.ExecuteScalarAsync<string>(sql)
               ?? throw new InvalidOperationException("No se pudo generar el código de ticket desde la secuencia.");
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = "SELECT EXISTS(SELECT 1 FROM tickets WHERE id = @id AND deleted_at IS NULL)";
        return await cn.ExecuteScalarAsync<bool>(sql, new { id });
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);
        const string sql = "SELECT EXISTS(SELECT 1 FROM tickets WHERE codigo = @codigo AND deleted_at IS NULL)";
        return await cn.ExecuteScalarAsync<bool>(sql,
            new { codigo = codigo.Trim().ToUpperInvariant() });
    }

    public async Task<PagedResult<Ticket>> ListarAsync(
        TicketConsultaParams filtros,
        int pagina,
        int tamanoPagina,
        CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        var conditions = new List<string> { "deleted_at IS NULL" };
        var parameters = new DynamicParameters();

        if (filtros.EmpresaId.HasValue)
        {
            conditions.Add("empresa_id = @EmpresaId");
            parameters.Add("EmpresaId", filtros.EmpresaId.Value);
        }
        if (filtros.SucursalId.HasValue)
        {
            conditions.Add("sucursal_id = @SucursalId");
            parameters.Add("SucursalId", filtros.SucursalId.Value);
        }
        if (filtros.AreaId.HasValue)
        {
            conditions.Add("area_id = @AreaId");
            parameters.Add("AreaId", filtros.AreaId.Value);
        }
        if (filtros.TecnicoId.HasValue)
        {
            conditions.Add("tecnico_id = @TecnicoId");
            parameters.Add("TecnicoId", filtros.TecnicoId.Value);
        }
        if (filtros.SolicitanteId.HasValue)
        {
            conditions.Add("solicitante_id = @SolicitanteId");
            parameters.Add("SolicitanteId", filtros.SolicitanteId.Value);
        }
        if (filtros.Estado.HasValue)
        {
            conditions.Add("estado = @Estado::ticket_estado_tipo");
            parameters.Add("Estado", filtros.Estado.Value.ToString());
        }
        if (filtros.Prioridad.HasValue)
        {
            conditions.Add("prioridad_efectiva = @Prioridad::prioridad_tipo");
            parameters.Add("Prioridad", filtros.Prioridad.Value.ToString());
        }
        if (filtros.FechaDesde.HasValue)
        {
            conditions.Add("fecha_creacion >= @FechaDesde");
            parameters.Add("FechaDesde", filtros.FechaDesde.Value);
        }
        if (filtros.FechaHasta.HasValue)
        {
            conditions.Add("fecha_creacion <= @FechaHasta");
            parameters.Add("FechaHasta", filtros.FechaHasta.Value);
        }
        if (!string.IsNullOrWhiteSpace(filtros.BusquedaTexto))
        {
            conditions.Add("(titulo ILIKE @Busqueda OR descripcion ILIKE @Busqueda OR codigo ILIKE @Busqueda)");
            parameters.Add("Busqueda", $"%{filtros.BusquedaTexto.Trim()}%");
        }

        var where = string.Join(" AND ", conditions);
        parameters.Add("Offset", (pagina - 1) * tamanoPagina);
        parameters.Add("Limit", tamanoPagina);

        var sql = $"""
            SELECT {SelectCols},
                   COUNT(*) OVER() AS "TotalCount"
            FROM tickets
            WHERE {where}
            ORDER BY fecha_creacion DESC
            LIMIT @Limit OFFSET @Offset
            """;

        var rows = (await cn.QueryAsync<TicketRow>(sql, parameters)).AsList();
        var total = rows.Count > 0 ? (int)rows[0].TotalCount : 0;

        return new PagedResult<Ticket>
        {
            Items = rows.ConvertAll(MapearEntidad),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total
        };
    }

    // ─── Escritura ────────────────────────────────────────────────────────────

    public async Task<Guid> CrearAsync(Ticket ticket, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // Los campos de SLA y tiempo_estimado_min son NULL en la creación;
        // se configuran posteriormente mediante ActualizarAsync.
        const string sql = """
            INSERT INTO tickets (
                id, codigo, titulo, descripcion,
                empresa_id, sucursal_id, area_id, tipo_servicio_id, categoria_id,
                prioridad_solicitante, prioridad_admin, prioridad_efectiva,
                estado, solicitante_id, tecnico_id, ubicacion,
                fecha_creacion, version, created_at, updated_at, created_by, updated_by)
            VALUES (
                @Id, @Codigo, @Titulo, @Descripcion,
                @EmpresaId, @SucursalId, @AreaId, @TipoServicioId, @CategoriaId,
                @PrioridadSolicitante::prioridad_tipo,
                @PrioridadAdmin::prioridad_tipo,
                @PrioridadEfectiva::prioridad_tipo,
                @Estado::ticket_estado_tipo,
                @SolicitanteId, @TecnicoId, @Ubicacion,
                @FechaCreacion, @Version, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)
            RETURNING id
            """;

        return await cn.ExecuteScalarAsync<Guid>(sql, new
        {
            ticket.Id,
            Codigo               = ticket.Codigo.Valor,
            ticket.Titulo,
            ticket.Descripcion,
            ticket.EmpresaId,
            ticket.SucursalId,
            ticket.AreaId,
            ticket.TipoServicioId,
            ticket.CategoriaId,
            PrioridadSolicitante = ticket.PrioridadSolicitante.ToString(),
            PrioridadAdmin       = ticket.PrioridadAdmin?.ToString(),
            PrioridadEfectiva    = ticket.PrioridadEfectiva.ToString(),
            Estado               = ticket.Estado.ToString(),
            ticket.SolicitanteId,
            ticket.TecnicoId,
            ticket.Ubicacion,
            ticket.FechaCreacion,
            ticket.Version,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            ticket.CreatedBy,
            ticket.UpdatedBy
        });
    }

    public async Task ActualizarAsync(Ticket ticket, CancellationToken ct = default)
    {
        await using var cn = (NpgsqlConnection)await _db.CrearConexionAsync(ct);

        // El trigger trg_fn_tickets_validate_transition llama auth.uid() para validar
        // transiciones. Las conexiones directas de Npgsql no tienen contexto de auth,
        // por lo que establecemos request.jwt.claims para que auth.uid() devuelva el
        // auth_id real del actor, permitiendo que get_current_user_rol() resuelva su rol.
        var authId = _currentUser.UsuarioActual?.AuthId;
        if (authId.HasValue && authId.Value != Guid.Empty)
        {
            var jwtClaims = $"{{\"sub\":\"{authId.Value}\",\"role\":\"authenticated\"}}";
            await cn.ExecuteAsync(
                "SELECT set_config('request.jwt.claims', @claims, false)",
                new { claims = jwtClaims });
        }

        // Optimistic locking: WHERE version = @Version.
        // Si afectadas == 0 → el ticket fue modificado concurrentemente o no existe.
        // version se incrementa en BD; el objeto en memoria conserva el valor anterior
        // (el llamador debe recargar si necesita la versión actualizada).
        const string sql = """
            UPDATE tickets SET
                titulo                        = @Titulo,
                descripcion                   = @Descripcion,
                area_id                       = @AreaId,
                prioridad_admin               = @PrioridadAdmin::prioridad_tipo,
                prioridad_efectiva            = @PrioridadEfectiva::prioridad_tipo,
                estado                        = @Estado::ticket_estado_tipo,
                tecnico_id                    = @TecnicoId,
                ubicacion                     = @Ubicacion,
                tiempo_estimado_min           = @TiempoEstimadoMin,
                sla_id                        = @SlaId,
                fecha_limite_primera_atencion = @FechaLimitePrimeraAtencion,
                fecha_limite_resolucion       = @FechaLimiteResolucion,
                valoracion                    = @Valoracion,
                motivo_cancelacion_id         = @MotivoCancelacionId,
                fecha_asignacion              = @FechaAsignacion,
                fecha_inicio_proceso          = @FechaInicioProceso,
                fecha_finalizacion_tecnico    = @FechaFinalizacionTecnico,
                fecha_validacion              = @FechaValidacion,
                fecha_cierre                  = @FechaCierre,
                fecha_cancelacion             = @FechaCancelacion,
                version                       = version + 1,
                updated_at                    = @UpdatedAt,
                updated_by                    = @UpdatedBy,
                deleted_at                    = @DeletedAt,
                deleted_by                    = @DeletedBy
            WHERE id = @Id AND version = @Version AND deleted_at IS NULL
            """;

        var afectadas = await cn.ExecuteAsync(sql, new
        {
            ticket.Id,
            ticket.Titulo,
            ticket.Descripcion,
            ticket.AreaId,
            PrioridadAdmin    = ticket.PrioridadAdmin?.ToString(),
            PrioridadEfectiva = ticket.PrioridadEfectiva.ToString(),
            Estado            = ticket.Estado.ToString(),
            ticket.TecnicoId,
            ticket.Ubicacion,
            ticket.TiempoEstimadoMin,
            ticket.SlaId,
            ticket.FechaLimitePrimeraAtencion,
            ticket.FechaLimiteResolucion,
            ticket.Valoracion,
            ticket.MotivoCancelacionId,
            ticket.FechaAsignacion,
            ticket.FechaInicioProceso,
            ticket.FechaFinalizacionTecnico,
            ticket.FechaValidacion,
            ticket.FechaCierre,
            ticket.FechaCancelacion,
            ticket.UpdatedAt,
            ticket.UpdatedBy,
            ticket.DeletedAt,
            ticket.DeletedBy,
            ticket.Version
        });

        if (afectadas == 0)
            throw new InvalidOperationException(
                $"El ticket '{ticket.Id}' no pudo actualizarse. " +
                $"Puede haber sido modificado concurrentemente (versión esperada: {ticket.Version}) " +
                $"o haber sido eliminado lógicamente.");
    }

    // ─── Mapeo ────────────────────────────────────────────────────────────────

    private static Ticket MapearEntidad(TicketRow r)
    {
        var entity = EntityReconstituter.Crear<Ticket>();

        // BaseEntity
        EntityReconstituter.Set(entity, "Id", r.Id);

        // AuditableEntity
        EntityReconstituter.Set(entity, "CreatedAt",  r.CreatedAt);
        EntityReconstituter.Set(entity, "UpdatedAt",  r.UpdatedAt);
        EntityReconstituter.Set(entity, "CreatedBy",  r.CreatedBy);
        EntityReconstituter.Set(entity, "UpdatedBy",  r.UpdatedBy);

        // SoftDeletableEntity
        EntityReconstituter.Set(entity, "DeletedAt",  r.DeletedAt);
        EntityReconstituter.Set(entity, "DeletedBy",  r.DeletedBy);

        // Ticket.Codigo — value object reconstituido con TicketCodigo.Crear (valida y normaliza)
        EntityReconstituter.Set(entity, "Codigo", TicketCodigo.Crear(r.Codigo));

        EntityReconstituter.Set(entity, "Titulo",         r.Titulo);
        EntityReconstituter.Set(entity, "Descripcion",    r.Descripcion);
        EntityReconstituter.Set(entity, "EmpresaId",      r.EmpresaId);
        EntityReconstituter.Set(entity, "SucursalId",     r.SucursalId);
        EntityReconstituter.Set(entity, "AreaId",         r.AreaId);
        EntityReconstituter.Set(entity, "TipoServicioId", r.TipoServicioId);
        EntityReconstituter.Set(entity, "CategoriaId",    r.CategoriaId);

        EntityReconstituter.Set(entity, "PrioridadSolicitante",
            Enum.Parse<PrioridadTipo>(r.PrioridadSolicitante, ignoreCase: true));
        EntityReconstituter.Set(entity, "PrioridadAdmin",
            r.PrioridadAdmin is null
                ? (PrioridadTipo?)null
                : Enum.Parse<PrioridadTipo>(r.PrioridadAdmin, ignoreCase: true));

        // PrioridadEfectiva es computed (PrioridadAdmin ?? PrioridadSolicitante); no requiere setter.

        EntityReconstituter.Set(entity, "Estado",
            Enum.Parse<TicketEstadoTipo>(r.Estado, ignoreCase: true));

        EntityReconstituter.Set(entity, "SolicitanteId",              r.SolicitanteId);
        EntityReconstituter.Set(entity, "TecnicoId",                  r.TecnicoId);
        EntityReconstituter.Set(entity, "Ubicacion",                  r.Ubicacion);
        EntityReconstituter.Set(entity, "TiempoEstimadoMin",          r.TiempoEstimadoMin);
        EntityReconstituter.Set(entity, "SlaId",                      r.SlaId);
        EntityReconstituter.Set(entity, "FechaLimitePrimeraAtencion", r.FechaLimitePrimeraAtencion);
        EntityReconstituter.Set(entity, "FechaLimiteResolucion",      r.FechaLimiteResolucion);

        // Valoracion: smallint en BD → short? en Dapper → byte? en dominio (rango 1-5 garantizado)
        EntityReconstituter.Set(entity, "Valoracion",
            r.Valoracion.HasValue ? (byte?)checked((byte)r.Valoracion.Value) : null);

        EntityReconstituter.Set(entity, "MotivoCancelacionId",      r.MotivoCancelacionId);
        EntityReconstituter.Set(entity, "FechaCreacion",            r.FechaCreacion);
        EntityReconstituter.Set(entity, "FechaAsignacion",          r.FechaAsignacion);
        EntityReconstituter.Set(entity, "FechaInicioProceso",       r.FechaInicioProceso);
        EntityReconstituter.Set(entity, "FechaFinalizacionTecnico", r.FechaFinalizacionTecnico);
        EntityReconstituter.Set(entity, "FechaValidacion",          r.FechaValidacion);
        EntityReconstituter.Set(entity, "FechaCierre",              r.FechaCierre);
        EntityReconstituter.Set(entity, "FechaCancelacion",         r.FechaCancelacion);
        EntityReconstituter.Set(entity, "Version",                  r.Version);

        return entity;
    }
}
