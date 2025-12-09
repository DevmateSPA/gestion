using System.ComponentModel.DataAnnotations;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Fotomecanica : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    [Required]
    public string Codigo { get; set; } = string.Empty;
    [Nombre("Descripción")]
    [Required]
    public string Descripcion { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}