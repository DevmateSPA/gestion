using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IProductoRepository : IBaseRepository<Producto>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId, long? excludeId = null);
}