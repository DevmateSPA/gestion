using System.Text;
using System.Windows;
using Gestion.core.interfaces.service;

namespace Gestion.presentation.views.util;

public static class DialogUtils
{
    private static IDialogService? _dialogService;

    public static void Init(IDialogService dialogService)
        => _dialogService = dialogService;

    private static IDialogService Service
    {
        get
        {
            if (_dialogService == null)
                throw new InvalidOperationException("DialogUtils no ha sido inicializado.");

            return _dialogService;
        }
    }

    public static void MostrarInfo(string mensaje, string titulo = "Información")
    {
        TryExecute(() => Service.ShowMessage(mensaje, titulo));
    }

    public static void MostrarAdvertencia(string mensaje, string titulo = "Advertencia")
    {
        TryExecute(() => Service.ShowWarning(mensaje, titulo));
    }

    public static void MostrarError(string mensaje, string titulo = "Error")
    {
        TryExecute(() => Service.ShowError(mensaje, titulo));
    }

    public static bool Confirmar(string mensaje, string titulo = "Confirmar acción")
    {
        return TryExecute(() => Service.Confirm(mensaje, titulo), false);
    }

    public static void MostrarErroresValidacion(IEnumerable<string> errores)
    {
        if (errores == null || !errores.Any())
            return;

        var sb = new StringBuilder();
        sb.AppendLine("Se encontraron los siguientes errores:");
        sb.AppendLine();

        foreach (var error in errores.Distinct())
            sb.AppendLine($"• {error}");

        MostrarAdvertencia(sb.ToString(), "Errores de validación");
    }

    // 🔐 Punto central del try/catch
    private static void TryExecute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Fallback(ex);
        }
    }

    private static T TryExecute<T>(Func<T> func, T defaultValue)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            Fallback(ex);
            return defaultValue;
        }
    }

    private static void Fallback(Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "Error crítico de diálogo",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }
}