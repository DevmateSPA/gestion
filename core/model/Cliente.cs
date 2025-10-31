using Gestion.core.interfaces;

namespace Gestion.core.model;

public class Cliente : IModel
{
    public int Id { get; set; }
    public string Rut { get; private set; } = string.Empty;
    public string RazonSocial { get; private set; } = string.Empty;
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

    public Cliente() { }
    public Cliente(string rut,
        string razonSocial,
        string giro,
        string direccion,
        string ciudad,
        string telefono,
        string fax,
        string obs1,
        string obs2,
        int debi,
        int habi,
        long debe,
        long habe,
        long saldo)
    {
        this.Rut = rut;
        this.RazonSocial = razonSocial;
        this.Giro = giro;
        this.Direccion = direccion;
        this.Ciudad = ciudad;
        this.Telefono = telefono;
        this.Fax = fax;
        this.Obs1 = obs1;
        this.Obs2 = obs2;
        this.debi = debi;
        this.habi = habi;
        this.debe = debe;
        this.habe = habe;
        this.saldo = saldo;
    }
}