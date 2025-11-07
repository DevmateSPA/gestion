using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class NotaCreditoService : BaseService<NotaCredito>, INotaCreditoService
{
    public NotaCreditoService(INotaCreditoRepository notaCreditoRepository)
        :base(notaCreditoRepository) { }
}