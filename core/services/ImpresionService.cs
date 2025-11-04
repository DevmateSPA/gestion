using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ImpresionService : BaseService<Impresion>, IImpresionService
{
    public ImpresionService(IImpresionRepository impresionRepository)
        :base(impresionRepository) { }
}