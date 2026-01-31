namespace Gestion.core.interfaces.lookup;

public interface ILookupResolver
{
    Task ResolveAsync(object entidad, string propertyName);
}
