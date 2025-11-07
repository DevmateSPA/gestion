using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class GuiaDespacho : IModel
{
    public int Id { get; set; }
    public int folio { get; set; }
    public string RutCliente { get; set; } = string.Empty;
    public string OrdenCompra { get; set; } = string.Empty;
    public string Memo { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}