using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Gestion.core.interfaces.model;

public abstract class FacturaBase<T> : IConDetalles<T>, INotifyPropertyChanged 
    where T : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public int Id { get; set; }

    [Nombre("Rut")]
    public string RutCliente { get; set; } = string.Empty;

    public string Folio { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int Neto { get; set; }

    public int Iva { get; set; }

    private int _total;

    [NotMapped]
    public int Total
    {
        get => _total;
        private set
        {
            if (_total != value)
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    private ObservableCollection<T> _detalles = new();
    
    [NotMapped]
    public ObservableCollection<T> Detalles
    {
        get => _detalles;
        set
        {
            if (_detalles != null)
            {
                _detalles.CollectionChanged -= DetallesChanged;
                foreach (var d in _detalles)
                    d.PropertyChanged -= DetalleChanged;
            }

            _detalles = value;

            _detalles.CollectionChanged += DetallesChanged;
            foreach (var d in _detalles)
                d.PropertyChanged += DetalleChanged;

            RecalcularTotal();
        }
    }
    private void DetallesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (T item in e.NewItems)
                item.PropertyChanged += DetalleChanged;
        }

        if (e.OldItems != null)
        {
            foreach (T item in e.OldItems)
                item.PropertyChanged -= DetalleChanged;
        }

        RecalcularTotal();
    }

    private void DetalleChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Cantidad" || e.PropertyName == "Precio" || e.PropertyName == "Total")
            RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        Total = Detalles
            .Select(d => (int?)d.GetType().GetProperty("Total")?.GetValue(d) ?? 0)
            .Sum();
    }
}
