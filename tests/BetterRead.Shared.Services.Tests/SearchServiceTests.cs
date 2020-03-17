using System.Threading.Tasks;
using BetterRead.Shared.Services.Abstractions;
using Xunit;

namespace BetterRead.Shared.Services.Tests
{
    public class SearchServiceTests
    {
        [Fact]
        public async void GetBook_ShouldNotBeNull()
        {
            //Assign
            var sut = GetSut();
            //Act
            var book = await sut.SearchBooksAsync("Тест");
            //Assert
            Assert.NotNull(book);
        }

        [Fact]
        public async void GetAuthor_ShouldNotBeNull()
        {
            var sut = GetSut();
            //Act
            var authors = await sut.SearchAuthorAsync("Олег");
            
            Assert.NotNull(authors);
        }
        
        [Fact]
        public async void GetSeries_ShouldNotBeNull()
        {
            var sut = GetSut();
            //Act
            var series = await sut.SearchSeriesBookAsync("Олег");
            
            Assert.NotNull(series);
        }
        private static SearchService GetSut()
        {
            return new SearchService(new GetJsonDataService());
        }
    }
}