using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

namespace Gestion.core.model.detalles;

public abstract class ConDetalles<T> : IConDetalles<T>, INotifyPropertyChanged
{
    [NotMapped]
    private ObservableCollection<T> _detalles = new ObservableCollection<T>();
    [NotMapped]
    public ObservableCollection<T> Detalles
    {
        get => _detalles;
        set
        {
            if (_detalles != value)
            {
                _detalles = value;
                OnPropertyChanged(nameof(Detalles));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}