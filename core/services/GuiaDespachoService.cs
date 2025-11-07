using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class GuiaDespachoService : BaseService<GuiaDespacho>, IGuiaDespachoService
{
    public GuiaDespachoService(IGuiaDespachoRepository guiaDespachoRepository)
        :base(guiaDespachoRepository) { }
}