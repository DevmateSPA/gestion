using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Grupo : IModel
{
    public int Id { get; set; }
    public string grupo { get; private set; } = string.Empty;
    public string descripcion { get; private set; } = string.Empty;

    public Grupo() {}
}