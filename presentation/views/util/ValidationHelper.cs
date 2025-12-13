using System.Windows;
using System.Windows.Controls;

namespace Gestion.presentation.views.util;

public static class ValidationHelper
{
    public static List<string> GetValidationErrors(FrameworkElement container)
    {
        List<string> errores = [];

        if (container == null)
            return errores;

        foreach (var child in GetLogicalChildren(container))
        {
            if (child is Control ctrl && (ctrl is TextBox || ctrl is DatePicker))
            {
                if (Validation.GetHasError(ctrl))
                {
                    foreach (var error in Validation.GetErrors(ctrl))
                    {
                        if (error.ErrorContent != null)
                            errores.Add(error.ErrorContent?.ToString() ?? "Error de validación");
                    }
                }
            }
        }

        return errores;
    }

    public static bool AnyValidationErrors(FrameworkElement container)
        => GetValidationErrors(container).Count != 0;

    private static IEnumerable<object> GetLogicalChildren(DependencyObject parent)
    {
        if (parent == null)
            yield break;

        foreach (var child in LogicalTreeHelper.GetChildren(parent))
        {
            yield return child;

            if (child is DependencyObject dobj)
            {
                foreach (var grand in GetLogicalChildren(dobj))
                    yield return grand;
            }
        }
    }

    public static void ForceValidation(FrameworkElement container)
    {
        if (container == null)
            return;

        foreach (var child in GetLogicalChildren(container))
        {
            if (child is Control ctrl &&
                (ctrl is TextBox || ctrl is DatePicker))
            {
                // Fuerza el UpdateSource del binding
                if (ctrl is TextBox tb)
                {
                    tb.GetBindingExpression(TextBox.TextProperty)
                    ?.UpdateSource();
                }
                else if (ctrl is DatePicker dp)
                {
                    dp.GetBindingExpression(DatePicker.SelectedDateProperty)
                    ?.UpdateSource();
                }
            }
        }
    }
}