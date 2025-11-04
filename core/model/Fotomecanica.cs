using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Fotomecanica : IModel
{
    public int Id { get; set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public Fotomecanica() {}
}