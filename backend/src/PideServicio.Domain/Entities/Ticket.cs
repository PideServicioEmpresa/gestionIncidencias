namespace PideServicio.Domain.Entities;

using PideServicio.Domain.Common;
using PideServicio.Domain.Enums;
using PideServicio.Domain.Events.Tickets;
using PideServicio.Domain.Exceptions;
using PideServicio.Domain.ValueObjects;

public sealed class Ticket : AggregateRoot
{
    public TicketCodigo Codigo { get; private set; } = null!;
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public Guid EmpresaId { get; private set; }
    public Guid SucursalId { get; private set; }
    public Guid AreaId { get; private set; }
    public Guid TipoServicioId { get; private set; }
    public Guid CategoriaId { get; private set; }
    public PrioridadTipo PrioridadSolicitante { get; private set; }
    public PrioridadTipo? PrioridadAdmin { get; private set; }

    /// <summary>COALESCE(prioridad_admin, prioridad_solicitante) — se persiste en BD como columna física.</summary>
    public PrioridadTipo PrioridadEfectiva => PrioridadAdmin ?? PrioridadSolicitante;

    public TicketEstadoTipo Estado { get; private set; }
    public Guid SolicitanteId { get; private set; }
    public Guid? TecnicoId { get; private set; }
    public string? Ubicacion { get; private set; }
    public int? TiempoEstimadoMin { get; private set; }
    public Guid? SlaId { get; private set; }
    public DateTimeOffset? FechaLimitePrimeraAtencion { get; private set; }
    public DateTimeOffset? FechaLimiteResolucion { get; private set; }
    public byte? Valoracion { get; private set; }
    public Guid? MotivoCancelacionId { get; private set; }
    public DateTimeOffset FechaCreacion { get; private set; }
    public DateTimeOffset? FechaAsignacion { get; private set; }
    public DateTimeOffset? FechaInicioProceso { get; private set; }
    public DateTimeOffset? FechaFinalizacionTecnico { get; private set; }
    public DateTimeOffset? FechaValidacion { get; private set; }
    public DateTimeOffset? FechaCierre { get; private set; }
    public DateTimeOffset? FechaCancelacion { get; private set; }
    public int Version { get; private set; } = 1;

    public bool EstaEnEstadoTerminal => Estado is TicketEstadoTipo.CERRADO or TicketEstadoTipo.CANCELADO;

    private Ticket() { }

    public static Ticket Crear(
        string codigo,
        string titulo,
        string descripcion,
        Guid empresaId,
        Guid sucursalId,
        Guid areaId,
        Guid tipoServicioId,
        Guid categoriaId,
        PrioridadTipo prioridadSolicitante,
        Guid solicitanteId,
        string? ubicacion = null,
        Guid? creadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ValidationException("Titulo", "El título del ticket es requerido.");
        if (titulo.Length > 300)
            throw new ValidationException("Titulo", "El título no puede exceder 300 caracteres.");
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ValidationException("Descripcion", "La descripción del ticket es requerida.");

        var codigoVo = TicketCodigo.Crear(codigo);
        var ahora = DateTimeOffset.UtcNow;

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Codigo = codigoVo,
            Titulo = titulo.Trim(),
            Descripcion = descripcion.Trim(),
            EmpresaId = empresaId,
            SucursalId = sucursalId,
            AreaId = areaId,
            TipoServicioId = tipoServicioId,
            CategoriaId = categoriaId,
            PrioridadSolicitante = prioridadSolicitante,
            PrioridadAdmin = null,
            Estado = TicketEstadoTipo.NUEVO,
            SolicitanteId = solicitanteId,
            Ubicacion = ubicacion?.Trim(),
            FechaCreacion = ahora,
            Version = 1,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            CreatedBy = creadoPor ?? solicitanteId
        };

        ticket.AgregarEvento(new TicketCreadoEvent(
            ticket.Id, codigoVo.Valor, empresaId,
            sucursalId, areaId, solicitanteId,
            prioridadSolicitante, ahora));

