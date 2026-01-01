using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class FacturaCompra : FacturaBase, IConDetalles<FacturaCompraProducto>, INotifyPropertyChanged
{
    private const int GRUPO_DOCUMENTO = 10;
    private const int GRUPO_CLIENTE = 20;


    [Nombre("Folio")]
    [Required]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(0)]
    public string Folio { get; set; } = string.Empty;

    [Nombre("Fecha")]
    [Fecha]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(1)]
    public DateTime Fecha { get; set; } = DateTime.Now;
    [Nombre("Fecha de Vencimiento")]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(2)]
    [Fecha]
    public DateTime FechaVencimiento { get; set; } = DateTime.Now;

    [Nombre("Tipo de Factura")]
    [Required]
    [Visible(false)]
    [Grupo("Datos del Documento", GRUPO_DOCUMENTO)]
    [Orden(3)]
    public string Tipo { get; set; } = "FA";


    [Nombre("Rut")]
    [Rut]
    [Grupo("Proveedor", GRUPO_CLIENTE)]
    [Orden(0)]
    public string RutCliente { get; set; } = string.Empty;

    [Nombre("Nombre")]
    [Grupo("Proveedor", GRUPO_CLIENTE)]
    [Orden(1)]
    [NoSaveDb]
    [OnlyRead]
    public string NombreProveedor { get; set; } = string.Empty;

    [Nombre("Haber")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(4)]
    [Visible(false)]
    public int Habe { get; set; }
    [Nombre("Forma de pago")]
    [Grupo("Totales", GRUPO_TOTALES)]
    [Orden(5)]
    [Visible(false)]
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
                OnPropertyChanged(nameof(Detalles));
            }
        }
    }
}