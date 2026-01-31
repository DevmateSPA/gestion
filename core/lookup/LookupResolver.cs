using System.Diagnostics;
using System.Reflection;
using Gestion.core.attributes;
using Gestion.core.interfaces.lookup;
using Gestion.core.session;

namespace Gestion.core.lookup;

public sealed class LookupResolver : ILookupResolver
{
    private readonly IServiceProvider _provider;

    private bool IsDebuging = false;

    public LookupResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    private void Debug(string msg)
    {
        if (IsDebuging)
            Debugger.Log(0, "LOOKUP", $"[LOOKUP] {msg}\n");
    }

    public async Task ResolveAsync(object entidad, string propertyName)
    {
        Debug($"ResolveAsync → entidad={entidad.GetType().Name}, prop={propertyName}");

        var prop = entidad.GetType().GetProperty(propertyName);
        if (prop == null)
        {
            Debug("Propiedad no encontrada");
            return;
        }

        var lookups = prop.GetCustomAttributes<LookupAttribute>().ToArray();
        Debug($"Lookups encontrados: {lookups.Length}");

        if (lookups.Length == 0)
            return;

        var keyValue = prop.GetValue(entidad)?.ToString();
        Debug($"Valor clave: '{keyValue}'");

        if (string.IsNullOrWhiteSpace(keyValue))
            return;

        foreach (var lookup in lookups)
        {
            Debug($"Ejecutando lookup → {lookup.MethodName}");
            await EjecutarLookup(entidad, lookup, keyValue);
        }
    }

    private async Task EjecutarLookup(
        object entidad,
        LookupAttribute lookup,
        string keyValue)
    {
        Debug($"EjecutarLookup → Service={lookup.ServiceType.Name}");

        var service = _provider.GetService(lookup.ServiceType);
        if (service == null)
        {
            Debug("Servicio NULL");
            return;
        }

        Debug($"Servicio OK → {service.GetType().Name}");

        var method = lookup.ServiceType.GetMethod(lookup.MethodName);
        if (method == null)
        {
            Debug("Método NO encontrado");
            return;
        }

        Debug($"Método OK → {method.Name}");

        var task = (Task)method.Invoke(
            service,
            [keyValue, SesionApp.IdEmpresa]
        )!;

        Debug($"Invoke OK → TaskType={task.GetType().Name}");

        await task;

        var resultProp = task.GetType().GetProperty("Result");
        Debug($"Result property existe: {resultProp != null}");

        var result = resultProp?.GetValue(task);

        Debug(result == null
            ? "Lookup devolvió NULL"
            : $"Lookup OK → ResultType={result.GetType().Name}");

        if (result == null)
        {
            if (lookup.ClearOnNull)
                LimpiarCampos(entidad, lookup.Map);
            return;
        }

        AplicarMapeo(entidad, result, lookup.Map);
    }

    private void AplicarMapeo(object target, object source, string[] map)
    {
        Debug($"AplicarMapeo → target={target.GetType().Name}");

        foreach (var m in map)
        {
            Debug($"Map: {m}");

            var parts = m.Split(':');
            if (parts.Length != 2)
            {
                Debug("Map mal formado");
                continue;
            }

            var from = source.GetType().GetProperty(parts[0]);
            var to = target.GetType().GetProperty(parts[1]);

            Debug($"FromProp={from?.Name ?? "NULL"} → ToProp={to?.Name ?? "NULL"}");

            if (from != null && to != null && to.CanWrite)
            {
                var value = from.GetValue(source);
                Debug($"Asignando {to.Name} = {value}");

                to.SetValue(target, value);
            }
            else
            {
                Debug("No se pudo asignar");
            }
        }
    }

    private void LimpiarCampos(object target, string[] map)
    {
        foreach (var m in map)
        {
            var dest = m.Split(':')[1];
            var prop = target.GetType().GetProperty(dest);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(target, null);
            }
        }
    }
}
