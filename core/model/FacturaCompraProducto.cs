using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompraProducto : IModel
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public int Producto { get; set; }
    public int Entrada { get; set; }
    public int Salida { get; set; }
    public string Maquina { get; set; } = string.Empty;
    public string Operario { get; set; } = string.Empty;
    [Fecha]
    public DateTime Fecha { get; set; }
    [Visible(false)]
    public int Empresa { get; set; }
}