using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ImpresionService : BaseService<Impresion>, IImpresionService
{
    private readonly IImpresionRepository _impresionRepository;
    public ImpresionService(IImpresionRepository impresionRepository)
        :base(impresionRepository)
    {
        _impresionRepository = impresionRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Impresion entity,
        long? excludeId = null)
    {
        List<string> errores = [];

        if (await _impresionRepository.ExisteCodigo(
                codigo: entity.Codigo,
                empresaId: entity.Empresa,
                excludeId: excludeId))
        {
            errores.Add($"El código de la impresión: {entity.Codigo}, ya existe para la empresa actual.");
        }

        if (string.IsNullOrWhiteSpace(entity.Descripcion))
            errores.Add("La descripción de la impresión es obligatoria.");

        return errores;
    }
}