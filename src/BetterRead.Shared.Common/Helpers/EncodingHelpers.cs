using System.Text;
using System.Web;

namespace BetterRead.Shared.Common.Helpers
{
    public static class EncodingHelpers
    {
        public static string GetCyrillicEncoding(string name) =>
            HttpUtility.UrlEncode(name, Encoding.GetEncoding(1251));
    }
}
