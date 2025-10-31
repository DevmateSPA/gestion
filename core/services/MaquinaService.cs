using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class MaquinaService : BaseService<Maquina>, IMaquinaService
{
    public MaquinaService(IMaquinaRepository maquinaRepository)
        :base(maquinaRepository) { }
}