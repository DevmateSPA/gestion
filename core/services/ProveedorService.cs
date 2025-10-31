using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ProveedorService : BaseService<Proveedor>, IProveedorService
{
    public ProveedorService(IProveedorRepository proveedorRepository)
        :base(proveedorRepository) { }
}