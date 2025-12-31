using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Fotomecanica : IEmpresa
{
    public long Id { get; set; }

    [Nombre("Código")]
    [Required]
    [Orden(0)]
    public string Codigo { get; set; } = string.Empty;

    [Nombre("Descripción")]
    [Required]
    [Orden(1)]
    public string Descripcion { get; set; } = string.Empty;

    [Visible(false)]
    public long Empresa { get; set; }
}