using System.ComponentModel.DataAnnotations;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Empresa : IModel
{
    public long Id { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;
}