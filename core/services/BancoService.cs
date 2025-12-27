using Gestion.core.exceptions;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class BancoService : BaseService<Banco>, IBancoService
{
    private readonly IBancoRepository _bancoRepository;
    public BancoService(IBancoRepository bancoRepository)
        :base(bancoRepository)
    {
        _bancoRepository = bancoRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Banco entity,
        long? excludeId = null)
    {
        List<string> errores = [];

        if (await _bancoRepository.ExisteCodigo(
                codigo: entity.Codigo,
                empresaId: entity.Empresa,
                excludeId: excludeId))
        {
            errores.Add($"El código del banco: {entity.Codigo}, ya existe para la empresa actual.");
        }

        if (string.IsNullOrWhiteSpace(entity.Nombre))
            errores.Add("El nombre del banco es obligatorio.");

        return errores;
    }

}