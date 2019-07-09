using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBCrud.Core.Models
{
    [DynamoDBTable("clientes")]
    public class Cliente
    {
        [DynamoDBProperty("email")]
        public string Email { get; set; }

        [DynamoDBProperty("nome")]
        public string Nome { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey]
        [DynamoDBProperty("sobrenome")]
        public string Sobrenome { get; set; }

    }

}