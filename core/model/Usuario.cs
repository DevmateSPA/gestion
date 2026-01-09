using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Usuario : IEmpresa
{
    public long Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}