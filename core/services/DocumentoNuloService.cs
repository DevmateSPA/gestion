using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class DocumentoNuloService : BaseService<DocumentoNulo>, IDocumentoNuloService
{
    private readonly IDocumentoNuloRepository _documentoNuloRepository;
    public DocumentoNuloService(IDocumentoNuloRepository documentoNuloRepository)
        :base(documentoNuloRepository)
    {
        _documentoNuloRepository = documentoNuloRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(DocumentoNulo entity)
    {
        List<string> erroresEncontrados = [];

        if (await _documentoNuloRepository.ExisteFolio(folio: entity.Folio, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El folio del documento: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio del documento es obligatorio.");

        return erroresEncontrados;
    }
}