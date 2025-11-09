using Gestion.core.interfaces.model;

namespace Gestion.core.model;

public class DocumentoNulo : IModel
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public string Glosa { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}