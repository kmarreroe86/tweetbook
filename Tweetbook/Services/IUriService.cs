using System;
using Tweetbook.Contracts.V1.Requests.Queries;

namespace Tweetbook.Services
{
    public interface IUriService
    {

        Uri GetPostUri(string postId);

        Uri GetAllPost(PaginationQuery paginationQuery = null);
    }
}