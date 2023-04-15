using FakkuMetaScraper.Models;

using Flurl;
using Flurl.Http;

using HtmlAgilityPack;

using Newtonsoft.Json;

using Sandbox;

using System.IO.Compression;
using System.Text;

namespace FakkuMetaScraper;
public class FakkuMeta
{
	private readonly List<EntryInfo> _entries;
	private CultureSorter? _sorter;
	private readonly List<ErrorEntry> _errors;

	public FakkuMeta(IEnumerable<string> files)
	{
		_entries = files.Select(f => new EntryInfo(f)).ToList();
		_errors = new List<ErrorEntry>();
	}

	private record ErrorEntry(string File, string Error);
	public FakkuMeta AddSorter(CultureSorter sorter)
	{
		_sorter = sorter;
		return this;
	}

	public FakkuMeta Sort()
	{
		if (_sorter == null)
			throw new InvalidOperationException("A sorter has not been added");

		foreach (var entry in _entries)
		{
			var info = GenerateDoujinInfo(entry);
			var interFile = GenerateIntermediteFile(entry.Filepath, info);
			_sorter.SortItem(entry.Filepath, _ => info, 
			accept: (_, inf) =>
			{
				var dstPath = Path.Combine(_sorter.AcceptPath, $"[{entry.Artist}] {entry.Name}.zip");
				WriteFile(dstPath, interFile);
			}, reject: (_, inf) =>
			{
				var dstPath = Path.Combine(_sorter.RejectPath, $"[{entry.Artist}] {entry.Name}.zip");
				WriteFile(dstPath, interFile);
			}, maybe: (_, inf) => {
				var dstPath = Path.Combine(_sorter.MaybeDir, $"[{entry.Artist}] {entry.Name}.zip");
				WriteFile(dstPath, interFile);
			});
		}

		return this;
	}

	private static void WriteFile(string filepath, Stream data)
	{
		using var file = new FileStream(filepath, FileMode.Create);
		data.CopyTo(file);
		file.Flush();
		data.Dispose();
	}

	public async Task LoadTagsAsync(float delaySeconds = 5)
	{
		foreach (var entry in _entries)
		{
			Console.Write($"Loading data for {entry.Name}... ");
			try
			{
				await LoadEntryTagsAsync(entry);
				Console.WriteLine("Done!");
			}catch(FlurlHttpException ex)
			{
				_errors.Add(new(entry.Filepath, ex.StatusCode?.ToString() ?? ""));
				Console.WriteLine("Failed!");
			}
			await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
		}
	}

	public FakkuMeta WriteLogs(string filepath) 
	{
		File.WriteAllText(filepath, JsonConvert.SerializeObject(_errors));
		return this;
	}

	public static MemoryStream GenerateIntermediteFile(string filename, DoujinInfo info)
	{
		using var sourceFileData = new FileStream(filename, FileMode.Open);
		using var zip = new ZipArchive(sourceFileData, ZipArchiveMode.Read);


		info.Pages = zip.Entries.Count;
		var targetStream = new MemoryStream();
		using var targetZip = new ZipArchive(targetStream, ZipArchiveMode.Create, true);

		foreach (var entry in zip.Entries)
		{
			using var srcStream = entry.Open();

			var newEntry = targetZip.CreateEntry(entry.Name);
			using var tgtEntryStream = newEntry.Open();
			srcStream.CopyTo(tgtEntryStream);

			tgtEntryStream.Flush();
		}

		var infoEntry = targetZip.CreateEntry("info.json");
		using var infoStream = infoEntry.Open();
		infoStream.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(info, Formatting.Indented)));

		targetZip.Dispose();
		targetStream.Position = 0;
		return targetStream;
	}

	public static async Task LoadEntryTagsAsync(EntryInfo entry)
	{
		var url = entry.GetEntryUrl();
		var html = await url.GetStringAsync();

		if (string.IsNullOrWhiteSpace(html))
			return;

		var tags= ParseTags(html);
		entry.Tags = tags;
	}

	public static List<string> ParseTags(string html)
	{
		var doc = new HtmlDocument();
		doc.LoadHtml(html);

		var tagElems = doc.QuerySelectorAll(".inline-block.bg-gray-100[href]");

		var tags = tagElems.Select(t => t.InnerText.Trim()).ToList();

		return tags;

	}


	public static DoujinInfo GenerateDoujinInfo(EntryInfo entry)
	{
		var info = new DoujinInfo()
		{
			Artist = new[] { entry.Artist },
			Description = entry.Collection,
			Title = entry.Name,
			Tags = entry.Tags.ToArray()
		};

		return info;
	}

	public static TachiDetails GenerateTachiDetials(EntryInfo entry)
	{
		var info = GenerateDoujinInfo(entry);
		var details = new TachiDetails(info);

		return details;
	}

}
