using System.Collections.Generic;
using UrlShortener.Application.Models;
using UrlShortener.DomainModel.Entities;

namespace UrlShortener.Application.Interfaces
{
    public interface IUrlService
    {
        IEnumerable<UrlModel> GetUrlViewModel();

        void CreateUrl(string newUserUrl);

        bool TryGetUrl(string shortedUrlPart, out Url url);

        void UpdateClickedUrl(Url url);
    }
}
