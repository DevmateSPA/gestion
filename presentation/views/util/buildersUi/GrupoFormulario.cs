using System.Reflection;

namespace Gestion.presentation.views.util.buildersUi;

public class GrupoFormulario
{
    public string Nombre { get; set; } = "Otros";
    public int Orden { get; set; }
    public List<PropertyInfo> Propiedades { get; set; } = [];
}