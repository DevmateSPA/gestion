using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

public abstract class FacturaBase<T> : IConDetalles<T>
{
    public int Id { get; set; }
    [NotMapped]
    public ObservableCollection<T> Detalles { get; set; } = new();
}