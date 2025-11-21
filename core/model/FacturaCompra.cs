using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompra : FacturaBase
{
    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;
    [Nombre("Proveedor")]
    [Visible(false)]
    public int Debe { get; set; }
    [Visible(false)]
    public int Habe { get; set; }
    [Visible(false)]
    public string Fopa { get; set; } = string.Empty;
}