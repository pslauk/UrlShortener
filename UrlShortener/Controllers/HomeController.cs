using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUrlService _urlService;

        public HomeController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "UrlShortener");
        }

        [Route("{shortedUrlPart::maxlength(7)}")]
        public IActionResult UserShortedUrl(string shortedUrlPart)
        {
            if (!_urlService.TryGetUrl(shortedUrlPart, out var url)) return RedirectToAction("Index", "UrlShortener");

            _urlService.UpdateClickedUrl(url);

            return Redirect(url.UserUrl);
        }
    }
}
