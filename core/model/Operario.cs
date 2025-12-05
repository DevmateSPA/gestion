    using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Operario : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}