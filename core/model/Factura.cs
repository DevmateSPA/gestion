using Gestion.core.interfaces.model;

namespace Gestion.core.model;

// Implementa la lista de detalles, implementa la interfaz IModel
public class Factura : FacturaBase<Detalle>
{
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    [Nombre("Rut Cliente")]
    public string RutCliente { get; set; } = string.Empty;
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
    public string OrdenTrabajo { get; set; } = string.Empty;
    [Nombre("Fecha de vencimiento")]
    public string FechaVencimiento { get; set; } = string.Empty;
    [Nombre("Crédito")]
    public int TipoCredito { get; set; }
    public int Neto { get; set; }
    public int Iva { get; set; }
    public int Total { get; set; }
    [Nombre("Haber")]
    public int Habe { get; set; }
    [Nombre("Nota de crédito")]
    public int NotaCredito { get; set; }
    [Visible(false)]
    public string Memo { get; set; } = string.Empty;
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
    public string peli { get; set; } = string.Empty;
    [Visible(false)]
    public string plan { get; set; } = string.Empty;
}