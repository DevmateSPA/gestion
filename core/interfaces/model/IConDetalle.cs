using System.Collections.ObjectModel;

namespace Gestion.core.interfaces.model;

public interface IConDetalles<T> : IModel
{
    string Folio { get; set; }
    ObservableCollection<T> Detalles { get; }
}