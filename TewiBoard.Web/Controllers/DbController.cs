using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TewiBoard.Web.Models;
using TewiBoard.Web.Repositories;

namespace TewiBoard.Web.Controllers
{
    public class DbController : Controller
    {
        public CardDb Cards { get; set; }

        protected readonly ILogger<DbController> _logger;

        public DbController(CardDb db, ILogger<DbController> logger)
        {
            _logger = logger;
            Cards = db;
            Cards.UpdatePrevText();
        }

        protected List<CardModel> GetCardsByWord(string word)
        {
            return Cards.GetCardsByKeyword(word);
        }

        protected List<CardModel> GetCardsAll() => Cards.GetCardsAll();

        protected List<CardModel> GetCardsByGenre(string genre) => Cards.GetCardsByGenre(genre);

        protected List<CardModel> GetCardsByModule(string module) => Cards.GetCardsByModule(module);

        protected List<CardModel> GetCardThread(long rootpid)
        {
            List<CardModel> result = new List<CardModel>
            {
                Cards.GetCardByPid(rootpid)
            };

            result.AddRange(Cards.GetCardThread(rootpid));

            result = result.Distinct().ToList();

            return result;
        }

        protected List<CardModel> GetCardsToPage(List<CardModel> sourceList, ref int? index, out int maxpage, int size = 20)
        {
            // Count page index
            maxpage = (sourceList.Count + size - 1) / size;
            if (index == null || index <= 0) index = 1;
            if (index > maxpage)
            {
                index = maxpage;
            }

            // Get pages
            int startRow = (index.Value - 1) * size;
            try
            {
                var result = sourceList.OrderByDescending(p => p.PostTime)
                                .Skip(startRow).Take(size);
                // Return 
                return result.ToList();
            }
            catch (Exception)
            {
                return new List<CardModel>()
                {
                    new CardModel()
                    {
                        Title = "Not Found",
                        UserNick = "Not Found",
                        UserId = "0x00",
                        Content = "Not found or deleted."
                    }
                };
            }
        }
    }
}
