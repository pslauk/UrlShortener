using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using UrlShortener.Application.Models;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Utils.Extensions;

namespace UrlShortener.Controllers
{
    public class UrlShortenerController : Controller
    {
        private readonly IUrlService _urlService;

        public UrlShortenerController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        public IActionResult Index()
        {
            return View(_urlService.GetUrlViewModel());
        }

        [HttpPost]
        public ActionResult ShortUrl(List<UrlModel> urls)
        {
            var newUserUrl = urls.FirstOrDefault(url => url.Id == null).UserUrl;

            if (newUserUrl.IsUrl())
            {
                _urlService.CreateUrl(newUserUrl);
            }

            return PartialView("UrlsPartial", _urlService.GetUrlViewModel());
        }
    }
}