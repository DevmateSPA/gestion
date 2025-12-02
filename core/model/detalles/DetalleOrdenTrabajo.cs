using Gestion.core.interfaces.model;

namespace Gestion.core.model.detalles;

public class DetalleOrdenTrabajo : IModel
{
    public long Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string TipoPapel { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int Sobr { get; set; }
    public int Total { get; set; }
    public int Tamanio { get; set; }
    public int Cplie { get; set; }
}