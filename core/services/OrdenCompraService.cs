using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class OrdenTrabajoService : BaseService<OrdenTrabajo>, IOrdenTrabajoService
{
    public OrdenTrabajoService(IOrdenTrabajoRepository ordenTrabajoRepository)
        :base(ordenTrabajoRepository) { }
}