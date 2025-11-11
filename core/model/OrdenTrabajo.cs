using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class OrdenTrabajo : IModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    [Nombre("Rut Cliente")]
    public string RutCliente { get; set; } = string.Empty;
    public int Neto { get; set; }
    public int Iva { get; set; }
    [Visible(false)]
    public int Debe { get; set; }
    [Visible(false)]
    public int Habe { get; set; }
    [Visible(false)]
    public string Fopa { get; set; } = string.Empty;
}