using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class OrdenCompraPelicula : IModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public string RutCliente { get; set; } = string.Empty;
    public int Neto { get; set; }
    public int Iva { get; set; }
    public int Debe { get; set; }
    public int Habe { get; set; }
    public int fopa { get; set; }
}