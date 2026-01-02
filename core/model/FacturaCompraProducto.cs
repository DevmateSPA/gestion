using Gestion.core.model.detalles;

namespace Gestion.core.model;

public class FacturaCompraProducto : Detalle
{
    public string Productonombre { get; set; } = string.Empty;
    public override Detalle Clone()
    {
        return new FacturaCompraProducto
        {
            Id = Id,
            Folio = Folio,
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