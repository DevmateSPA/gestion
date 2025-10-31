using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class OperadorService : BaseService<Operador>, IOperadorService
{
    public OperadorService(IOperadorRepository operadorRepository)
        :base(operadorRepository) { }
}