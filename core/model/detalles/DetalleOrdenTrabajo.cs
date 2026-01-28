using Gestion.core.interfaces.model;

namespace Gestion.core.model.detalles;

public class DetalleOrdenTrabajo : IModel
{
    public long Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string TipoPapel { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int Sobr { get; set; }
    public long Total { get; set; }
    public int Tamanio { get; set; }
    public int Cplie { get; set; }
    public long Empresa { get; set; } = 0;

    public DetalleOrdenTrabajo Clone()
    {
        return new DetalleOrdenTrabajo
        {
            Id = Id,
            Folio = Folio,
            TipoPapel = TipoPapel,
            Cantidad = Cantidad,
            Sobr = Sobr,
            Total = Total,
            Tamanio = Tamanio,
            Cplie = Cplie,
            Empresa = Empresa
        };
    }
}