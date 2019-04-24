using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TewiBoard.Web.Repositories;

namespace TewiBoard.Web.Controllers
{
    public class TimelineController : DbController
    {
        public TimelineController(CardDb db, ILogger<DbController> logger) : base(db, logger) { }

        public IActionResult Index(int? page)
        {
            var result = GetCardsToPage(GetCardsAll(), ref page, out int maxpage);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;

            ViewBag.PrevText = CardDb.PrevText;

            return View(result);
        }
    }
}
