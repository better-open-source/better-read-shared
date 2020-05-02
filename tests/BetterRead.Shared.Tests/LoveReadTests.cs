using System.Diagnostics;
using System.Threading.Tasks;
using BetterRead.Shared.Infrastructure.Services;
using NUnit.Framework;

namespace BetterRead.Shared.Tests
{
    public class LoveReadTests
    {
        private const string BookUrl = "http://loveread.ec/view_global.php?id=45105";
        
        private readonly ILoveRead _sut;
        
        public LoveReadTests() => 
            _sut = new LoveRead();

        [Test]
        public async Task GetBookAsync_WithUrl_IsNotNull()
        {
            // Assign
            var stopWatch = new Stopwatch();
            
            // Act
            stopWatch.Start();
            var book = await _sut.GetBookAsync(BookUrl);
            stopWatch.Stop();

            // Assert
            await TestContext.Out.WriteLineAsync($"Elapsed seconds: {stopWatch.Elapsed.Seconds}");
            await TestContext.Out.WriteLineAsync($"Elapsed milliseconds: {stopWatch.Elapsed.Milliseconds}");
            
            Assert.NotNull(book);
        }
    }
}