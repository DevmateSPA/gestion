using System.Collections.ObjectModel;
using System.ComponentModel;
using Gestion.core.interfaces.model;

public interface IConDetalles<T> : IModel, INotifyPropertyChanged
{
    string Folio { get; set; }
    string RutCliente { get; set; }
    DateTime Fecha { get; set; }
    long Neto { get; }
    long Iva { get; }
    long Total { get; }
    string Detalle { get; }
}
