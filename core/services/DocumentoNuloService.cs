using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class DocumentoNuloService : BaseService<DocumentoNulo>, IDocumentoNuloService
{
    private readonly IDocumentoNuloRepository _documentoNuloRepository;
    public DocumentoNuloService(IDocumentoNuloRepository documentoNuloRepository)
        :base(documentoNuloRepository)
    {
        _documentoNuloRepository = documentoNuloRepository;
    }

    protected override IEnumerable<IReglaNegocio<DocumentoNulo>> DefinirReglas(
        DocumentoNulo entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<DocumentoNulo>(
                d => d.Folio,
                "El folio del documento es obligatorio."),

            new UnicoRegla<DocumentoNulo>(
                existe: (dn, id) =>
                    _documentoNuloRepository.ExisteFolio(
                        dn.Folio,
                        dn.Empresa,
                        id),

                valor: dn => dn.Folio,

                mensaje: "El folio del documento: {0}, ya existe para la empresa actual.")
        ];
    }
}