using Newtonsoft.Json;

namespace Sandbox;

public class TachiDetails
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

	public TachiDetails(DoujinInfo info)
	{
		Title = info.Title;
		Artist = Author = string.Join(", ", info.Artist);
		Description = info.Description;
		Status = "0";
		Genre = info.Tags.ToArray();
	}
}