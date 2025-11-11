using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompra : IModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    [Nombre("Proveedor")]
    public string RutCliente { get; set; } = string.Empty;
    public int Neto { get; set; }
    public int Iva { get; set; }
    [Visible(false)]
    public int Deber { get; set; }
    [Visible(false)]
    public int Habe { get; set; }
    [Visible(false)]
    public string Fopa { get; set; } = string.Empty;
}