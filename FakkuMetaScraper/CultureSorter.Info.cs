using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandbox;

public class DoujinInfo
{
	[JsonConverter(typeof(ArtistConverter))]
	[JsonProperty("Artist")]
	public string[] Artist { get; set; } = Array.Empty<string>();

	[JsonProperty("Description")]
	public string Description { get; set; } = string.Empty;

	[JsonProperty("Pages")]
	public int Pages { get; set; }

	[JsonProperty("Tags")]
	public string[] Tags { get; set; } = Array.Empty<string>();

	[JsonProperty("Title")]
	public string Title { get; set; } = string.Empty;

	/*[JsonProperty("Released")]
	public DateTime Released { get; set; }*/
}

internal class ArtistConverter : JsonConverter
{
	public override bool CanWrite => false;

	public override bool CanConvert(Type objectType) => objectType == typeof(string[]);

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var token = JToken.ReadFrom(reader);

		switch (token)
		{
			case JValue v:
				var val = v.ToObject<string>();
				if (val == null)
					return Array.Empty<string>();
				return new string[] { val };

			case JArray arr:
				return arr.ToObject<string[]>();

			default:
				throw new NotSupportedException();
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
}