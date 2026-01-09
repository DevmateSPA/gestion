using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Provee utilidades de validación para formularios dinámicos
/// construidos mediante reflexión.
/// </summary>
/// <remarks>
/// Esta clase permite forzar la ejecución inmediata de las reglas de
/// validación asociadas a los bindings, incluso antes de que el usuario
/// interactúe con los controles.
///
/// Es especialmente útil para:
/// <list type="bullet">
/// <item>Mostrar errores iniciales en formularios de edición.</item>
/// <item>Validar datos cargados automáticamente.</item>
/// <item>Prevenir guardados con estados inválidos no detectados aún.</item>
/// </list>
/// </remarks>
public static class FormularioValidator
{
    /// <summary>
    /// Fuerza la validación inicial de todos los controles del formulario.
    /// </summary>
    /// <param name="controles">
    /// Diccionario que relaciona propiedades de la entidad con sus
    /// controles visuales asociados.
    /// </param>
    /// <remarks>
    /// Este método invoca <see cref="System.Windows.Data.BindingExpression.UpdateSource"/>
    /// en los bindings relevantes para provocar la ejecución inmediata
    /// de las reglas de validación (DataAnnotations, excepciones, etc.).
    ///
    /// No modifica valores, solo sincroniza el estado de validación.
    /// </remarks>
    public static void ForzarValidacionInicial(
        Dictionary<PropertyInfo, FrameworkElement> controles)
    {
        foreach (var control in controles.Values)
        {
            switch (control)
            {
                case TextBox tb:
                    tb.GetBindingExpression(TextBox.TextProperty)
                      ?.UpdateSource();
                    break;

                case DatePicker dp:
                    dp.GetBindingExpression(DatePicker.SelectedDateProperty)
                      ?.UpdateSource();
                    break;

                case ComboBox cb:
                    cb.GetBindingExpression(ComboBox.SelectedItemProperty)
                      ?.UpdateSource();
                    break;
            }
        }
    }
}