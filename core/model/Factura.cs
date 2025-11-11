using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Factura : IModel
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
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
    public string Ot1 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot2 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot3 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot4 { get; set; } = string.Empty;
    [Visible(false)]
    public string Ot5 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op1 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op2 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op3 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op4 { get; set; } = string.Empty;
    [Visible(false)]
    public string Op5 { get; set; } = string.Empty;
    [Visible(false)]
    public string peli { get; set; } = string.Empty;
    [Visible(false)]
    public string plan { get; set; } = string.Empty;

    public List<Detalle> Detalles { get; set; } = new List<Detalle>();

}