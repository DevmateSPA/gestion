using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IFacturaService : IBaseService<Factura>
{
    Task<List<Factura>> FindAllByEmpresa(long empresaId);
}