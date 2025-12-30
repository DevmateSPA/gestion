using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class GuiaDespacho : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS DEL DOCUMENTO
    // ===============================

    [Required]
    [Nombre("Folio")]
    [Grupo("Datos del Documento", 1)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Rut]
    [Nombre("Rut Cliente")]
    [Grupo("Datos del Documento", 1)]
    [Orden(1)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Orden de Trabajo")]
    [Grupo("Datos del Documento", 1)]
    [Orden(2)]
    public string OrdenTrabajo { get; set; } = string.Empty;

    [Nombre("Factura")]
    [Grupo("Datos del Documento", 1)]
    [Orden(3)]
    public string Factura { get; set; } = string.Empty;

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("Datos del Documento", 1)]
    [Orden(4)]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Nombre("Observaciones")]
    [Grupo("Datos del Documento", 1)]
    [Orden(5)]
    public string Memo { get; set; } = string.Empty;


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}