using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Banco : IModel
{
    public int Id { get; set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;

    public Banco() {}
}