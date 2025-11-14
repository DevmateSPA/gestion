using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Detalle : IModel
{
    public int Id { get; set; }
    [Visible(false)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;
    [Orden(1)]
    public string Producto { get; set; } = string.Empty;
    [Orden(2)]
    public int Cantidad { get; set; }
    [Orden(3)]
    public int Precio { get; set; }
    [Orden(4)]
    public int Total { get; set; }
    public DateTime Fecha { get; set; }
}
