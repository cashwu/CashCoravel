using Coravel.Events.Interfaces;
using testCoravel.Controllers;

namespace testCoravel.Models
{
    public class BlogPostCreated : IEvent
    {
        public Post Post { get; set; }

        public BlogPostCreated(Post post)
        {
            Post = post;
        }
    }
}