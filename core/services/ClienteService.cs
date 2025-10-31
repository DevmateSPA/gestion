using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ClienteService : BaseService<Cliente>, IClienteService
{
    public ClienteService(IClienteRepository clienteRepository)
        :base(clienteRepository) { }
}