namespace PideServicio.Persistence.Helpers;

using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Permite reconstituir entidades de dominio que tienen constructor privado y setters privados,
/// eludiendo la construcción normal sin violar el encapsulamiento en la capa de dominio.
/// Solo debe usarse en la capa de Persistencia para hidratar entidades desde la base de datos.
/// </summary>
internal static class EntityReconstituter
{
    /// <summary>
    /// Crea una instancia no inicializada de T sin invocar ningún constructor.
    /// </summary>
    internal static T Crear<T>() where T : class
        => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

    /// <summary>
    /// Asigna un valor a una propiedad (con setter privado o protegido) de la instancia dada.
    /// Recorre la jerarquía de herencia para encontrar el setter declarado en el tipo exacto.
    /// </summary>
    internal static void Set<T>(T obj, string propertyName, object? value) where T : class
    {
        var type = obj.GetType();
        while (type is not null)
        {
            var prop = type.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var setter = prop?.GetSetMethod(nonPublic: true);
            if (setter is not null)
            {
                setter.Invoke(obj, [value]);
                return;
            }
            type = type.BaseType;
        }
        throw new InvalidOperationException(
            $"Propiedad '{propertyName}' no encontrada en '{typeof(T).Name}' ni en su jerarquía de herencia.");
    }
}
