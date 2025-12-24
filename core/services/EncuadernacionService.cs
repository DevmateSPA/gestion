using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class EncuadernacionService : BaseService<Encuadernacion>, IEncuadernacionService
{
    private readonly IEncuadernacionRepository _encuadernacionRepository;
    public EncuadernacionService(IEncuadernacionRepository encuadernacionRepository)
        :base(encuadernacionRepository)
    {
        _encuadernacionRepository = encuadernacionRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(Encuadernacion entity)
    {
        List<string> erroresEncontrados = [];

        if (await _encuadernacionRepository.ExisteCodigo(codigo: entity.Codigo, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El código de la encuadernación: {entity.Codigo}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Descripcion))
            erroresEncontrados.Add("La descripción de la encuadernación es obligatoria.");

        return erroresEncontrados;
    }
}