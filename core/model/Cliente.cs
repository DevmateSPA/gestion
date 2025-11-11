using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Cliente : IModel
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
    [Nombre("Observación")]
    public string Obs1 { get; private set; } = string.Empty;
    public int debi { get; private set; }
    [Nombre("Debe Inicial")]
    public int habi { get; private set; }
    [Nombre("Haber Inicial")]
    public long debe { get; private set; }
    [Nombre("Haber")]
    public long habe { get; private set; }
    [Nombre("Saldo")]
    public long saldo { get; private set; }

    public Cliente() { }
    public Cliente(string rut,
        string razonSocial,
        string giro,
        string direccion,
        string ciudad,
        string telefono,
        string fax,
        string obs1,
        int debi,
        int habi,
        long debe,
        long habe,
        long saldo)
    {
        this.Rut = rut;
        this.Razon_Social = razonSocial;
        this.Giro = giro;
        this.Direccion = direccion;
        this.Ciudad = ciudad;
        this.Telefono = telefono;
        this.Fax = fax;
        this.Obs1 = obs1;
        this.debi = debi;
        this.habi = habi;
        this.debe = debe;
        this.habe = habe;
        this.saldo = saldo;
    }
}