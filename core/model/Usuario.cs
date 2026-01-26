using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Usuario : IEmpresa
{
    public long Id { get; set; }
    [Orden(0)]
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Orden(1)]
    [Required]
    public string Clave { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
    [Visible(false)]
    [NoSaveDb]
    public string TipoDesc { get; set; } = string.Empty;
    [Orden(2)]
    [ComboSource("TIPOS_USUARIO", Display = "Nombre", Value = "Id")]
    public long Tipo { get; set; }
}