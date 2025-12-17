using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IMaquinaService : IBaseService<Maquina>
{
    Task<List<Maquina>> FindMaquinaWithPendingOrders(long empresaId);
}