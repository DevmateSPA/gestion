using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace Gestion.Infrastructure.Logging;

public static class SqlLogger
{
    private const int UiWarningThresholdMs = 200;

    public static async Task<TResult> LogAsync<TResult>(
        string operation,
        string sql,
        Func<Task<TResult>> action,
        Func<TResult, int>? countSelector = null)
    {
        var sw = Stopwatch.StartNew();

        bool isUiThread = Application.Current?.Dispatcher.CheckAccess() ?? false;
        int threadId = Environment.CurrentManagedThreadId;

        Log.Debug(
            "SQL START | {Operation} | UI={UI} | Thread={Thread}\n{Sql}",
            operation,
            isUiThread,
            threadId,
            sql);

        try
        {
            var result = await action();

            sw.Stop();

            if (countSelector != null)
            {
                Log.Information(
                    "SQL END | {Operation} | {Count} rows | {Elapsed}ms | UI={UI}\n",
                    operation,
                    countSelector(result),
                    sw.ElapsedMilliseconds,
                    isUiThread);
            }
            else
            {
                Log.Information(
                    "SQL END | {Operation} | {Elapsed}ms | UI={UI}\n",
                    operation,
                    sw.ElapsedMilliseconds,
                    isUiThread);
            }

            // Advertencia si bloquea UI
            if (isUiThread && sw.ElapsedMilliseconds > UiWarningThresholdMs)
            {
                Log.Warning(
                    "UI BLOCKING SQL | {Operation} | {Elapsed}ms",
                    operation,
                    sw.ElapsedMilliseconds);
            }

            // Advertencia si no hay LIMIT
            if (!sql.Contains("LIMIT", StringComparison.OrdinalIgnoreCase))
            {
                Log.Warning(
                    "SQL WITHOUT LIMIT | {Operation}",
                    operation);
            }

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();

            Log.Error(
                ex,
                "SQL ERROR | {Operation} | {Elapsed}ms\n{Sql}",
                operation,
                sw.ElapsedMilliseconds,
                sql);

            throw;
        }
    }
}