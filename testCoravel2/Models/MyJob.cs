using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace testCoravel2.Models
{
    public class MyJob : IInvocable
    {
        private readonly ILogger<MyJob> _logger;

        public MyJob(ILogger<MyJob> logger)
        {
            _logger = logger;
        }

        public async Task Invoke()
        {
            await Task.Delay(500);
            _logger.LogInformation($"job running - {DateTime.Now}");
        }
    }
}