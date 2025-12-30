using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

// Implementa la lista de detalles, implementa la interfaz IModel
public class Factura : FacturaBase
{
    // ===============================
    // OBSERVACIONES / DETALLE
    // ===============================

    [Nombre("Detalle")]
    [Grupo("Observaciones", 4)]
    [Orden(1)]
    public string Memo { get; set; } = string.Empty;


    // ===============================
    // CRÉDITO / PAGOS
    // ===============================

    [Nombre("Orden de trabajo")]
    [Grupo("Totales", 3)]
    [Orden(3)]
    public string OrdenTrabajo { get; set; } = string.Empty;

    [Nombre("Fecha de vencimiento")]
    [Grupo("Totales", 3)]
    [Orden(4)]
    [Fecha]
    public DateTime FechaVencimiento { get; set; } = DateTime.Now;

    [Nombre("Crédito")]
    [Grupo("Totales", 3)]
    [Orden(5)]
    public long TipoCredito { get; set; }

    [Nombre("Haber")]
    [Grupo("Totales", 3)]
    [Orden(6)]
    public int Habe { get; set; }

    [NotMapped]
    public decimal Debe => Total - Habe;

    [Nombre("Nota de crédito")]
    [Grupo("Totales", 3)]
    [Orden(7)]
    public long NotaCredito { get; set; }


    // ===============================
    // REFERENCIAS / CONTROL
    // ===============================

    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;

    [Nombre("Orden de trabajo")]
    [Grupo("Control", 5)]
    [Orden(0)]
    public string Ot01 { get; set; } = string.Empty;

    [Visible(false)] public string Ot02 { get; set; } = string.Empty;
    [Visible(false)] public string Ot03 { get; set; } = string.Empty;
    [Visible(false)] public string Ot04 { get; set; } = string.Empty;
    [Visible(false)] public string Ot05 { get; set; } = string.Empty;

    [Visible(false)] public string Op01 { get; set; } = string.Empty;
    [Visible(false)] public string Op02 { get; set; } = string.Empty;
    [Visible(false)] public string Op03 { get; set; } = string.Empty;
    [Visible(false)] public string Op04 { get; set; } = string.Empty;
    [Visible(false)] public string Op05 { get; set; } = string.Empty;

    [Visible(false)] public string Guia1 { get; set; } = string.Empty;
    [Visible(false)] public string Guia2 { get; set; } = string.Empty;
    [Visible(false)] public string Guia3 { get; set; } = string.Empty;
    [Visible(false)] public string Guia4 { get; set; } = string.Empty;
    [Visible(false)] public string Guia5 { get; set; } = string.Empty;

    [Visible(false)] public string Peli { get; set; } = string.Empty;
    [Visible(false)] public string Plan { get; set; } = string.Empty;
}