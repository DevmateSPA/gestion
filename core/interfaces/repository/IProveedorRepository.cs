using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IProveedorRepository : IBaseRepository<Proveedor>
{
    Task<bool> ExisteRut(string rut, long empresaId);
}