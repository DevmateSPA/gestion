using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

public abstract class FacturaBase<T> : IConDetalles<T>
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    [NotMapped]
    public ObservableCollection<T> Detalles { get; set; } = new();
}