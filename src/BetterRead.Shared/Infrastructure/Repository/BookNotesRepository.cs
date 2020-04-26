﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Helpers;
using BetterRead.Shared.Infrastructure.Domain.Book;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Infrastructure.Repository
{
    public interface IBookNotesRepository
    {
        Task<IEnumerable<Note>> GetNotesAsync(int bookId);
    }
    
    public class BookNotesRepository : IBookNotesRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookNotesRepository(HtmlWeb htmlWeb) => 
            _htmlWeb = htmlWeb;
        
        public async Task<IEnumerable<Note>> GetNotesAsync(int bookId)
        {
            var url = string.Format(BookUrlPatterns.Notes, bookId);

            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var documentNode = htmlDocument.DocumentNode.QuerySelectorAll("td.MsoNormal").FirstOrDefault()?.ChildNodes;
             
            return documentNode
                .SplitOn(node => node.Name == "a")
                .ToList()
                .Select(g => g.Where(node => !(node is HtmlTextNode)))
                .Where(g => g.Any())
                .Select(ConvertNote);
        }

        private static Note ConvertNote(IEnumerable<HtmlNode> note) =>
            new Note
            {
                Id = Convert.ToInt32(note.FirstOrDefault()?.InnerText),
                Contents = note.Select(nt => nt.InnerText).ToList()
            };
    }
}