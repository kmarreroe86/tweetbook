namespace Tweetbook.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;
        
        
        public static class Posts
        {
            public const string GetAll = Base + "/posts";
            public const string Get = Base + "/post/{postId}";
            public const string Update = Base + "/post/{postId}";
            public const string Delete = Base + "/post/{postId}";
            public const string Create = Base + "/post";
            
        }

        public static class Tags
        {
            public const string GetAll = Base + "/tags";

            public const string Get = Base + "/tags/{tagName}";

            public const string Create = Base + "/tags";

            public const string Delete = Base + "/tags/{tagName}";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";
        }
    }
}