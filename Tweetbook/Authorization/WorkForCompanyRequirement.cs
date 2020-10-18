using Microsoft.AspNetCore.Authorization;

namespace Tweetbook.Authorization
{
    public class WorkForCompanyRequirement : IAuthorizationRequirement
    {
        public string DomainName { get; }

        public WorkForCompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }
    }
}