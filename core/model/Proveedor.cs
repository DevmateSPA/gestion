using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Proveedor : IModel
{
    public int Id { get; set; }
    public string Rut { get; private set; } = string.Empty;
    public string Razon_Social { get; private set; } = string.Empty;
    public string Giro { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public string Ciudad { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string Fax { get; private set; } = string.Empty;
    public string Obs1 { get; private set; } = string.Empty;
    public string Obs2 { get; private set; } = string.Empty;
    public int debi { get; private set; }
    public int habi { get; private set; }
    public long debe { get; private set; }
    public long habe { get; private set; }
    public long saldo { get; private set; }

    public Proveedor() { }
}