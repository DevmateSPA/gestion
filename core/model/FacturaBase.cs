using System.ComponentModel;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public abstract class FacturaBase : IEmpresa, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public long Id { get; set; }


    [Visible(false)]
    public long Empresa { get; set; }

    protected const int GRUPO_TOTALES = 30;

    [Nombre("Neto")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(0)]
    [OnlyRead]
    public long Neto { get; set; }

    [Nombre("I.V.A")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(1)]
    [OnlyRead]
    public long Iva { get; set; }

    private long _total;

    [Nombre("Total")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(3)]
    public long Total
    {
        get => _total;
        set
        {
            if (_total == value) return;
            _total = value;
            OnPropertyChanged(nameof(Total));
            RecalcularTotales();
        }
    }

    private const decimal IVA_FACTOR = 1.19m;

    private void RecalcularTotales()
    {
        Neto = (long)Math.Round(Total / IVA_FACTOR);
        Iva = Total - Neto;

        OnPropertyChanged(nameof(Neto));
        OnPropertyChanged(nameof(Iva));
    }
}