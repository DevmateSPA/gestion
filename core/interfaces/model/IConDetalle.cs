using System.Collections.ObjectModel;

namespace Gestion.core.interfaces.model;

public interface IConDetalles<T> : IModel
{
    string Folio { get; set; }
    string RutCliente { get; set; }
    DateTime Fecha { get; set; }
    int Neto { get; set; }
    int Iva { get; set; }
    int Total { get; set; }
    ObservableCollection<T> Detalles { get; }
}