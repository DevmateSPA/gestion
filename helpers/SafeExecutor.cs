using System.Windows;
using Gestion.core.attributes;
using Gestion.core.interfaces.service;

namespace Gestion.helpers;

/// <summary>
/// Proporciona métodos auxiliares para ejecutar acciones asincrónicas
/// de forma segura, manejando excepciones y errores de UI.
/// </summary>
/// <remarks>
/// Esta clase:
/// <list type="bullet">
/// <item>Muestra mensajes de error mediante <see cref="IDialogService"/>.</item>
/// <item>Protege la ejecución en el hilo de UI.</item>
/// <item>Detecta métodos potencialmente peligrosos ejecutados en UI Thread.</item>
/// <item>Respeta métodos marcados con <see cref="UiSafeAttribute"/>.</item>
/// </list>
/// </remarks>
public static class SafeExecutor
{
    /// <summary>
    /// Ejecuta una acción asincrónica sin valor de retorno,
    /// capturando excepciones y mostrando un mensaje de error en la UI.
    /// </summary>
    /// <param name="action">Acción asincrónica a ejecutar.</param>
    /// <param name="dialogService">
    /// Servicio encargado de mostrar diálogos de error al usuario.
    /// </param>
    /// <param name="mensajeError">
    /// Mensaje base que se mostrará si ocurre una excepción.
    /// </param>
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

    /// <summary>
    /// Ejecuta una acción asincrónica que retorna un valor,
    /// devolviendo un valor por defecto si ocurre un error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor retornado.</typeparam>
    /// <param name="action">Acción asincrónica a ejecutar.</param>
    /// <param name="dialogService">
    /// Servicio encargado de mostrar diálogos de error al usuario.
    /// </param>
    /// <param name="mensajeError">
    /// Mensaje base que se mostrará si ocurre una excepción.
    /// </param>
    /// <param name="defaultValue">
    /// Valor que se devolverá si ocurre una excepción durante la ejecución.
    /// </param>
    /// <returns>
    /// El resultado de la acción si se ejecuta correctamente,
    /// o <paramref name="defaultValue"/> en caso de error.
    /// </returns>
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

    /// <summary>
    /// Ejecuta una acción asincrónica que retorna una lista,
    /// controlando si debe ejecutarse en el hilo de UI o en background.
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos de la lista.</typeparam>
    /// <param name="action">Acción asincrónica a ejecutar.</param>
    /// <param name="dialogService">
    /// Servicio encargado de mostrar diálogos de error al usuario.
    /// </param>
    /// <param name="mensajeError">
    /// Mensaje base que se mostrará si ocurre una excepción.
    /// </param>
    /// <returns>
    /// La lista resultante de la acción,
    /// o una lista vacía si ocurre un error.
    /// </returns>
    /// <remarks>
    /// Si el método no está marcado con <see cref="UiSafeAttribute"/>
    /// y se ejecuta desde el hilo de UI, se registra una advertencia
    /// indicando una ejecución potencialmente peligrosa.
    /// </remarks>
    public static async Task<List<T>> RunAsyncList<T>(
        Func<Task<List<T>>> action,
        IDialogService dialogService,
        string mensajeError)
    {
        try
        {
            var method = action.Method;
            var isUiThread = Application.Current.Dispatcher.CheckAccess();
            var isUiSafe = method.IsDefined(typeof(UiSafeAttribute), true);

            if (isUiThread && !isUiSafe)
            {
                // LOG EARLY WARNING
                System.Diagnostics.Debug.WriteLine(
                    $"[UiDanger] {method.Name} ejecutado en UI Thread");
            }

            return isUiSafe
                ? await action()           // corre en UI
                : await Task.Run(action);  // corre en background
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