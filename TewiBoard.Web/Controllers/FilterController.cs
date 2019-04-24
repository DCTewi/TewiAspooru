using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TewiBoard.Web.Repositories;

namespace TewiBoard.Web.Controllers
{
    public class FilterController : DbController
    {
        public FilterController(CardDb db, ILogger<DbController> logger) : base(db, logger) { }

        public IActionResult Index(string genre, string module, int? page)
        {
            if (string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(module))
            {
                return RedirectToAction("Index", "Timeline");
            }

            if (!string.IsNullOrEmpty(module))
            {
                ViewBag.Module = module;
            }
            else if (!string.IsNullOrEmpty(genre))
            {
                ViewBag.Genre = genre;
            }

            var result = GetCardsToPage(string.IsNullOrEmpty(module) ? GetCardsByGenre(genre) : GetCardsByModule(module), ref page, out int maxpage);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;

            ViewBag.PrevText = CardDb.PrevText;

            return View(result);
        }
    }
}