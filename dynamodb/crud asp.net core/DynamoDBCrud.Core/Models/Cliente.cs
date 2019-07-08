using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBCrud.Core.Models
{
    [DynamoDBTable("clientes")]
    public class Cliente
    {
        public string email { get; set; }
        public string nome { get; set; }

    }

}