using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

// Implementa la lista de detalles, implementa la interfaz IModel
public class Factura : FacturaBase
{
    [Nombre("Detalle")]
    public string Memo { get; set; }  = string.Empty;
    public long Total { set; get; }
    [Visible(false)]
    public string Guia1 { get; set; } = string.Empty;
    [Visible(false)]
    public string Guia2 { get; set; } = string.Empty;
    [Visible(false)]
    public string Guia3 { get; set; } = string.Empty;
    [Visible(false)]
    public string Guia4 { get; set; } = string.Empty;
    [Visible(false)]
    public string Guia5 { get; set; } = string.Empty;
    [Nombre("Orden de trabajo")]
    [Orden(3)]
    public string OrdenTrabajo { get; set; } = string.Empty;
    [Nombre("Fecha de vencimiento")]
    [Orden(4)]
    [Fecha]
    public DateTime FechaVencimiento { get; set; } = DateTime.Now;
    [Nombre("Crédito")]
    [Orden(5)]
    public long TipoCredito { get; set; }
    [Nombre("Haber")]
    [Orden(6)]
    public int Habe { get; set; }
    [Nombre("Nota de crédito")]
    [Orden(7)]
    public long NotaCredito { get; set; }

    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;
    [Nombre("Orden de trabajo")]
    public string Ot01 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot02 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot03 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot04 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot05 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op01 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op02 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op03 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op04 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op05 { get; set; } = string.Empty;
    [Visible(false)]
    public string Peli { get; set; } = string.Empty;
    [Visible(false)]
    public string Plan { get; set; } = string.Empty;
}