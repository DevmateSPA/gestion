using System.ComponentModel.DataAnnotations;
using Gestion.core.attributes;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Producto : IEmpresa
{
    public long Id { get; set; }

    // ===============================
    // DATOS GENERALES
    // ===============================

    [Nombre("Código")]
    [Required]
    [Grupo("General", 1)]
    [Orden(0)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Grupo("General", 1)]
    [Orden(1)]
    public string Descripcion { get; set; } = string.Empty;

    [Grupo("General", 1)]
    [Orden(2)]
    public int Grupo { get; set; }

    [Nombre("Stock minimo")]
    [Grupo("General", 1)]
    [Orden(3)]
    public int Stmi { get; set; }

    [Grupo("Características", 2)]
    [Orden(1)]
    public int Papel { get; set; }

    [Nombre("Dimensiones")]
    [Grupo("Características", 2)]
    [Orden(2)]
    public int Dime { get; set; }

    [Nombre("P. Uni. Pliego(?")]
    [Grupo("Características", 2)]
    [Orden(0)]
    public int Punp { get; set; }


    // ===============================
    // CONTROL INTERNO (NO VISIBLE)
    // ===============================

    [Visible(false)]
    public int Enin { get; set; }

    [Visible(false)]
    public int Sain { get; set; }

    [Visible(false)]
    public int Saldo { get; set; }

    [Visible(false)]
    public int Salida { get; set; }

    [Visible(false)]
    public int Entrada { get; set; }

    [Visible(false)]
    public long Empresa { get; set; }
}