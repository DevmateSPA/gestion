using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FotomecanicaService : BaseService<Fotomecanica>, IFotomecanicaService
{
    public FotomecanicaService(IFotomecanicaRepository fotomecanicaRepository)
        :base(fotomecanicaRepository) { }
}