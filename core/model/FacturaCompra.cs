using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompra : FacturaBase, IConDetalles<FacturaCompraProducto>, INotifyPropertyChanged
{
    public string Tipo { get; set; } = string.Empty;
    public int Debe { get; set; }
    public int Habe { get; set; }
    public string Fopa { get; set; } = string.Empty;
    [NotMapped]
    private ObservableCollection<FacturaCompraProducto> _detalles 
        = new ObservableCollection<FacturaCompraProducto>();

    [NotMapped]
    public ObservableCollection<FacturaCompraProducto> Detalles
    {
        get => _detalles;
        set
        {
            if (_detalles != value)
            {
                _detalles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Detalles)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}