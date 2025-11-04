using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Impresion : IModel
{
    public int Id { get; set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int Valorpormil { get; private set; } = 0;

    public Impresion() {}
}