namespace TewiBoard.Web.Models
{
    public class PostModal
    {
        public string Hash;
        public string ButtonText;

        public bool IsReply;
        public long? ReplyId;
        public CardModel ReplyCard;

        public string Genre;
        public string Module;
    }
}
