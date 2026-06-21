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

        [Fact]
        public void DeleteUrlShouldCallRepositoryDeleteAndCommit()
        {
            var testUrl = GetTestUrls().First();
            var repositoryMock = new Mock<IRepository<Url>>();
            repositoryMock.Setup(repo => repo.GetById(testUrl.Id)).Returns(testUrl);
            var workerMock = new Mock<IContextWorker>();
            workerMock.Setup(worker => worker.Commit());
            var service = GetUrlService(repositoryMock, workerMock);

            service.DeleteUrl(testUrl.Id);

            repositoryMock.Verify(repo => repo.GetById(testUrl.Id), Times.Once);
            repositoryMock.Verify(repo => repo.Delete(testUrl), Times.Once);
            workerMock.Verify(worker => worker.Commit(), Times.Once);
        }

        [Fact]
        public void DeleteUrlShouldDoNothingIfUrlNotFound()
        {
            var repositoryMock = new Mock<IRepository<Url>>();
            repositoryMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns((Url)null);
            var workerMock = new Mock<IContextWorker>();
            var service = GetUrlService(repositoryMock, workerMock);

            service.DeleteUrl(999);

            repositoryMock.Verify(repo => repo.Delete(It.IsAny<Url>()), Times.Never);
            workerMock.Verify(worker => worker.Commit(), Times.Never);
        }

        private IUrlService GetUrlService(Mock<IRepository<Url>> repositoryMock)
        {
            return GetUrlService(repositoryMock, new Mock<IContextWorker>());
        }

        private IUrlService GetUrlService(Mock<IRepository<Url>> repositoryMock, Mock<IContextWorker> workerMock)
        {
            var logger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { });
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            }, logger);
            var mockMapper = mapperConfig.CreateMapper();
            workerMock.Setup(worker => worker.Commit());

            return new UrlService(workerMock.Object, repositoryMock.Object, mockMapper);
        }

        private List<Url> GetTestUrls()
        {
            var urls = new List<Url>
            {
                new Url { Id = 1, UserUrl = "https://myurl.com", ShortedUrl = _testShortedUrl, Clicks = 5 },
                new Url { Id = 2, UserUrl = "http://TEST.ua", ShortedUrl = "15Tre7F", Clicks = 0 }
            };
            
            return urls;
        }
    }
}
