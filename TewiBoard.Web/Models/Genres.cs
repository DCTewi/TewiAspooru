using System.Collections.Generic;

namespace TewiBoard.Web.Models
{
    public class Genres
    {
        public static string[] List =
        {
            "Complex",
            "Culture",
            "Game",
            "Activity",
            "Admin"
        };

        public static Dictionary<string, List<string>> Modules = new Dictionary<string, List<string>>()
        {
            {
                "Complex", new List<string>
                {
                    "ComplexTotal",
                    "Spoof",
                    "Feeling",
                    "Sport",
                    "Pet",
                    "TreeHole",
                    "UrbanLegend"
                }
            },
            {
                "Culture", new List<string>
                {
                    "Lesson",
                    "Food",
                    "Story",
                    "ACG",
                    "Idol",
                    "Movie",
                    "Tech"
                }
            },
            {
                "Game", new List<string>
                {
                    "PC Game",
                    "GameConsole",
                    "BoardGame",
                }
            },
            {
                "Activity", new List<string>
                {
                    "Match",
                    "BuyBuyBuy",
                    "Festival"
                }
            },
            {
                "Admin", new List<string>
                {
                    "DutyRoom",
                    "Support",
                    "QandA",
                }
            }
        };

        public static Dictionary<string, string> Rules = new Dictionary<string, string>()
        {
            { "Complex", "The Complex Genre is for any thing other genre not included.."},
            { "Culture", "Culture Rules" },
            { "Game", "Game Rules" },
            { "Activity", "Activity Rules" },
            { "Admin", "Admin Rules" },
            { "ComplexTotal", "ComplexTotal" }
            // etc...
        };

        public static List<string> OpKeys = new List<string>()
        {
        };
    }
}
