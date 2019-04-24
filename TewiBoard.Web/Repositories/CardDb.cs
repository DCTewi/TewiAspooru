using System;
using System.Collections.Generic;
using System.Linq;
using TewiBoard.Web.Models;

namespace TewiBoard.Web.Repositories
{
    public class CardDb
    {
        private TewiDbContext DbContext { get; }
        public static Dictionary<long, string> PrevText = new Dictionary<long, string>();

        public CardDb(TewiDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int Count
        {
            get
            {
                return DbContext.Cards.Count();
            }
        }

        public int Add(CardModel card)
        {
            DbContext.Cards.Add(card);
            UpdatePrevText();
            return DbContext.SaveChanges();
        }

        public int Delete(long pid)
        {
            var targetCard = GetCardByPid(pid);
            DbContext.Cards.Remove(targetCard);
            return DbContext.SaveChanges();
        }

        public int Delete(CardModel card)
        {
            var targetCard = GetCardByPid(card.Pid);
            DbContext.Cards.Remove(targetCard);
            return DbContext.SaveChanges();
        }

        public void UpdatePrevText()
        {
            PrevText.Clear();
            var list = GetCardsAll();
            foreach (var item in list)
            {
                PrevText[item.Pid] = GetCardTextPrev(item.Pid);
            }
        }

        public int UpdateReplyTop(CardModel topcard)
        {
            var targetCard = DbContext.Cards.FirstOrDefault(c => c.Pid == topcard.Pid);
            targetCard.ReplyTop = targetCard.Pid;
            return DbContext.SaveChanges();
        }

        public string GetCardTextPrev(long pid)
        {
            CardModel cardtemp = GetCardByPid(pid);

            return cardtemp.Content.Substring(0, Math.Min(cardtemp.Content.Length, 9));
        }

        public CardModel GetCardByPid(long pid)
        {
            return DbContext.Cards.FirstOrDefault(c => c.Pid == pid);
        }

        public List<CardModel> GetCardThread(long rootpid)
        {
            var result = from p in DbContext.Cards
                         where p.ReplyId == rootpid
                         select p;
            var resultList = new List<CardModel>();
            var tempList = result.ToList();

            while (tempList.Count > 0)
            {
                int lastcount = tempList.Count;
                for (int i = 0; i < lastcount; i++)
                {
                    var tempEnum = from p in DbContext.Cards
                                   where p.ReplyId == tempList[i].Pid
                                   select p;
                    foreach (var p in tempEnum.ToList())
                    {
                        tempList.Add(p);
                    }

                    resultList.AddRange(tempList);
                }

                tempList.RemoveRange(0, lastcount);
            }

            return resultList;
        }

        public List<CardModel> GetCardsAll()
        {
            return DbContext.Cards.ToList();
        }

        public List<CardModel> GetCardsByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return null;
            }
            else
            {
                var result = from p in DbContext.Cards
                             where p.Genre == genre
                             select p;
                return result.ToList();
            }
        }

        public List<CardModel> GetCardsByModule(string module)
        {
            if (string.IsNullOrEmpty(module))
            {
                return null;
            }
            else
            {
                var result = from p in DbContext.Cards
                             where p.Module == module
                             select p;
                return result.ToList();
            }
        }

        public List<CardModel> GetCardsByKeyword(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return DbContext.Cards.ToList();
            }
            else
            {
                var result = from p in DbContext.Cards
                             where p.Title.IndexOf(word) > -1 || p.Content.IndexOf(word) > -1
                             select p;
                return result.ToList();
            }
        }
    }
}
