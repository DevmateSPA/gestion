using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class DocumentoNulo : IEmpresa
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public string Glosa { get; set; } = string.Empty;
    [Fecha]
    public DateTime Fecha { get; set; }
    [Visible(false)]
    public long Empresa { get; set; }
}