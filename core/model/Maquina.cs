using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Maquina : IModel
{
    public long Id { set; get; }
    [Nombre("Código")]
    public string Codigo { get; private set; } = string.Empty;
    [Nombre("Descripción")]
    public string Descripcion { get; private set; } = string.Empty;
    [Visible(false)]
    public int Empresa { get; set; }
}