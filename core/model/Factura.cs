using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Factura : IModel
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string RutCliente { get; set; } = string.Empty;
    public string Guia1 { get; set; } = string.Empty;
    public string Guia2 { get; set; } = string.Empty;
    public string Guia3 { get; set; } = string.Empty;
    public string Guia4 { get; set; } = string.Empty;
    public string Guia5 { get; set; } = string.Empty;
    public string OrdenTrabajo { get; set; } = string.Empty;
    public string FechaVencimiento { get; set; } = string.Empty;
    public int TipoCredito { get; set; }
    public int Neto { get; set; }
    public int Iva { get; set; }
    public int Total { get; set; }
    public int Habe { get; set; }
    public int NotaCredito { get; set; }
    public string Memo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Ot1 { get; set; } = string.Empty;
    public string Ot2 { get; set; } = string.Empty;
    public string Ot3 { get; set; } = string.Empty;
    public string Ot4 { get; set; } = string.Empty;
    public string Ot5 { get; set; } = string.Empty;
    public string Op1 { get; set; } = string.Empty;
    public string Op2 { get; set; } = string.Empty;
    public string Op3 { get; set; } = string.Empty;
    public string Op4 { get; set; } = string.Empty;
    public string Op5 { get; set; } = string.Empty;
    public string peli { get; set; } = string.Empty;
    public string plan { get; set; } = string.Empty;
}