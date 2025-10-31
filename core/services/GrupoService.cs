using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class GrupoService : BaseService<Grupo>, IGrupoService
{
    public GrupoService(IGrupoRepository grupoRepository)
        :base(grupoRepository) { }
}