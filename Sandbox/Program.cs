using Superpower;
using Superpower.Model;
using Superpower.Parsers;

using System;
using System.IO;
using System.Linq;
using FakkuMetaScraper.Models;
using Flurl.Http;
using System.Threading;
using FakkuMetaScraper;
using System.Threading.Tasks;

namespace Sandbox
{
    class Program
    {
        static async Task Main(string[] args)
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
				.EnsureFolders();


			var files = Directory.GetFiles(@"D:\DL_Zone\Fakku\fakku 1-1000\chapters");

			var fakku = new FakkuMeta(files)
				.AddSorter(sorter);


			await fakku.LoadTagsAsync();

			fakku.Sort().WriteLogs("errors.json");

			
		}
    }
}
