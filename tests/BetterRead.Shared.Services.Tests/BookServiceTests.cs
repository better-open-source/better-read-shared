using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Repository.Abstractions;
using Xunit;

namespace BetterRead.Shared.Services.Tests
{
    public class BookServiceTests
    {
        [Fact]
        public async Task GetBook_WhenValidUrl_ShouldNotBeNull()
        {
            //Assign
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            const string bookUrl = "http://loveread.ec/view_global.php?id=45105";
            var sut = GetSut();
            
            //Act
            var book = await sut.GetBookByUrlAsync(bookUrl);

            //Assert
            Assert.NotNull(book);
        }

        [Fact]
        public async Task GetSearchBooks_WhenValidName_ShouldNotBeNull()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Assign
            var sut = GetSut();

            //Act
            var books = await sut.SearchBooks("Война миров");

            //Assert
            Assert.NotNull(books);
        }

        private static BookService GetSut() => 
            new BookService();

        private static BookService GetSut(
            IBookInfoRepository infoRepository,
            IBookSheetsRepository sheetsRepository,
            IBookContentsRepository contentsRepository,
            IBookNotesRepository notesRepository,
            IBookSearchRepository bookSearchRepository) =>
            new BookService(infoRepository, sheetsRepository, contentsRepository, notesRepository, bookSearchRepository);
    }
}