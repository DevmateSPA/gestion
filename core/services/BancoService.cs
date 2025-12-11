using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class BancoService : BaseService<Banco>, IBancoService
{
    private readonly IBancoRepository _bancoRepository;
    public BancoService(IBancoRepository bancoRepository)
        :base(bancoRepository)
    {
        _bancoRepository = bancoRepository;
    }
}