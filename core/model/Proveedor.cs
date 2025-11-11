using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Proveedor : IModel
{
    public int Id { get; set; }
    public string Rut { get; private set; } = string.Empty;
    [Nombre("Razón Social")]
    public string Razon_Social { get; private set; } = string.Empty;
    public string Giro { get; private set; } = string.Empty;
    [Nombre("Dirección")]
    public string Direccion { get; private set; } = string.Empty;
    public string Ciudad { get; private set; } = string.Empty;
    [Nombre("Teléfono")]
    public string Telefono { get; private set; } = string.Empty;
    public string Fax { get; private set; } = string.Empty;
    [Nombre("Observaciones")]
    public string Obs1 { get; private set; } = string.Empty;
    [Visible(false)]
    public string Obs2 { get; private set; } = string.Empty;
    [Nombre("Debe inicial")]
    public int Debi { get; private set; }
    [Nombre("Haber inicial")]
    public int Habi { get; private set; }
    public long Debe { get; private set; }
    [Nombre("Haber")]
    public long Habe { get; private set; }
    public long Saldo { get; private set; }

    public Proveedor() { }
}