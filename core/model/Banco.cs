using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Banco : IModel
{
    public long Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    [Nombre("Dirección")]
    public string Direccion { get; private set; } = string.Empty;

    public Banco() {}
}