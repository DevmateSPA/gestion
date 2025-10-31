using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Operador : IModel
{
    public int Id { get; set; }
    public string nombre { get; private set; } = string.Empty;
    public string operador { get; private set; } = string.Empty;

    public Operador() {}
}