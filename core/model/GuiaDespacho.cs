using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class GuiaDespacho : FacturaBase
{
    [Nombre("Detalle")]
    public string Memo { get; set; }  = string.Empty;
    [Nombre("Orden de trabajo")]
    public string OrdenTrabajo { get; set; } = string.Empty;
    [Visible(false)]
    public string Factura { get; set; } = string.Empty;

}