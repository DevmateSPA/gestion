using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Usuario : IEmpresa
{
    List<String> USUARIOS = ["admin","usuario"];
    public long Id { get; set; }
    [Orden(0)]
    public string Nombre { get; set; } = string.Empty;
    [Orden(1)]
    public string Clave { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
    [Orden(2)]
    [ComboSource("TIPOS", Display = "Nombre", Value = "Nombre")]
    public string Tipo { get; set; }
}