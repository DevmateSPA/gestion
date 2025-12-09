using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Proveedor : IEmpresa
{
    public long Id { get; set; }
    [Rut]
    public string Rut { get; set; } = string.Empty;
    [Nombre("Razón Social")]
    public string Razon_Social { get; set; } = string.Empty;
    public string Giro { get; set; } = string.Empty;
    [Nombre("Dirección")]
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    [Nombre("Teléfono")]
    public string Telefono { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    [Nombre("Observaciones")]
    public string Obs1 { get; set; } = string.Empty;
    [Visible(false)]
    public string Obs2 { get; set; } = string.Empty;
    [Nombre("Debe inicial")]
    public int Debi { get; set; }
    [Nombre("Haber inicial")]
    public int Habi { get; set; }
    public long Debe { get; set; }
    [Nombre("Haber")]
    public long Habe { get; set; }
    public long Saldo { get; set; }
    [Visible(false)]
    public long Empresa { get; set; }
}