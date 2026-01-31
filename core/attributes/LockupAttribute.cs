using System;

namespace Gestion.core.attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class LookupAttribute : Attribute
{
    public Type ServiceType { get; }
    public string MethodName { get; }
    public string[] Map { get; }

    public bool ClearOnNull { get; init; } = true;

    public LookupAttribute(
        Type serviceType,
        string methodName,
        params string[] map)
    {
        ServiceType = serviceType;
        MethodName = methodName;
        Map = map;
    }
}