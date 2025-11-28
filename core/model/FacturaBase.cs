using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Gestion.core.attributes;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

public abstract class FacturaBase : IModel
{
    public long Id { get; set; }

    [Nombre("Rut")]
    [Orden(0)]
    [Rut]
    public string RutCliente { get; set; } = string.Empty;
    [Orden(1)]
    public string Folio { get; set; } = string.Empty;
    [Orden(2)]
    [Fecha]
    public DateTime Fecha { get; set; } = DateTime.Now;
    public long Neto { set; get; }
    public long Iva { set; get; }
    [Visible(false)]
    public long Empresa { get; set; }
    [Visible(false)]
    [DbIgnore]
    public string EmpresaNombre { get; set; } = string.Empty;
}
