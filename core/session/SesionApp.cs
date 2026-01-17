namespace Gestion.core.session;

public static class SesionApp
{
    public static long IdUsuario { get; set; }
    public static long IdEmpresa { get; set; }
    public static string NombreEmpresa { get; set; } = string.Empty;
    public static string NombreUsuario { get; set; } = string.Empty;
    public static string TipoUsuario { get; set; } = string.Empty;
}