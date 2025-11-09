using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Maquina : IModel
{
    public int Id { set; get; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
}