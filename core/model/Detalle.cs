using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Detalle : IModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // Campos privados
    private int _cantidad;
    private int _precio;
    public int Id { get; set; }
    [Visible(false)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;
    [Orden(1)]
    public string Producto { get; set; } = string.Empty;
    [Orden(2)]
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (_cantidad != value)
            {
                _cantidad = value;
                OnPropertyChanged(nameof(Cantidad));
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    [Orden(3)]
    public int Precio
    {
        get => _precio;
        set
        {
            if (_precio != value)
            {
                _precio = value;
                OnPropertyChanged(nameof(Precio));
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    [Orden(4)]
    [NotMapped]
    public int Total => Cantidad * Precio;
    public DateTime Fecha { get; set; }
}
