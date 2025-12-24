using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Encuadernacion : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    [Required]
    public string Codigo { get; set; } = string.Empty;
    [Nombre("Descripción")]
    [Required]
    public string Descripcion { get; set; } = string.Empty;
    [Nombre("Valor por mil")]
    public int Valorpormil { get; set; } = 0;
    [Visible(false)]
    public long Empresa { get; set; }
}