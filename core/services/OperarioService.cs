using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class OperarioService : BaseService<Operario>, IOperarioService
{
    public OperarioService(IOperarioRepository operarioRepository)
        :base(operarioRepository) { }
}