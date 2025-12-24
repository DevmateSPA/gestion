using System.ComponentModel.DataAnnotations;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Operario : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    [Required]
    public string Codigo { get; set; } = string.Empty;
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}