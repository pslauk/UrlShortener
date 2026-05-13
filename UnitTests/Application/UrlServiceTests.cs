using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Services;
using UrlShortener.Application.Utils;
using UrlShortener.DomainModel.Entities;
using UrlShortener.DomainModel.Interfaces;
using Xunit;

namespace UnitTests.Application
{
    public class UrlServiceTests
    {
        private const string _testShortedUrl = "rt12ubI";

        [Fact]
        public void GetUrlViewModelShouldReturnOneEmptyUrl()
        {
            var repositoryMock = new Mock<IRepository<Url>>();
            repositoryMock.Setup(repo => repo.GetAll(null)).Returns(GetTestUrls());
            var service = GetUrlService(repositoryMock);

            var result = service.GetUrlViewModel();

            Assert.Contains(result, url => url.Id == null);
        }

        [Fact]
        public void TryGetUrlShouldReturnFalseIfNotFound()
        {
            var repositoryMock = new Mock<IRepository<Url>>();
            repositoryMock.Setup(repo => repo.Find(url => url.ShortedUrl.Contains(""), null)).Returns(new List<Url>());
            var service = GetUrlService(repositoryMock);

            var result = service.TryGetUrl("", out var url);

            Assert.Null(url);
            Assert.False(result);
        }

        [Fact]
        public void TryGetUrlShouldReturnTrueIfFound()
        {
            var repositoryMock = new Mock<IRepository<Url>>();
            repositoryMock.Setup(repo => repo.Find(url => url.ShortedUrl.Contains(_testShortedUrl), null))
                .Returns(GetTestUrls().Where(url => url.ShortedUrl.Contains(_testShortedUrl)).ToList());

            var service = GetUrlService(repositoryMock);

            var result = service.TryGetUrl(_testShortedUrl, out var url);

            Assert.True(url.ShortedUrl == _testShortedUrl);
            Assert.True(result);
        }

        private IUrlService GetUrlService(Mock<IRepository<Url>> repositoryMock)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            var mockMapper = mapperConfig.CreateMapper();
            var workerMock = new Mock<IContextWorker>();
            workerMock.Setup(worker => worker.Commit());

            return new UrlService(workerMock.Object, repositoryMock.Object, mockMapper);
        }

        private List<Url> GetTestUrls()
        {
            var urls = new List<Url>
            {
                new Url { Id = 1, UserUrl = "https://myurl.com", ShortedUrl = _testShortedUrl, Clics = 5 },
                new Url { Id = 2, UserUrl = "http://TEST.ua", ShortedUrl = "15Tre7F", Clics = 0 }
            };
            
            return urls;
        }
    }
}
