using Gestion.core.interfaces.model;

namespace Gestion.core.model.detalles;

public class DetalleOrdenTrabajo : IModel, IEquatable<DetalleOrdenTrabajo>
{
    public long Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string TipoPapel { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int Sobr { get; set; }
    public long Total { get; set; }
    public int Tamanio { get; set; }
    public int Cplie { get; set; }
    public long Empresa { get; set; }

    // Comparación de contenido (NO incluye Id)
    public bool Equals(DetalleOrdenTrabajo? other)
    {
        if (other is null) return false;

        return Folio == other.Folio
            && Empresa == other.Empresa
            && TipoPapel == other.TipoPapel
            && Cantidad == other.Cantidad
            && Sobr == other.Sobr
            && Total == other.Total
            && Tamanio == other.Tamanio
            && Cplie == other.Cplie;
    }

    public override bool Equals(object? obj)
        => Equals(obj as DetalleOrdenTrabajo);

    public override int GetHashCode()
        => HashCode.Combine(
            Folio,
            Empresa,
            TipoPapel,
            Cantidad,
            Sobr,
            Total,
            Tamanio,
            Cplie
        );

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