using Flurl;

using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

using System.Text;

namespace FakkuMetaScraper.Models;

public class EntryInfo
{

    public string Filepath { get; }
    public List<string> Tags { get; set; } = new List<string>();
	public string Name { get; }
	public string Artist { get; }
	public string Collection { get; }

	private static readonly TextParser<string> _artistParser = from open in Character.EqualTo('[')
																				 from artist in Character.Except(']').Many()
																				 from close in Span.EqualTo("] ")
																				 select new string(artist);

	private static readonly TextParser<string> _collectionNameParser = from open in Character.EqualTo('(')
																	   from collection in Character.ExceptIn('(', ')').Many()
																	   from close in Span.EqualTo(").zip").AtEnd()
																	   select new string(collection);

	private static readonly TextParser<char> _nameParser = from open in Character.LetterOrDigit
															 from rest in Character.AnyChar
															 select open;

	private static readonly Tokenizer<TitleTokenType> _titleTokenzier = new TokenizerBuilder<TitleTokenType>()
			.Match(_artistParser, TitleTokenType.Artist)
			.Match(_collectionNameParser, TitleTokenType.Collection)
			.Match(Character.AnyChar, TitleTokenType.Name)
			.Build();

	//public EntryInfo(string name, string artist, string collection)
	//{
	//	Name = name;
	//	Artist = artist;
	//	Collection = collection;
	//	File
	//}

	public EntryInfo(string filepath)
	{
		Filepath = filepath;
		var filename = Path.GetFileName(filepath);
		(Name, Artist, Collection) = ParseFilename(filename);
	}

	public static (string name, string artist, string collection) ParseFilename(string filename)
	{
		var tokens = _titleTokenzier.Tokenize(filename);
		var artist = _artistParser.Parse(tokens.First(t => t.Kind == TitleTokenType.Artist).ToStringValue());
		var colleciton = _collectionNameParser.Parse(tokens.First(t => t.Kind == TitleTokenType.Collection).ToStringValue());
		var name = string.Join("", tokens.Where(t => t.Kind == TitleTokenType.Name)
			.Select(t => t.ToStringValue())
			.ToList()).Trim();
		return (name, artist, colleciton);
	}

	public string GetEntryUrl()
	{
		return "https://fakku.net/hentai/".AppendPathSegment(PrepareUrlName());
	}

	private string PrepareUrlName()
	{
		var specialParser = from c in Character.Matching(c => c > 122, "")
							select c;

		var tokenizer = new TokenizerBuilder<TokenType>()
			.Ignore(Character.In('(', ')', '!', '?', '.', ':', ',', ';', '/', '\\', '_', '#', '$', '@', '~', '&', '^', '%', '\''))
			.Match(specialParser, TokenType.Special)
			.Match(Character.AnyChar, TokenType.Letter)
			.Build();

		var tokens = tokenizer.Tokenize(Name.ToLowerInvariant());

		var sb = new StringBuilder();

		foreach (var token in tokens)
		{
			switch(token.Kind)
			{
				case TokenType.Letter:
					var str = token.ToStringValue()
						.Replace(' ', '-')
						.Replace("★", "bzb")
						.Replace("(", "")
						.Replace(")", "");

					if (sb.Length > 1 && str == "-" && sb[^1] == '-')
						continue;

					sb.Append(str);
					break;
				case TokenType.Special:
					var sp = token.ToStringValue()
						.Replace("★", "bzb");
					sb.Append(sp);
					break;
			}
		}


		sb.Append("-english");
		if (sb[0] == '-')
			return sb.ToString()[1..];
		return sb.ToString();
	}

	private enum TitleTokenType
	{
		Artist,
		Collection,
		Name
	}

	private enum TokenType
	{
		Letter,
		Special,
	}

	public static string GetReplacement(char c)
	{
		return c switch
		{
			' ' => "-",
			',' => "",
			'-' => "-",
			'\'' => "-",

			_ => c.ToString()
		};
	}

	public static Dictionary<char, string> REPLACEMENTS = new Dictionary<char, string>()
	{
		{ ' ', "-" },
		{ ',', "" },
		{ '-', "-" },
		{ '★', "" }
	};
}