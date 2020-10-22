namespace Tweetbook.Contracts.V1.Requests.Queries
{
    public class PaginationQuery
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }


        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public PaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize > 100 ? 100 : pageSize;
        }
    }
}