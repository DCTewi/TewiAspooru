using Microsoft.AspNetCore.Http;
using System;

namespace TewiBoard.Web.Models
{
    public class CardWithImageModel
    {
        public long Pid { get; set; }

        public string Genre { get; set; }
        public string Module { get; set; }

        public long? ReplyId { get; set; }
        public long? ReplyTop { get; set; }

        public string Title { get; set; }
        public string UserNick { get; set; }

        public string UserId { get; set; }

        public DateTime PostTime { get; set; }
        public string Content { get; set; }
        public IFormFile Image { get; set; }

        public string Hash { get; set; }
    }
}
