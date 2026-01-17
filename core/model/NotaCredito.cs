using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;

namespace Gestion.core.model;

public class NotaCredito : TotalBase
{
    [Required]
    [Nombre("Folio")]
    [Grupo("Datos del Documento", 1)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("Datos del Documento", 1)]
    [Orden(3)]
    public DateTime Fecha { get; set; }

    [Nombre("Factura")]
    [Grupo("Datos del Documento", 1)]
    [Orden(1)]
    [Searchable("FOLIO_FACTURA")]
    public string Factura { get; set; } = string.Empty;

    [Nombre("Rut")]
    [Rut]
    [Grupo("Cliente", 1)]
    [Orden(2)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Nombre")]
    [Grupo("Cliente", 1)]
    [Orden(2)]
    [NoSaveDb]
    [OnlyRead]
    public string NombreCliente { get; set; } = string.Empty;

    [Nombre("Detalle")]
    [Grupo("Detalles", 2)]
    [TextArea]
    public string Memo { get; set; } = string.Empty;
}