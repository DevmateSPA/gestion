using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class NotaCredito : IEmpresa
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

    [Nombre("Rut Cliente")]
    [Rut]
    [Grupo("Datos del Documento", 1)]
    [Orden(1)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Factura")]
    [Grupo("Datos del Documento", 1)]
    [Orden(2)]
    public string Factura { get; set; } = string.Empty;


    // ===============================
    // MONTOS
    // ===============================

    [Nombre("Neto")]
    [Grupo("Totales", 2)]
    [Orden(0)]
    public int Neto { get; set; }

    [Nombre("IVA")]
    [Grupo("Totales", 2)]
    [Orden(1)]
    public int Iva { get; set; }

    [Nombre("Total")]
    [Grupo("Totales", 2)]
    [Orden(2)]
    public int Total { get; set; }


    // ===============================
    // OBSERVACIONES
    // ===============================

    [Nombre("Detalle")]
    [Grupo("Observaciones", 3)]
    [Orden(0)]
    public string Memo { get; set; } = string.Empty;


    // ===============================
    // FECHA
    // ===============================

    [Fecha]
    [Nombre("Fecha")]
    [Grupo("Datos del Documento", 1)]
    [Orden(3)]
    public DateTime Fecha { get; set; }


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}