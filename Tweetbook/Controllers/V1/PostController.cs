using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        private readonly IMapper _mapper;

        public PostController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();

            /* Commented due AutoMapper use
             var postResponse = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                Name = p.Name,
                UserId = p.UserId,
                Tags = p.Tags.Select(t => new TagResponse {Name = t.TagName})
            }).ToList();*/

            return Ok(_mapper.Map<List<PostResponse>>(posts));
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null) return NotFound();

            return Ok(_mapper.Map<PostResponse>(post));
            /*return Ok(new PostResponse
            {
                Name = post.Name,
                Id = post.Id,
                UserId = post.UserId,
                Tags = post.Tags.Select(t => new TagResponse {Name = t.TagName}).ToList()
            });*/
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var newPostId = Guid.NewGuid();
            var postModel = new Post()
            {
                Id = newPostId,
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(t => new PostTag {PostId = newPostId, TagName = t}).ToList()
            };
            await _postService.CreatePostAsync(postModel);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", postModel.Id.ToString());
            /*var response = new PostResponse
            {
                Id = postModel.Id,
                Name = postModel.Name,
                UserId = postModel.UserId,
                Tags = postModel.Tags.Select(t => new TagResponse {Name = t.TagName})
            };*/

            return Created(location, _mapper.Map<PostResponse>(postModel));
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest postRequest)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnsPost) return BadRequest(new {Error = "Yo do not own this post"});

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = postRequest.Name;

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
                return Ok(_mapper.Map<PostResponse>(post));
            /*return Ok(new PostResponse
            {
                Name = post.Name,
                Id = post.Id,
                UserId = post.UserId,
                Tags = post.Tags.Select(t => new TagResponse {Name = t.TagName})
            });*/

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnsPost) return BadRequest(new {Error = "Yo do not own this post"});

            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted) return NoContent();

            return NotFound();
        }
    }
}