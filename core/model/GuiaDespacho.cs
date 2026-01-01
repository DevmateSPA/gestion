using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class GuiaDespacho : IEmpresa
{
    public long Id { get; set; }

    [Required]
    [Nombre("Folio")]
    [Grupo("Datos del Documento", 1)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("Datos del Documento", 1)]
    [Orden(1)]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Nombre("Orden de Trabajo")]
    [Grupo("Datos Internos", 2)]
    [Orden(0)]
    public string OrdenTrabajo { get; set; } = string.Empty;

    [Nombre("Factura")]
    [Grupo("Datos Internos", 2)]
    [Orden(1)]
    public string Factura { get; set; } = string.Empty;

    [Rut]
    [Nombre("Rut")]
    [Grupo("Datos del Cliente", 2)]
    [Orden(0)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Nombre")]
    [Grupo("Datos del Cliente", 2)]
    [Orden(1)]
    [NoSaveDb]
    [OnlyRead]
    public string NombreCliente { get; set; } = string.Empty;

    [Nombre("Detalles")]
    public string Memo { get; set; } = string.Empty;

    [Visible(false)]
    public long Empresa { get; set; }
}