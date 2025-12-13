using System.Windows;
using System.Windows.Controls;

namespace Gestion.presentation.views.util;

public static class ValidationHelper
{
    public static bool AnyValidationErros(FrameworkElement container)
    {
        if (container == null)
            return false;

        foreach (var child in GetLogicalChildren(container))
        {
            if (child is Control ctrl && (ctrl is TextBox || ctrl is DatePicker))
            {
                if (Validation.GetHasError(ctrl))
                    return true;
            }
        }

        return false;
    }

    private static IEnumerable<object> GetLogicalChildren(DependencyObject parent)
    {
        if (parent == null)
            yield break;

        foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<object>())
        {
            yield return child;

            if (child is DependencyObject dobj)
            {
                foreach (var grand in GetLogicalChildren(dobj))
                {
                    yield return grand;
                }
            }
        }
    }
}