using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class NotaCredito : IEmpresa
{
    public long Id { get; set; }
    [Required]
    public string Folio { get; set; } = string.Empty;
    [Nombre("Rut Cliente")]
    [Rut]
    public string RutCliente { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    public int Neto { get; set; }
    public int Iva { get; set; }
    public int Total { get; set; }
     [Nombre("Detalle")]
    public string Memo { get; set; } = string.Empty;
    [Fecha]
    public DateTime Fecha { get; set; }
    [Visible(false)]
    public long Empresa { get; set; }
}