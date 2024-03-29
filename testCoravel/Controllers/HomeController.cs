﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using testCoravel.Models;

namespace testCoravel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDispatcher _dispatcher;
        private readonly IQueue _queue;

        public HomeController(ILogger<HomeController> logger, IDispatcher dispatcher, IQueue queue)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _queue = queue;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var post = new Post();
            _logger.LogInformation($"Create - {post.Id}");
            var blogPostCreated = new BlogPostCreated(post);
            await _dispatcher.Broadcast(blogPostCreated);
            _logger.LogInformation($"Broadcast - {post.Id}");

            return Ok();
        }

        public IActionResult Queue()
        {
            var post = new Post();
            _logger.LogInformation($"Create - {post.Id}");
            var blogPostCreated = new BlogPostCreated(post);
            _queue.QueueBroadcast(blogPostCreated);
            _logger.LogInformation($"Queue - {post.Id}");

            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }

    public class Post
    {
        public Post()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}