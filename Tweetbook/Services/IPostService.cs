﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(GetAllPostFilter filter = null, PaginationFilter paginationFilter = null);

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> CreatePostAsync(Post post);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);
        
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);

        Task<List<Tag>> GetAllTagsAsync();
        
        Task<Tag> GetTagByNameAsync(string tagName);

        Task<bool> CreateTAgAsync(Tag tag);

        Task<bool> DeleteTagAsync(string tagName);
    }
}