using System.IO;
using System.Threading.Tasks;


namespace HoloJson
{
	public interface LiteJsonBuilder
	{
		Task<string> BuildAsync(object jsonObj);
		Task BuildAsync(TextWriter writer, object jsonObj);
        // Task BuildAsync(IStorageFile jsonFile, object jsonObj);
	}
}
