using System.Linq;
using AutoMapper;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.Tags.Select(t => new TagResponse {Name = t.TagName})));

            CreateMap<Tag, TagResponse>();

            // CreateMap<PostTag, TagResponse>();
        }
    }
}