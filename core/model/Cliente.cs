using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Cliente : IEmpresa
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
    [Nombre("Observación")]
    public string Observaciones1 { get; set; } = string.Empty;
    public int debi { get; set; }
    [Nombre("Debe Inicial")]
    public int habi { get; set; }
    [Nombre("Haber Inicial")]
    public long Debe { get; set; }
    [Nombre("Haber")]
    public long Habe { get; set; }
    [Nombre("Saldo")]
    public long saldo { get; set; }
    [Visible(false)]
    public long Empresa { get; set; }
}