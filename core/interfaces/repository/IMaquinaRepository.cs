using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IMaquinaRepository: IBaseRepository<Maquina>
{
    Task<List<Maquina>> FindAllMaquinaWithPendingOrders(long empresaId);
    Task<List<Maquina>> FindPageMaquinaWithPendingOrders(long empresaId, int pageNumber, int pageSize);
    Task<long> ContarMaquinasConPendientes(long empresaId);
    Task<bool> ExisteCodigo(string codigo, long empresaId, long? excludeId = null);
}