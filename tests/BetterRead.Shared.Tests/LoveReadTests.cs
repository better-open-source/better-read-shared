using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BetterRead.Shared.Tests
{
    public class LoveReadTests
    {

        [Test]
        public async Task GetBookAsync_WithUrl_IsNotNull()
        {
            // Assign
            const string bookUrl = "http://loveread.ec/view_global.php?id=45105";
            var stopWatch = new Stopwatch();
            var sut = new LoveRead();
            
            // Act
            stopWatch.Start();
            var data = await sut.GetBookAsync(bookUrl);
            stopWatch.Stop();

            // Assert
            await TestContext.Out.WriteLineAsync(stopWatch.Elapsed.Seconds.ToString());
            await TestContext.Out.WriteLineAsync(stopWatch.Elapsed.Milliseconds.ToString());
            
            Assert.NotNull(data);
        }
    }
}