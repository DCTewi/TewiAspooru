using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TewiBoard.Web.Repositories;

namespace TewiBoard.Web.Controllers
{
    public class SearchController : DbController
    {
        public SearchController(CardDb db, ILogger<DbController> logger) : base(db, logger) { }

        public IActionResult Index(string keyword, int? page)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index", "Home");
            }

            var result = GetCardsToPage(GetCardsByWord(keyword), ref page, out int maxpage);

            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;

            ViewBag.PrevText = CardDb.PrevText;

            return View(result);
        }
    }
}