using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostApi.Application.Interfaces;

namespace PostApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPost _postService;

        public PostController(IPost postService)
        {
            _postService = postService;
        }
    }
    a
}
