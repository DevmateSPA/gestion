using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class BancoService : BaseService<Banco>, IBancoService
{
    public BancoService(IBancoRepository bancoRepository)
        :base(bancoRepository) { }
}