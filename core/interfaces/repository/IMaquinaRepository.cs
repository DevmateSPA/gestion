using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IMaquinaRepository: IBaseRepository<Maquina>
{
    Task<List<Maquina>> FindMaquinaWithPendingOrders(long empresaId);
}