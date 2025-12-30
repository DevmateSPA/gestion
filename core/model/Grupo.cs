using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Grupo : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS GENERALES
    // ===============================

    [Nombre("Código")]
    [Required]
    [Grupo("Datos Generales", 1)]
    [Orden(0)]
    public string Codigo { get; set; } = string.Empty;

    [Nombre("Descripción")]
    [Required]
    [Grupo("Datos Generales", 1)]
    [Orden(1)]
    public string Descripcion { get; set; } = string.Empty;


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}