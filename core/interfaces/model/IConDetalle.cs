using System.Collections.ObjectModel;
using System.ComponentModel;
using Gestion.core.interfaces.model;

public interface IConDetalles<T> : IModel, INotifyPropertyChanged
{
    string Folio { get; set; }
    string RutCliente { get; set; }
    DateTime Fecha { get; set; }
    int Neto { get; }
    int Iva { get; }
    int Total { get; }

    ObservableCollection<T> Detalles { get; }
}
