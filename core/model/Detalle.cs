using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Detalle : IModel
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int Precio { get; set; }
    public int Total { get; set; }
    public DateTime Fecha { get; set; }
}
