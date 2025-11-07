using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class DocumentoNuloService : BaseService<DocumentoNulo>, IDocumentoNuloService
{
    public DocumentoNuloService(IDocumentoNuloRepository documentoNuloRepository)
        :base(documentoNuloRepository) { }
}