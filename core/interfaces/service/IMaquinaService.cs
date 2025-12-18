using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IMaquinaService : IBaseService<Maquina>
{
    Task<List<Maquina>> FindAllMaquinaWithPendingOrders(long empresaId);
    Task<List<Maquina>> FindPageMaquinaWithPendingOrders(long empresaId, int pageNumber, int pageSize);
    Task<long> ContarMaquinasConPendientes(long empresaId);
}