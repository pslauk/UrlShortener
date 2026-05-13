using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Models;
using UrlShortener.DomainModel.Entities;
using UrlShortener.DomainModel.Interfaces;

namespace UrlShortener.Application.Services
{
    public class UrlService : IUrlService
    {
        const string charsForRandomUrl = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly IContextWorker _contextWorker;
        private readonly IRepository<Url> _urlsRepository;
        private readonly IMapper _mapper;

        public UrlService(IContextWorker contextWorker, IRepository<Url> urlsRepository, IMapper mapper)
        {
            _contextWorker = contextWorker;
            _urlsRepository = urlsRepository;
            _mapper = mapper;
        }

        public IEnumerable<UrlModel> GetUrlViewModel()
        {
            var urlViewModelList = _mapper.Map<IEnumerable<Url>, List<UrlModel>>(_urlsRepository.GetAll());
            urlViewModelList.Add(new UrlModel { Id = null });

            return urlViewModelList;
        }

        public void CreateUrl(string newUserUrl)
        {
            var newUrl = new Url { UserUrl = newUserUrl, Clics = 0 };
            newUrl.ShortedUrl = CreateShortedUrl();
            _urlsRepository.Add(newUrl);

            _contextWorker.Commit();
        }

        public bool TryGetUrl(string shortedUrlPart, out Url url)
        {
            var urls = _urlsRepository.Find(url => url.ShortedUrl.Contains(shortedUrlPart));
            url = urls.FirstOrDefault();

            return urls.Any();
        }

        public void UpdateClickedUrl(Url url)
        {
            url.Clics++;
            _urlsRepository.Update(url);

            _contextWorker.Commit();
        }

        private string CreateShortedUrl()
        {
            string shortedUrl;

            do
            {
                shortedUrl = GetRandomString();
            }
            while (TryGetUrl(shortedUrl, out _));

            return shortedUrl;
        }

        private string GetRandomString()
        {
            Random random = new Random();

            return new string(Enumerable.Repeat(charsForRandomUrl, 7)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
