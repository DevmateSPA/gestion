using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class EncuadernacionService : BaseService<Encuadernacion>, IEncuadernacionService
{
    public EncuadernacionService(IEncuadernacionRepository encuadernacionRepository)
        :base(encuadernacionRepository) { }
}