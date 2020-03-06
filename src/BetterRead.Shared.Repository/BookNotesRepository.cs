using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Common.Helpers;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Repository.Abstractions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Repository
{
    public class BookNotesRepository : IBookNotesRepository
    {
        private readonly HtmlWeb _htmlWeb;
        public BookNotesRepository()
        {
            _htmlWeb = new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("windows-1251")};    
        }
        public async Task<IEnumerable<Note>> GetNotesAsync(int bookId)
        {
            var url = string.Format(BookUrlPatterns.Notes, bookId);
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var documentNode = htmlDocument.DocumentNode.QuerySelectorAll("td.MsoNormal").FirstOrDefault()?.ChildNodes;
            
            return documentNode
                .SplitOn(node => node.Name == "a")
                .Select(g => g.Where(node => !(node is HtmlTextNode)))
                .Where(g => g.Any())
                .Select(note => ConvertHtmlToNote(note));;
        }
        
        public Note ConvertHtmlToNote(IEnumerable<HtmlNode> note) =>
            new Note()
            {
                Id = Convert.ToInt32(note.FirstOrDefault()?.InnerText),
                Contents = note.Skip(1).Select(note => note.InnerText).ToList()
            };
    }
}