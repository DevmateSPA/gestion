using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Banco : IModel
{
    public int Id { get; set; }
    public string banco { get; private set; } = string.Empty;
    public string nombre { get; private set; } = string.Empty;
    public string direccion { get; private set; } = string.Empty;

    public Banco() {}
}