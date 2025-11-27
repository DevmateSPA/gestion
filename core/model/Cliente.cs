using System.ComponentModel.DataAnnotations;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Cliente : IModel
{
    public long Id { get; set; }
    [Required]
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
    [Nombre("Observación")]
    public string Observaciones1 { get; set; } = string.Empty;
    public int debi { get; set; }
    [Nombre("Debe Inicial")]
    public int habi { get; set; }
    [Nombre("Haber Inicial")]
    public long debe { get; set; }
    [Nombre("Haber")]
    public long habe { get; set; }
    [Nombre("Saldo")]
    public long saldo { get; set; }
    [Visible(false)]
    public int Empresa { get; set; }
}