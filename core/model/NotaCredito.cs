using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class NotaCredito : IEmpresa
{
    public long Id { get; set; }

    [Required]
    [Nombre("Folio")]
    [Grupo("General", 1)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Rut Cliente")]
    [Rut]
    [Grupo("General", 1)]
    [Orden(2)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Factura")]
    [Grupo("General", 1)]
    [Orden(1)]
    public string Factura { get; set; } = string.Empty;

    [Nombre("Neto")]
    [Grupo("Totales", 2)]
    [Orden(0)]
    public int Neto { get; set; }

    [Nombre("I.V.A")]
    [Grupo("Totales", 2)]
    [Orden(1)]
    public int Iva { get; set; }

    [Nombre("Total")]
    [Grupo("Totales", 2)]
    [Orden(2)]
    public int Total { get; set; }


    [Nombre("Detalle")]
    public string Memo { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("General", 1)]
    [Orden(3)]
    public DateTime Fecha { get; set; }

    [Visible(false)]
    public long Empresa { get; set; }
}