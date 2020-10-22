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
using Tweetbook.Contracts.V1.Requests.Queries;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Data.Migrations;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Filters;
using Tweetbook.Helpers;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUriService _uriService;

        private readonly IMapper _mapper;

        public PostController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostQuery query, [FromQuery] PaginationQuery paginationQuery)
        {

            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllPostFilter>(query);
            var posts = await _postService.GetPostsAsync(filter, pagination);

            /* Commented due AutoMapper use
             var postResponse = posts.Select(p => new PostResponse
            {
                Id = p.Id,
                Name = p.Name,
                UserId = p.UserId,
                Tags = p.Tags.Select(t => new TagResponse {Name = t.TagName})
            }).ToList();*/

            var postResponse = _mapper.Map<List<PostResponse>>(posts);
            var paginationResponse = PaginationHelper.CreatePaginatedResponse(_uriService, pagination, postResponse);
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null) return NotFound();

            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
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

            // var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            // var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", postModel.Id.ToString());
            var locationUri = _uriService.GetPostUri(postModel.Id.ToString());

            return Created(locationUri, new Response<PostResponse>(_mapper.Map<PostResponse>(postModel)));
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
                return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
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