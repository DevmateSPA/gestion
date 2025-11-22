using Gestion.core.interfaces.model;

namespace Gestion.core.model.detalles;

public class Detalle : IDetalle
{
    public long Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public long Entrada { get; set; }
    public long Salida { get; set; }
    public string Maquina { get; set; } = string.Empty;
    public string Operario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}