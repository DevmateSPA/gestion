using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Maquina : IModel
{
    public int Id { set; get; }
    public string maquina { get; private set; } = string.Empty;
    public string descripcion { get; private set; } = string.Empty;
}