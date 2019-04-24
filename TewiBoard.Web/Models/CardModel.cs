using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TewiBoard.Web.Models
{
    [Table("card"), Serializable]
    public class CardModel
    {
        // post id
        [Key, Column("pid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Pid { get; set; }

        // genre
        [Column("genre")]
        public string Genre { get; set; }
        // module 
        [Column("module")]
        public string Module { get; set; }

        // post reply to pid (if exists)
        [Column("replyid")]
        public long? ReplyId { get; set; }
        // post reply root pid
        [Column("replytop")]
        public long? ReplyTop { get; set; }

        // post title
        [Column("title"), StringLength(30)]
        public string Title { get; set; }
        // post author's nickname
        [Column("usernick"), StringLength(30)]
        public string UserNick { get; set; }
        // post author's id
        [Column("userid")]
        public string UserId { get; set; }
        // post time
        [Column("posttime")]
        public DateTime PostTime { get; set; }

        // post conntent
        [Column("content"), Required, StringLength(1000), MinLength(6)]
        public string Content { get; set; }
        // post image
        [Column("imgurl")]
        public string ImageUrl { get; set; }

        [Column("isred")]
        public int? IsRed { get; set; }

        [NotMapped]
        public string Hash { get; set; }

        public CardModel() { }

        public CardModel(CardWithImageModel cardwithfile)
        {
            Pid = cardwithfile.Pid;
            Genre = cardwithfile.Genre;
            Module = cardwithfile.Module;

            ReplyId = cardwithfile.ReplyId;
            ReplyTop = cardwithfile.ReplyTop;

            Title = cardwithfile.Title;
            UserNick = cardwithfile.UserNick;
            UserId = cardwithfile.UserId;
            PostTime = cardwithfile.PostTime;

            Content = cardwithfile.Content;
        }
    }
}
