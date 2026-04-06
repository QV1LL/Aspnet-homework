using System.Data;
using System.Reflection;
using Dapper;
using Microsoft.Data.Sqlite;
using TestsSample.Core.Infrastructure.Persistence.Mappers;
using TestsSample.Core.Infrastructure.Services;
using TestsSample.Core.Models;

namespace TestsSample.Tests.Infrastructure.Persistence.Repositories;

public class DbConnectionFactoryFixture : IDisposable
{
    public IDbConnection Connection { get; }
    public Mock<IDbConnectionFactory> FactoryMock { get; }

    public DbConnectionFactoryFixture()
    {
        Connection = new SqliteConnection("Data Source=:memory:");
        Connection.Open();

        Connection.Execute(@"
            CREATE TABLE Users (
                Id GUID PRIMARY KEY,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                Age INTEGER NOT NULL,
                HashedPassword TEXT NOT NULL
            )");
        
        SqlMapper.AddTypeHandler(new SqliteGuidHandler());

        FactoryMock = new Mock<IDbConnectionFactory>();
        FactoryMock.Setup(f => f.CreateConnection()).Returns(Connection);
    }

    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();
    }
}