using Gestion.core.interfaces.service;

namespace Gestion.helpers;

public static class SafeExecutor
{
    public static async Task RunAsync(Func<Task> action, IDialogService dialogService, string mensajeError)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            dialogService.ShowError($"{mensajeError}: {ex.Message}");
        }
    }
}