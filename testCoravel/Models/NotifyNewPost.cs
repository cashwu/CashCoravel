using System.Threading;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.Logging;

namespace testCoravel.Models
{
    public class NotifyNewPost : IListener<BlogPostCreated>
    {
        private readonly ILogger<NotifyNewPost> _logger;

        public NotifyNewPost(ILogger<NotifyNewPost> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(BlogPostCreated blogPostCreated)
        {
            _logger.LogInformation("NotifyNewPost start");
            Thread.Sleep(2000);
            _logger.LogInformation($"NotifyNewPost - {blogPostCreated.Post.Id}");

            return Task.CompletedTask;
        }
    }
}