        return ticket;
    }

    /// <summary>NUEVO → SIN_ASIGNAR. Se ejecuta automáticamente tras la creación válida.</summary>
    public void SubmitParaAsignacion(Guid actorId)
    {
        ValidarTransicion(TicketEstadoTipo.SIN_ASIGNAR);
        CambiarEstado(TicketEstadoTipo.SIN_ASIGNAR, actorId);
    }

    /// <summary>SIN_ASIGNAR|REABIERTO → ASIGNADO (primera asignación).</summary>
    public void Asignar(Guid tecnicoId, Guid asignadorId)
    {
        ValidarTransicion(TicketEstadoTipo.ASIGNADO);
        var ahora = DateTimeOffset.UtcNow;

        TecnicoId = tecnicoId;
        FechaAsignacion = ahora;
        CambiarEstado(TicketEstadoTipo.ASIGNADO, asignadorId, ahora);

        AgregarEvento(new TicketAsignadoEvent(
            Id, Codigo.Valor, EmpresaId,
            tecnicoId, asignadorId,
            false, null,
            ahora));
    }

    /// <summary>
    /// ASIGNADO → ASIGNADO (reasignación administrativa directa, e.g. técnico no disponible).
    /// La Application layer debe validar que el actor sea Admin/SuperAdmin.
    /// </summary>
    public void Reasignar(Guid nuevoTecnicoId, Guid asignadorId, string? motivo = null)
    {
        ValidarNoTerminal("reasignar");
        if (Estado != TicketEstadoTipo.ASIGNADO)
            throw new TransicionEstadoInvalidaException(Estado, TicketEstadoTipo.ASIGNADO);

        var ahora = DateTimeOffset.UtcNow;
        var tecnicoAnteriorId = TecnicoId;

        TecnicoId = nuevoTecnicoId;
        FechaAsignacion = ahora;
        UpdatedAt = ahora;
        UpdatedBy = asignadorId;

        AgregarEvento(new TicketAsignadoEvent(
            Id, Codigo.Valor, EmpresaId,
            nuevoTecnicoId, asignadorId,
            true, tecnicoAnteriorId,
            ahora));
    }

    /// <summary>ASIGNADO → EN_PROCESO.</summary>
    public void IniciarProceso(Guid tecnicoId)
    {
        ValidarTransicion(TicketEstadoTipo.EN_PROCESO);
        var ahora = DateTimeOffset.UtcNow;
        FechaInicioProceso ??= ahora;
        CambiarEstado(TicketEstadoTipo.EN_PROCESO, tecnicoId, ahora);
    }

    /// <summary>EN_PROCESO → EN_ESPERA.</summary>
    public void PauseParaEspera(Guid tecnicoId)
    {
        ValidarTransicion(TicketEstadoTipo.EN_ESPERA);
        CambiarEstado(TicketEstadoTipo.EN_ESPERA, tecnicoId);
    }

    /// <summary>EN_ESPERA → EN_PROCESO.</summary>
    public void ReanudarDesdeEspera(Guid tecnicoId)
    {
        ValidarTransicion(TicketEstadoTipo.EN_PROCESO);
        CambiarEstado(TicketEstadoTipo.EN_PROCESO, tecnicoId);
    }

    /// <summary>EN_PROCESO → PENDIENTE_VALIDACION.</summary>
    public void SubmitParaValidacion(Guid tecnicoId)
    {
        ValidarTransicion(TicketEstadoTipo.PENDIENTE_VALIDACION);
        var ahora = DateTimeOffset.UtcNow;
        FechaFinalizacionTecnico = ahora;
        CambiarEstado(TicketEstadoTipo.PENDIENTE_VALIDACION, tecnicoId, ahora);
    }

    /// <summary>PENDIENTE_VALIDACION → CERRADO.</summary>
    public void Cerrar(Guid actorId, byte? valoracion = null)
    {
        ValidarTransicion(TicketEstadoTipo.CERRADO);

        if (valoracion.HasValue && (valoracion.Value < 1 || valoracion.Value > 5))
            throw new ValidationException("Valoracion", "La valoración debe estar entre 1 y 5.");

        var ahora = DateTimeOffset.UtcNow;
        Valoracion = valoracion;
        FechaValidacion = ahora;
        FechaCierre = ahora;
        CambiarEstado(TicketEstadoTipo.CERRADO, actorId, ahora);

        AgregarEvento(new TicketCerradoEvent(
            Id, Codigo.Valor, EmpresaId,
            actorId, valoracion, ahora));
    }

    /// <summary>PENDIENTE_VALIDACION → REABIERTO. El motivo de rechazo es obligatorio.</summary>
    public void Reabrir(Guid solicitanteId, Guid motivoRechazoId, string? comentarioRechazo)
    {
        ValidarTransicion(TicketEstadoTipo.REABIERTO);
        var ahora = DateTimeOffset.UtcNow;

        FechaValidacion = null;
        FechaFinalizacionTecnico = null;
        TecnicoId = null;
        CambiarEstado(TicketEstadoTipo.REABIERTO, solicitanteId, ahora);

        AgregarEvento(new TicketReabiertoEvent(
            Id, Codigo.Valor, EmpresaId,
            solicitanteId, motivoRechazoId, comentarioRechazo,
            ahora));
    }

    /// <summary>Cualquier estado no-terminal → CANCELADO.</summary>
    public void Cancelar(Guid actorId, Guid motivoCancelacionId)
    {
        if (Estado == TicketEstadoTipo.CERRADO)
            throw new TicketCerradoException("cancelar");
        if (Estado == TicketEstadoTipo.CANCELADO)
            throw new TicketCanceladoException("cancelar");

        var ahora = DateTimeOffset.UtcNow;
        var estadoAnterior = Estado;
        MotivoCancelacionId = motivoCancelacionId;
        FechaCancelacion = ahora;
        CambiarEstado(TicketEstadoTipo.CANCELADO, actorId, ahora);

        AgregarEvento(new TicketCanceladoEvent(
            Id, Codigo.Valor, EmpresaId,
            actorId, motivoCancelacionId, estadoAnterior,
            ahora));
    }

    /// <summary>
    /// Sobreescribe la prioridad con la decisión del Admin/SuperAdmin.
    /// La Application layer valida que el actor tenga el rol necesario.
    /// </summary>
    public void CambiarPrioridad(PrioridadTipo nuevaPrioridad, Guid actorId)
    {
        ValidarNoTerminal("cambiar prioridad");
        var ahora = DateTimeOffset.UtcNow;
        var prioridadAnterior = PrioridadEfectiva;

        PrioridadAdmin = nuevaPrioridad;
        UpdatedAt = ahora;
        UpdatedBy = actorId;

        AgregarEvento(new TicketPrioridadCambiadaEvent(
            Id, Codigo.Valor, EmpresaId,
            prioridadAnterior, PrioridadEfectiva, actorId,
            ahora));
    }

    /// <summary>
    /// Cambia el área cuando el ticket está en SIN_ASIGNAR.
    /// Para cambio de área por Admin (cualquier estado), usar CambiarAreaAdmin.
    /// </summary>
    public void CambiarArea(Guid nuevaAreaId, Guid actorId)
    {
        ValidarNoTerminal("cambiar área");
        if (Estado != TicketEstadoTipo.SIN_ASIGNAR)
            throw new ValidationException("AreaId", "El área solo puede modificarse cuando el ticket está en estado Sin Asignar.");

        var ahora = DateTimeOffset.UtcNow;
        var areaAnteriorId = AreaId;
        AreaId = nuevaAreaId;
        UpdatedAt = ahora;
        UpdatedBy = actorId;

        AgregarEvento(new TicketAreaCambiadaEvent(
            Id, Codigo.Valor, EmpresaId,
            areaAnteriorId, nuevaAreaId, actorId,
            ahora));
    }

    /// <summary>
    /// Cambio de área con excepción Admin. La Application layer debe verificar que el actor sea Admin/SuperAdmin.
    /// </summary>
    public void CambiarAreaAdmin(Guid nuevaAreaId, Guid actorId)
    {
        ValidarNoTerminal("cambiar área");
        var ahora = DateTimeOffset.UtcNow;
        var areaAnteriorId = AreaId;

        AreaId = nuevaAreaId;
        UpdatedAt = ahora;
        UpdatedBy = actorId;

        AgregarEvento(new TicketAreaCambiadaEvent(
            Id, Codigo.Valor, EmpresaId,
            areaAnteriorId, nuevaAreaId, actorId,
            ahora));
    }

    /// <summary>
    /// Actualiza título y/o tipo de servicio. Solo Admin/SuperAdmin. Prohibido en estados terminales.
    /// </summary>
    public void ActualizarDatos(string? nuevoTitulo, Guid? nuevoTipoServicioId, Guid actorId)
    {
        ValidarNoTerminal("actualizar datos");

        if (nuevoTitulo is not null)
        {
            if (string.IsNullOrWhiteSpace(nuevoTitulo))
                throw new ValidationException("Titulo", "El título no puede estar vacío.");
            if (nuevoTitulo.Length > 300)
                throw new ValidationException("Titulo", "El título no puede exceder 300 caracteres.");
            Titulo = nuevoTitulo.Trim();
        }

        if (nuevoTipoServicioId.HasValue)
            TipoServicioId = nuevoTipoServicioId.Value;

        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    public void AsignarSla(
        Guid slaId,
        DateTimeOffset? fechaLimitePrimeraAtencion,
        DateTimeOffset? fechaLimiteResolucion,
        Guid actorId)
    {
        SlaId = slaId;
        FechaLimitePrimeraAtencion = fechaLimitePrimeraAtencion;
        FechaLimiteResolucion = fechaLimiteResolucion;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = actorId;
    }

    private void ValidarNoTerminal(string accion)
    {
        if (Estado == TicketEstadoTipo.CERRADO)
            throw new TicketCerradoException(accion);
        if (Estado == TicketEstadoTipo.CANCELADO)
            throw new TicketCanceladoException(accion);
    }

    private void ValidarTransicion(TicketEstadoTipo estadoNuevo)
    {
        var esValida = (Estado, estadoNuevo) switch
        {
            (TicketEstadoTipo.NUEVO, TicketEstadoTipo.SIN_ASIGNAR) => true,
            (TicketEstadoTipo.SIN_ASIGNAR, TicketEstadoTipo.ASIGNADO) => true,
            (TicketEstadoTipo.ASIGNADO, TicketEstadoTipo.EN_PROCESO) => true,
            (TicketEstadoTipo.EN_PROCESO, TicketEstadoTipo.EN_ESPERA) => true,
            (TicketEstadoTipo.EN_PROCESO, TicketEstadoTipo.PENDIENTE_VALIDACION) => true,
            (TicketEstadoTipo.EN_ESPERA, TicketEstadoTipo.EN_PROCESO) => true,
            (TicketEstadoTipo.PENDIENTE_VALIDACION, TicketEstadoTipo.CERRADO) => true,
            (TicketEstadoTipo.PENDIENTE_VALIDACION, TicketEstadoTipo.REABIERTO) => true,
            (TicketEstadoTipo.REABIERTO, TicketEstadoTipo.ASIGNADO) => true,
            _ => false
        };

        if (!esValida)
            throw new TransicionEstadoInvalidaException(Estado, estadoNuevo);
    }

    private void CambiarEstado(TicketEstadoTipo nuevoEstado, Guid actorId)
        => CambiarEstado(nuevoEstado, actorId, DateTimeOffset.UtcNow);

    private void CambiarEstado(TicketEstadoTipo nuevoEstado, Guid actorId, DateTimeOffset ahora)
    {
        var estadoAnterior = Estado;
        Estado = nuevoEstado;
        UpdatedAt = ahora;
        UpdatedBy = actorId;

        AgregarEvento(new TicketEstadoCambiadoEvent(
            Id, Codigo.Valor, EmpresaId,
            estadoAnterior, nuevoEstado, actorId,
            ahora));
    }
}
