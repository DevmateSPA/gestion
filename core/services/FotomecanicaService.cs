using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FotomecanicaService : BaseService<Fotomecanica>, IFotomecanicaService
{
    private readonly IFotomecanicaRepository _fotomecanicaRepository;
    public FotomecanicaService(IFotomecanicaRepository fotomecanicaRepository)
        :base(fotomecanicaRepository)
    {
        _fotomecanicaRepository = fotomecanicaRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(Fotomecanica entity)
    {
        List<string> erroresEncontrados = [];

        if (await _fotomecanicaRepository.ExisteCodigo(codigo: entity.Codigo, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El código de la fotomecánica: {entity.Codigo}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Descripcion))
            erroresEncontrados.Add("La descripción de la fotomecánica es obligatoria.");

        return erroresEncontrados;
    }
}