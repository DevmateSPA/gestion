using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;
using Gestion.core.model.detalles;

namespace Gestion.core.model;

public class FacturaCompra : FacturaBase
{
    [Visible(false)]
    public string Tipo { get; set; } = string.Empty;
    [Nombre("Proveedor")]
    [Visible(false)]
    public int Debe { get; set; }
    [Visible(false)]
    public int Habe { get; set; }
    [Visible(false)]
    public string Fopa { get; set; } = string.Empty;
    [Visible(false)]
    public int Empresa { get; set; }
    [NotMapped]
    public ObservableCollection<Detalle> Detalles = new();
}