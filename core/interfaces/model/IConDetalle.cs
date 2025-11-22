using System.Collections.ObjectModel;
using Gestion.core.interfaces.model;

public interface IConDetalles<T> : IModel
{
    string Folio { get; set; }
    string RutCliente { get; set; }
    DateTime Fecha { get; set; }
    long Neto { get; }
    long Iva { get; }
    long Total { get; }
    ObservableCollection<T> Detalles { get; set; }
}
