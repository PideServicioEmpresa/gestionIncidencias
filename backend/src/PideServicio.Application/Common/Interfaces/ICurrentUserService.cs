namespace PideServicio.Application.Common.Interfaces;

using PideServicio.Application.Common.Models;

public interface ICurrentUserService
{
    CurrentUser? UsuarioActual { get; }
    bool EstaAutenticado { get; }
}
