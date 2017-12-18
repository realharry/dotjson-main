using System.IO;
using System.Threading.Tasks;


namespace DotJson.Lite
{
	public interface LiteJsonParser
	{
		Task<object> ParseAsync(string jsonStr);
        Task<object> ParseAsync(TextReader reader);
        // Task<object> ParseAsync(IStorageFile jsonFile);
    }
}
