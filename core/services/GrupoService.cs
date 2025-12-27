using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class GrupoService : BaseService<Grupo>, IGrupoService
{
    private readonly IGrupoRepository _grupoRepository;
    public GrupoService(IGrupoRepository grupoRepository)
        :base(grupoRepository)
    {
        _grupoRepository = grupoRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Grupo entity,
        long? excludeId = null)
    {
        List<string> errores = [];

        if (await _grupoRepository.ExisteCodigo(
                codigo: entity.Codigo,
                empresaId: entity.Empresa,
                excludeId: excludeId))
        {
            errores.Add($"El código del grupo: {entity.Codigo}, ya existe para la empresa actual.");
        }

        if (string.IsNullOrWhiteSpace(entity.Descripcion))
            errores.Add("La descripción del grupo es obligatorio.");

        return errores;
    }
}