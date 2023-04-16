using Newtonsoft.Json;

using System.IO.Compression;

namespace Sandbox;

public class CultureSorter
{
	public string RejectPath => _rejectDir;
	public string AcceptPath => _acceptDir;
	public string MaybeDir => _maybeDir;
	public string SrcDir => _srcDir;
	public string OutDir => _outDir;

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

	public CultureSorter EnsureFolders()
	{
		if (!Directory.Exists(_rejectDir))
			Directory.CreateDirectory(_rejectDir);
		if (!Directory.Exists(_acceptDir))
			Directory.CreateDirectory(_acceptDir);
		if (!Directory.Exists(_maybeDir))
			Directory.CreateDirectory(_maybeDir);
		if (!Directory.Exists(_srcDir))
			Directory.CreateDirectory(_srcDir);
		if (!Directory.Exists(_outDir))
			Directory.CreateDirectory(_outDir);
		return this;
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

	public CultureSorter ResetLikedTags()
	{
		_likedTags.Clear();
		return this;
	}

	public CultureSorter WithDislikedTags(params string[] tags)
	{
		_dislikedTags.AddRange(tags);
		return this;
	}

	public CultureSorter ResetDislikedTags()
	{
		_dislikedTags.Clear();
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
			if (infoEntry == null)
				continue;
			using var infoStream = infoEntry.Open();
			using var reader = new StreamReader(infoStream);
			var infoJson = reader.ReadToEnd();

			var info = JsonConvert.DeserializeObject<DoujinInfo>(infoJson);
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

	public void SortItem(string file, Func<string, DoujinInfo> getInfo, Action<string, DoujinInfo> accept, Action<string, DoujinInfo> reject, Action<string, DoujinInfo> maybe)
	{
		var like = 0;
		var dislike = 0;

		var info = getInfo(file);

		if(info.Tags.Length == 0)
		{
			maybe(file, info);
			return;
		}

		foreach (var tag in info.Tags)
		{
			if (_likedTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
				like++;
			if (_dislikedTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
				dislike++;
		}

		if (dislike == 0 && like > 0)
		{
			accept(file, info);
			return;
		}

		if (like == 0 && dislike > 0)
		{
			reject(file, info);
			return;
		}

		maybe(file, info);
	}

	public CultureSorter ConvertForTachi(string folder = "accept")
	{
		var files = Directory.GetFiles(Path.Combine(_baseDir, folder));
		foreach (var filePath in files)
		{
			using var file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
			var filename = Path.GetFileName(filePath);
			var cleanName = Path.GetFileNameWithoutExtension(filename);

			var outFolder = Path.Combine(_outDir, cleanName);
			Directory.CreateDirectory(outFolder);
			var outFile = Path.Combine(outFolder, "Chapter1.cbz");

			using var outFileStream = new FileStream(outFile, FileMode.Create);

			file.CopyTo(outFileStream);
			outFileStream.Position = 0;
			file.Dispose();

			using var zip = new ZipArchive(outFileStream, ZipArchiveMode.Update, true);

			var infoEntry = zip.GetEntry("info.json");
			if (infoEntry == null)
				continue;
			using var infoStream = infoEntry.Open();
			using var reader = new StreamReader(infoStream);
			var infoJson = reader.ReadToEnd();
			infoStream.Dispose();

			var info = JsonConvert.DeserializeObject<DoujinInfo>(infoJson);
			var details = new TachiDetails(info);

			infoEntry.Delete();

			var coverEntry = zip.Entries.OrderBy(e => e.Name).First();

			using var coverStream = coverEntry.Open();
			var coverFile = Path.Combine(outFolder, "cover.png");
			using var coverFileStream = new FileStream(coverFile, FileMode.Create);
			coverFileStream.Position = 0;
			coverStream.CopyTo(coverFileStream);
			coverFileStream.Flush();
			coverStream.Dispose();

			zip.Dispose();

			var detailsFile = Path.Combine(outFolder, "details.json");

			File.WriteAllText(detailsFile, JsonConvert.SerializeObject(details, Formatting.Indented));
		}
		return this;
	}
}