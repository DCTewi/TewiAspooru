using TewiBoard.Web.Models;

namespace TewiBoard.Web.Repositories
{
    public class Generator
    {
        public static string GetUserId(string ip)
        {
            return string.Format("{0:x}", ip.GetHashCode());
        }

        internal static string GetGenreByModule(string module)
        {
            foreach (var genre in Genres.List)
            {
                foreach (var m in Genres.Modules[genre])
                {
                    if (m == module)
                    {
                        return genre;
                    }
                }
            }

            return "NONONO!!";
        }
    }
}
