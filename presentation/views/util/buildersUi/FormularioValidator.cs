using System.Reflection;
using System.Windows.Controls;

namespace Gestion.presentation.views.util.buildersUi;

public static class FormularioValidator
{
    public static void ForzarValidacionInicial(
        Dictionary<PropertyInfo, Control> controles)
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