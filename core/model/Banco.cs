using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Banco : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    [Required]
    [Grupo("Datos Generales", 1)]
    [Orden(0)]
    public string Codigo { get; set; } = string.Empty;
    [Required]
    [Grupo("Datos Generales", 1)]
    [Orden(1)]
    public string Nombre { get; set; } = string.Empty;
    [Nombre("Dirección")]
    [Grupo("Datos Generales", 1)]
    [Orden(2)]
    public string Direccion { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}