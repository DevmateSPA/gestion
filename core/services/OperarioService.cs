using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class OperarioService : BaseService<Operario>, IOperarioService
{
    private readonly IOperarioRepository _operarioRepository;
    public OperarioService(IOperarioRepository operarioRepository)
        :base(operarioRepository)
    {
        _operarioRepository = operarioRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Operario entity,
        long? excludeId = null)
    {
        List<string> errores = [];

        if (await _operarioRepository.ExisteCodigo(
                codigo: entity.Codigo,
                empresaId: entity.Empresa,
                excludeId: excludeId))
        {
            errores.Add($"El código del operario: {entity.Codigo}, ya existe para la empresa actual.");
        }

        if (string.IsNullOrWhiteSpace(entity.Nombre))
            errores.Add("El nombre del operario es obligatorio.");

        return errores;
    }
}