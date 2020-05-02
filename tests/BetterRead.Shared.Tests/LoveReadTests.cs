using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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

        [Test]
        public async Task GetBookDocument_SaveLocally()
        {
            // Assign
            var parserWatch = new Stopwatch();
            var builderWatch = new Stopwatch();
            
            // Act
            parserWatch.Start();
            var book = await _sut.GetBookAsync(BookUrl);
            parserWatch.Stop();
            
            builderWatch.Start();
            var content = await _sut.GetBookDocument(book);
            await File.WriteAllBytesAsync($"{book.Info.Name}.docx", content);
            builderWatch.Stop();

            // Assert
            await TestContext.Out.WriteLineAsync("========Parser=========");
            await TestContext.Out.WriteLineAsync($"Elapsed seconds: {parserWatch.Elapsed.Seconds}");
            await TestContext.Out.WriteLineAsync($"Elapsed milliseconds: {parserWatch.Elapsed.Milliseconds}");
            await TestContext.Out.WriteLineAsync("========Builder========");
            await TestContext.Out.WriteLineAsync($"Elapsed seconds: {builderWatch.Elapsed.Seconds}");
            await TestContext.Out.WriteLineAsync($"Elapsed milliseconds: {builderWatch.Elapsed.Milliseconds}");
            
            Assert.Pass();
        }
    }
}