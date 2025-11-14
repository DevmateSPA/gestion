using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

public abstract class FacturaBase<T> : IConDetalles<T>
{
    public int Id { get; set; }
    [Nombre("Rut")]
    public string RutCliente { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int Neto { get; set; }
    public int Iva { get; set; }
    public int Total { get; set; }
    [NotMapped]
    public ObservableCollection<T> Detalles { get; set; } = new();
}