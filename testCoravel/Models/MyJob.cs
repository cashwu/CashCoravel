using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace testCoravel.Models
{
    public class MyJob : IInvocable
    {
        private readonly ILogger<MyJob> _logger;

        public MyJob(ILogger<MyJob> logger)
        {
            _logger = logger;
        }
        
        public Task Invoke()
        {
            _logger.LogInformation("job running");
            
            return Task.CompletedTask;
        }
    }
}