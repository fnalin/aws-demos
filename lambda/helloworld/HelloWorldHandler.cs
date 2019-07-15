using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

namespace HelloWorld
{
    public class HelloWorldHandler
    {

        [LambdaSerializer(typeof(JsonSerializer))]
        public async Task<Result> Handle(Request request)
        {
            return await
                Task.Run(()=>new Result{ Message = $"Olá {request.Name} from Lambda"});
        }

    }
}