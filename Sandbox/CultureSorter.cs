using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Sandbox;

public class CultureSorter
{
	private readonly string _baseDir;
	private string _rejectDir;
	private string _acceptDir;
	private string _maybeDir;
	private string _srcDir;
	private string _outDir;

	private List<string> _likedTags;
	private List<string> _dislikedTags;

	public CultureSorter(string baseDir)
	{
		_baseDir = baseDir;
		_rejectDir = Path.Combine(baseDir, "reject");
		_acceptDir = Path.Combine(baseDir, "accept");
		_maybeDir = Path.Combine(baseDir, "maybe");
		_srcDir = Path.Combine(baseDir, "src");
		_outDir = Path.Combine(baseDir, "out");
		_likedTags = new List<string>();
		_dislikedTags = new List<string>();	
	}

	public CultureSorter WithRejectFolder(string foldername)
	{
		_rejectDir = Path.Combine(_baseDir, foldername);
		return this;
	}

	public CultureSorter WithAcceptFolder(string foldername)
	{
		_acceptDir = Path.Combine(_baseDir, foldername);
		return this;
	}

	public CultureSorter WithMaybeFolder(string foldername)
	{
		_maybeDir = Path.Combine(_baseDir, foldername);
		return this;
	}

	public CultureSorter WithSourceFolder(string foldername)
	{
		_srcDir = Path.Combine(_baseDir, foldername);
		return this;
	}

	public CultureSorter WithOutputFolder(string foldername)
	{
		_outDir = Path.Combine(_baseDir, foldername);
		return this;
	}

	public CultureSorter WithLikedTags(params string[] tags)
	{
		_likedTags.AddRange(tags);
		return this;
	}

	public CultureSorter WithDislikedTags(params string[] tags)
	{
		_dislikedTags.AddRange(tags);
		return this;
	}

	public CultureSorter SortFiles()
	{
		var files = Directory.GetFiles(_srcDir);
		var p = 0;
		foreach (var filePath in files)
		{
			p++;
			var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);

			using var zip = new ZipArchive(file, ZipArchiveMode.Read, false);

			var filename = Path.GetFileName(filePath);
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition(0, 0);
			
			Console.ResetColor();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("Current File: ");
			Console.ResetColor();
			Console.WriteLine(filename);
			
			Console.ResetColor();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("Progress: ");
			Console.ResetColor();
			Console.WriteLine($"[{p}/{files.Length}]");

			Console.ResetColor();
			Console.Write("Reading File...");

			var infoEntry = zip.GetEntry("info.json");
			using var infoStream = infoEntry.Open();
			using var reader = new StreamReader(infoStream);
			var infoJson = reader.ReadToEnd();

			var info = JsonConvert.DeserializeObject<Info>(infoJson);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Done!");
			var like = 0;
			var dislike = 0;

			Console.ResetColor();
			Console.Write("Sorting File...");
			foreach (var tag in info.Tags)
			{
				if (_likedTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
					like++;
				if (_dislikedTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
					dislike++;
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Done!");
			if (dislike == 0 && like > 0)
			{
				if (File.Exists(Path.Combine(_acceptDir, filename)))
					continue;
				File.Copy(filePath, Path.Combine(_acceptDir, filename));
				continue;
			}

			if (like == 0 && dislike > 0)
			{
				if (File.Exists(Path.Combine(_rejectDir, filename)))
					continue;
				File.Copy(filePath, Path.Combine(_rejectDir, filename));
				continue;
			}
			if (File.Exists(Path.Combine(_maybeDir, filename)))
				continue;
			File.Copy(filePath, Path.Combine(_maybeDir, filename));
		}
		return this;
	}

	public CultureSorter ConvertForTachi(string folder = "accept")
	{
		var files = Directory.GetFiles(Path.Combine(_baseDir, folder));
		foreach (var filePath in files)
		{
			var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);

			using var zip = new ZipArchive(file, ZipArchiveMode.Read, false);

			var filename = Path.GetFileName(filePath);


			var infoEntry = zip.GetEntry("info.json");
			using var infoStream = infoEntry.Open();
			using var reader = new StreamReader(infoStream);
			var infoJson = reader.ReadToEnd();

			var info = JsonConvert.DeserializeObject<Info>(infoJson);
			var details = new Details(info);

		}


		throw new  NotImplementedException();
	}

	private class Info
	{
		[JsonConverter(typeof(ArtistConverter))]
		[JsonProperty("Artist")]
		public string[] Artist { get; set; }

		[JsonProperty("Description")]
		public string Description { get; set; }

		[JsonProperty("Pages")]
		public int Pages { get; set; }


		[JsonProperty("Tags")]
		public string[] Tags { get; set; }


		[JsonProperty("Title")]
		public string Title { get; set; }

		

		/*[JsonProperty("Released")]
		public DateTime Released { get; set; }*/
	}

	private class Details
	{
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("artist")]
		public string Artist { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("genre")]
		public string[] Genre { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("_status values")]
		public string[] StatusValues { get; set; } = new[] { "0 = Unknown", "1 = Ongoing", "2 = Completed", "3 = Licensed" };

		public Details(Info info)
		{
			Title = info.Title;
			Artist = Author = string.Join(", ", info.Artist);
			Description = info.Description;
			Status = "0";
			Genre = info.Tags;
		}
	}

	class ArtistConverter : JsonConverter
	{
		public override bool CanWrite { get { return false; } }

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string[]);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var token = JToken.ReadFrom(reader);

			switch(token)
			{
				case JValue v:
					var val = v.ToObject<string>();
					if(val == null)
						return Array.Empty<string>();
					return new string[] { val };
				case JArray arr:
					return arr.ToObject<string[]>();
				default:
					throw new NotSupportedException();
			}


		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}