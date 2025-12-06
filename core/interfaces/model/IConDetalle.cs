using System.Collections.ObjectModel;

namespace Gestion.core.interfaces.model;

public interface IConDetalles<T>
{
    ObservableCollection<T> Detalles { get; set; }
}
