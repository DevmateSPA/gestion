using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ProveedorService : BaseService<Proveedor>, IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    public ProveedorService(IProveedorRepository proveedorRepository)
        :base(proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(Proveedor entity)
    {
        List<string> erroresEncontrados = [];

        if (await _proveedorRepository.ExisteRut(rut: entity.Rut, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El rut del proveedor: {entity.Rut}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Rut))
            erroresEncontrados.Add("El rut del proveedor es obligatorio.");

        return erroresEncontrados;
    }
}