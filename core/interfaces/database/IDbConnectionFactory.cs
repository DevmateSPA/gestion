using System.Data;

namespace Gestion.core.interfaces.database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnection();
}