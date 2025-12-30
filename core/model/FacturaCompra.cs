using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompra : FacturaBase, IConDetalles<FacturaCompraProducto>, INotifyPropertyChanged
{
    [Required]
    [Visible(false)]
    public string Tipo { get; set; } = "FA";
    [Nombre("Debe")]
    [Grupo("Totales", 3)]
    [Orden(5)]
    public int Debe { get; set; }
    [Nombre("Haber")]
    [Grupo("Totales", 3)]
    [Orden(4)]
    public int Habe { get; set; }
    [Nombre("Forma de pago")]
    [Grupo("Totales", 3)]
    [Orden(3)]
    public string Fopa { get; set; } = string.Empty;
    [NotMapped]
    private ObservableCollection<FacturaCompraProducto> _detalles = [];
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