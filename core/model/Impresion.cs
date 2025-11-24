using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Impresion : IModel
{
    public long Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; private set; } = string.Empty;
    [Nombre("Descripción")]
    public string Descripcion { get; private set; } = string.Empty;
    [Nombre("Valor por mil")]
    public int Valorpormil { get; private set; } = 0;
    [Visible(false)]
    public int Empresa { get; set; }

    public Impresion() {}
}