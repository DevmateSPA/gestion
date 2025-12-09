using Gestion.core.interfaces.model;

namespace Gestion.core.model.detalles;

public class Detalle : IDetalle
{
    [Visible(false)]
    public long Id { get; set; }
    [Visible(false)]
    public string Folio { get; set; } = string.Empty;
    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public long Entrada { get; set; }
    [Visible(false)]
    public long Salida { get; set; }
    [Visible(false)]
    public string Maquina { get; set; } = string.Empty;
    [Visible(false)]
    public string Operario { get; set; } = string.Empty;
    [Visible(false)]
    public DateTime Fecha { get; set; }
}