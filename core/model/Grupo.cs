using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Grupo : IModel
{
    public int Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; private set; } = string.Empty;
    [Nombre("Descripción")]
    public string Descripcion { get; private set; } = string.Empty;

    public Grupo() {}
}