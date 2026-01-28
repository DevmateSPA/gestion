using Gestion.core.model.detalles;

namespace Gestion.core.model;

public class FacturaCompraProducto : Detalle, IEquatable<FacturaCompraProducto>
{
    public string Productonombre { get; set; } = string.Empty;

    // Comparación por contenido (NO incluye Id)
    public bool Equals(FacturaCompraProducto? other)
    {
        if (other is null)
            return false;

        return Folio == other.Folio
            && Empresa == other.Empresa
            && Tipo == other.Tipo
            && Producto == other.Producto
            && Entrada == other.Entrada
            && Salida == other.Salida
            && Maquina == other.Maquina
            && Operario == other.Operario
            && Fecha == other.Fecha
            && Productonombre == other.Productonombre;
    }

    public override bool Equals(object? obj)
        => Equals(obj as FacturaCompraProducto);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Folio);
        hash.Add(Empresa);
        hash.Add(Tipo);
        hash.Add(Producto);
        hash.Add(Entrada);
        hash.Add(Salida);
        hash.Add(Maquina);
        hash.Add(Operario);
        hash.Add(Fecha);
        hash.Add(Productonombre);
        return hash.ToHashCode();
    }

    public override Detalle Clone()
    {
        return new FacturaCompraProducto
        {
            Id = Id,
            Folio = Folio,
            Empresa = Empresa,
            Tipo = Tipo,
            Producto = Producto,
            Entrada = Entrada,
            Salida = Salida,
            Maquina = Maquina,
            Operario = Operario,
            Fecha = Fecha,
            Productonombre = Productonombre
        };
    }
}