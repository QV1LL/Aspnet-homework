using System.Data;
using Dapper;

namespace TestsSample.Core.Infrastructure.Persistence.Mappers;

public class SqliteGuidHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value) => 
        parameter.Value = value.ToString();

    public override Guid Parse(object value) => 
        Guid.Parse((string)value);
}