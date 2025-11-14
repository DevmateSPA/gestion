using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class GuiaDespacho : FacturaBase<Detalle>
{
    public int Id { get; set; }
    public String Folio { get; set; } = string.Empty;
    [Nombre("Rut Cliente")]
    public string RutCliente { get; set; } = string.Empty;
    [Nombre("Orden de trabajo")]
    public string OrdenTrabajo { get; set; } = string.Empty;
    [Visible(false)]
    public string Memo { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}