using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    public long Id { get; set; }

    [Nombre("Rut")]
    [Orden(0)]
    public string RutCliente { get; set; } = string.Empty;
    [Orden(1)]
    public string Folio { get; set; } = string.Empty;
    [Orden(2)]
    public DateTime Fecha { get; set; } = DateTime.Now;
    private long _neto;
    [NotMapped]
    public long Neto
    {
        get => _neto;
        private set
        {
            if (_neto != value)
            {
                _neto = value;
                OnPropertyChanged(nameof(Neto));
            }
        }
    }

    private long _iva;
    [NotMapped]
    public long Iva
    {
        get => _iva;
        private set
        {
            if (_iva != value)
            {
                _iva = value;
                OnPropertyChanged(nameof(Iva));
            }
        }
    }

    private long _total;
    [NotMapped]
    public long Total
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

    private void DetallesChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
            .Select(d => (long?)d.GetType().GetProperty("Total")?.GetValue(d) ?? 0)
            .Sum();

        Neto = (long)Math.Round(Total / 1.19);
        Iva = Total - Neto;
    }
}
