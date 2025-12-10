using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompraProducto : IDetalle
{
    [Visible(false)]
    public long Id { get; set; } = 0;
    [Required]
    public string Folio { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Productonombre { get; set; } = string.Empty;
    public long Entrada { get; set; }
    [Visible(false)]
    public long Salida { get; set; }
    public string Maquina { get; set; } = string.Empty;
    public string Operario { get; set; } = string.Empty;
    [Fecha]
    [Visible(false)]
    public DateTime Fecha { get; set; } = DateTime.Now;
}