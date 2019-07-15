using System;
using Fansoft.CodEstruturado.Lib;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

namespace Fansoft.CodEstruturado.Core
{
    public class Handler
    {
        [LambdaSerializer(typeof(JsonSerializer))]
        public void Handle(Request request, ILambdaContext context)
        {
            context.Logger.LogLine($"Tempo restante da Lambda: {context.RemainingTime}");
            context.Logger.LogLine($"Tempo restante da Lambda: {context.FunctionName}");
            context.Logger.LogLine($"Dados recebidos: Id: {request.Id} - Nome: {request.Nome} - Idade: {request.Idade}");
            context.Logger.LogLine($"Tempo restante da Lambda: {context.RemainingTime}");
        }
    }
}
