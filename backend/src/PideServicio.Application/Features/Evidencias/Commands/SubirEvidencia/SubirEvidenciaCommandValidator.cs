namespace PideServicio.Application.Features.Evidencias.Commands.SubirEvidencia;

using FluentValidation;

public sealed class SubirEvidenciaCommandValidator : AbstractValidator<SubirEvidenciaCommand>
{
    // A04 / File Upload Security: lista blanca de MIME permitidos.
    // Debe coincidir con allowed_mime_types del bucket 'tickets-evidence' en Supabase (M-010).
    private static readonly HashSet<string> TiposMimePermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "video/mp4", "video/webm",
        "audio/mpeg", "audio/wav",
        "application/zip"
    };

    // Límite de 25 MB — igual al file_size_limit del bucket (M-010).
    private const long MaxTamanoBytes = 25L * 1024 * 1024;

    public SubirEvidenciaCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("El identificador del ticket es requerido.");

        RuleFor(x => x.NombreOriginal)
            .NotEmpty().WithMessage("El nombre del archivo es requerido.")
            .MaximumLength(255).WithMessage("El nombre del archivo no puede superar los 255 caracteres.");

        RuleFor(x => x.TipoMime)
            .NotEmpty().WithMessage("El tipo MIME del archivo es requerido.")
            .Must(mime => TiposMimePermitidos.Contains(mime))
            .WithMessage("Tipo de archivo no permitido.");

        RuleFor(x => x.TamanoBytes)
            .GreaterThan(0).WithMessage("El tamaño del archivo debe ser mayor que cero.")
            .LessThanOrEqualTo(MaxTamanoBytes)
            .WithMessage($"El archivo supera el límite de 25 MB.");

        RuleFor(x => x.Contenido)
            .NotNull().WithMessage("El contenido del archivo es requerido.");
    }
}
