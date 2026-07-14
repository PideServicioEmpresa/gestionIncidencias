namespace PideServicio.Application.Common.Mappings;

using Mapster;
using System.Reflection;

public static class MappingConfig
{
    public static void Configurar(TypeAdapterConfig config)
    {
        // Registra automáticamente todos los IRegister del ensamblado.
        // Cada módulo en Features/ expone su propio IRegister (MappingProfile).
        config.Scan(Assembly.GetExecutingAssembly());
    }
}
