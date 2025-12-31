using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

// Implementa la lista de detalles, implementa la interfaz IModel
public class Factura : IEmpresa
{
    private const int GRUPO_DOCUMENTO = 10;
    private const int GRUPO_CLIENTE = 20;
    private const int GRUPO_TOTALES = 30;
    private const int GRUPO_GUIAS = 40;
    private const int GRUPO_ORDENES = 50;

    public long Id { get; set; }
    [Visible(false)]
    public long Empresa { get; set; }

    [Nombre("Folio")]
    [Required]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Fecha")]
    [Fecha]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(1)]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Nombre("Rut")]
    [Rut]
    [Grupo("Cliente", GRUPO_CLIENTE)]
    [Orden(0)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Nombre")]
    [Grupo("Cliente", GRUPO_CLIENTE)]
    [Orden(1)]
    [NotMapped]
    public string NombreCliente { get; set; } = string.Empty;

    [Nombre("Neto")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(0)]
    public long Neto { get; set; }

    [Nombre("I.V.A")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(1)]
    public long Iva { get; set; }

    [Nombre("Total")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(2)]
    public long Total { get; set; }

    [Nombre("Crédito")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(5)]
    public long TipoCredito { get; set; }

    [Nombre("Haber")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(6)]
    public int Habe { get; set; }

    [NotMapped]
    public decimal Debe => Total - Habe;

    [Nombre("Nota de Crédito")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(7)]
    public long NotaCredito { get; set; }

    [Nombre("Orden de Compra")]
    [Grupo("Datos Internos", GRUPO_TOTALES+1)]
    [Orden(0)]
    public string OrdenTrabajo { get; set; } = string.Empty;

    [Nombre("Fecha de Vencimiento")]
    [Grupo("Datos Internos", GRUPO_TOTALES+1)]
    [Orden(1)]
    [Fecha]
    public DateTime FechaVencimiento { get; set; } = DateTime.Now;

    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;

    private const string OrdenesDeTrabajo = "Ordenes de Trabajo";

    [Nombre("Orden 1")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(0)]
    public string Ot01 { get; set; } = string.Empty;

    [Nombre("Orden 2")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(1)]
    public string Ot02 { get; set; } = string.Empty;
    [Nombre("Orden 3")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(2)]
    public string Ot03 { get; set; } = string.Empty;
    [Nombre("Orden 4")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(3)]
    public string Ot04 { get; set; } = string.Empty;
    [Nombre("Orden 5")]
    [Grupo(OrdenesDeTrabajo, GRUPO_ORDENES)]
    [Orden(4)]
    public string Ot05 { get; set; } = string.Empty;

    [Visible(false)] public string Op01 { get; set; } = string.Empty;
    [Visible(false)] public string Op02 { get; set; } = string.Empty;
    [Visible(false)] public string Op03 { get; set; } = string.Empty;
    [Visible(false)] public string Op04 { get; set; } = string.Empty;
    [Visible(false)] public string Op05 { get; set; } = string.Empty;

    private const string GuiasDespacho = "Guías de Despacho";

    [Nombre("Guía 1")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(0)]
    public string Guia1 { get; set; } = string.Empty;

    [Nombre("Guía 2")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(1)]
    public string Guia2 { get; set; } = string.Empty;

    [Nombre("Guía 3")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(2)]
    public string Guia3 { get; set; } = string.Empty;

    [Nombre("Guía 4")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(3)]
    public string Guia4 { get; set; } = string.Empty;

    [Nombre("Guía 5")]
    [Grupo(GuiasDespacho, GRUPO_GUIAS)]
    [Orden(4)]
    public string Guia5 { get; set; } = string.Empty;

    [Nombre("Película")]
    [Orden(1)]
    public bool Peli { get; set; }
    [Nombre("Plan")]
    [Orden(2)]
    public bool Plan { get; set; }

    // ===============================
    // OBSERVACIONES / DETALLE
    // ===============================

    [Nombre("Detalle")]
    public string Memo { get; set; } = string.Empty;
}