using System.Data;

namespace TestsSample.Core.Infrastructure.Services;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}