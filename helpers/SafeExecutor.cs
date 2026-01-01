using System.Windows;
using Gestion.core.interfaces.service;

namespace Gestion.helpers;

public static class SafeExecutor
{
    public static async Task RunAsync(
        Func<Task> action,
        IDialogService dialogService,
        string mensajeError)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialogService.ShowError($"{mensajeError}:\n{ex.Message}");
            });
        }
    }

    public static async Task<T> RunAsyncValue<T>(
        Func<Task<T>> action,
        IDialogService dialogService,
        string mensajeError,
        T defaultValue = default!)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialogService.ShowError($"{mensajeError}:\n{ex.Message}");
            });

            return defaultValue;
        }
    }

    public static async Task<List<T>> RunAsyncList<T>(
        Func<Task<List<T>>> action,
        IDialogService dialogService,
        string mensajeError)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialogService.ShowError($"{mensajeError}:\n{ex.Message}");
            });

            return [];
        }
    }
}