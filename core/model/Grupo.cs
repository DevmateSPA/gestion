using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class Grupo : IEmpresa
{
    public long Id { get; set; }
    [Nombre("Código")]
    public string Codigo { get; set; } = string.Empty;
    [Nombre("Descripción")]
    public string Descripcion { get; set; } = string.Empty;
    [Visible(false)]
    public long Empresa { get; set; }
}