using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class NotaCredito : IModel
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string RutCliente { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    public int Neto { get; set; }
    public int Iva { get; set; }
    public int Total { get; set; }
    public string Memoria { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}