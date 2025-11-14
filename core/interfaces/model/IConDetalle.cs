using System.Collections.ObjectModel;

namespace Gestion.core.interfaces.model;

public interface IConDetalles<T> : IModel
{
    ObservableCollection<T> Detalles { get; }
}