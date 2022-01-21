using System;
using System.IO;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var likedTags = new[]
            {
                "Petite",
                "Ahegao",
                "Orgasm Denial",
                "Crotch Tattoo",
                "Heart Pupils",
                "Deepthroat",
                "Big Dick",
                "X-ray",
                "Loli",
                "Yuri",
                "Qipao",
                "Body Swap",
                "Stockings",
                "Futanari",
            };

            var dislikedTags = new[]
            {
                "Huge Boobs",
                "Milf",
                "Busty",
            };

            var sorter = new CultureSorter(@"D:\DL_Zone\Fakku Data")
                .WithLikedTags(likedTags)
                .WithDislikedTags(dislikedTags)
                .SortFiles();


            /*foreach (var folder in folders)
            {
                var info = $@"{folder}\info.json";
                var details = $@"{folder}\details.json";

            }*/
        }
    }
}
