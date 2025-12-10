using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class GuiaDespacho : IEmpresa
{
    public long Id { get; set; }
    [Required]
    public string Folio { get; set; } = string.Empty;
    [Rut]
    public string RutCliente { get; set; } = string.Empty;
    public string OrdenTrabajo { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    [Fecha]
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Memo { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}