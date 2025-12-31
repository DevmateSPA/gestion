using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Impresion : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS GENERALES
    // ===============================

    [Nombre("Código")]
    [Required]
    [Orden(0)]
    public string Codigo { get; set; } = string.Empty;

    [Nombre("Descripción")]
    [Orden(1)]
    public string Descripcion { get; set; } = string.Empty;

    [Nombre("Valor por Mil")]
    [Orden(2)]
    public int Valorpormil { get; set; } = 0;


    // ===============================
    // CONTROL
    // ===============================

    [Visible(false)]
    public long Empresa { get; set; }
}