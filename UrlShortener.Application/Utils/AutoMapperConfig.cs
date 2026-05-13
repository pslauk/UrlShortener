using AutoMapper;
using UrlShortener.Application.Models;
using UrlShortener.DomainModel.Entities;

namespace UrlShortener.Application.Utils
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Url, UrlModel>();
        }
    }
}
