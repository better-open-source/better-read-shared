using System.IO;

namespace BetterRead.Shared.Helpers
{
    public static class StreamHelpers
    {
        public static MemoryStream ToMemoryStream(this Stream input)
        {
            var ms = new MemoryStream();
            input.CopyTo(ms);
            input.Flush();
            return ms;
        }
    }
}