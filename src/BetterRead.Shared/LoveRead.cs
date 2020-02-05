using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Abstractions;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Services;
using BetterRead.Shared.Services.Abstractions;

namespace BetterRead.Shared
{
    public class LoveRead : ILoveRead
    {
        private readonly IBookService _bookService;

        public LoveRead() : this(new BookService())
        {
        }
        
        public LoveRead(IBookService bookService) => 
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));

        public async Task<Book> GetBookAsync(int bookId) => 
            await _bookService.GetBookByIdAsync(bookId);

        public async Task<Book> GetBookAsync(string url) => 
            await _bookService.GetBookByUrlAsync(url);

        public async Task<BookInfo> GetBookInfoAsync(int bookId) => 
            await _bookService.GetBookInfoByIdAsync(bookId);

        public async Task<BookInfo> GetBookInfoAsync(string url) => 
            await _bookService.GetBookInfoByUrlAsync(url);

        public async Task<IEnumerable<BookInfo>> SearchBooks(string name) =>
            await _bookService.SearchBooks(name);
    }
}