using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BetterRead.Shared.Tests
{
    public class LoveReadTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LoveReadTests(ITestOutputHelper testOutputHelper) => 
            _testOutputHelper = testOutputHelper;

        [Fact]
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
            _testOutputHelper.WriteLine(stopWatch.Elapsed.Seconds.ToString());
            _testOutputHelper.WriteLine(stopWatch.Elapsed.Milliseconds.ToString());
            
            Assert.NotNull(data);
        }
    }
}