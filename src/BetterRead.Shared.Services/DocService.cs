using System;
using System.IO;
using System.Linq;
using BetterRead.Shared.Domain.Book;
using Xceed.Document.NET;
using Xceed.Words.NET;
using System.Drawing;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Common.Helpers;


namespace BetterRead.Shared.Services
{
    public class DocService : IDocService
    {
        private readonly IDownloadService _download;

        private static readonly string DefaultDownloadPath;
        private const string FileTemplate = "{0}\\{1}.docx";

        static DocService()
        {
            DefaultDownloadPath = Directory.GetCurrentDirectory() + "\\Downloads";
        }

        public DocService(IDownloadService download)
        {
            _download = download;
        }

        public void Save(Book book)
            => SaveAs(book, DefaultDownloadPath);

        public void SaveAs(Book book, string path)
        {
            var bookName = book.Info.Name.Replace(" ", String.Empty);
            var fileName = string.Format(FileTemplate, $"{path}\\", bookName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (new DirectoryInfo(path).GetFiles().Any(f => f.Name.Contains(bookName)))
                return;

            DocX doc = DocX.Create(fileName);
            doc.DifferentFirstPage = true;
            doc.AddFooters();

            InsertImage(doc, _download.DownloadFile(book.Info.ImageUrl, bookName));

            book.Contents.ToList().ForEach((content) => InsertContents(doc,content));
            
            foreach (var page in book.Sheets)
            {
                foreach (var data in page.SheetContents)
                {
                    switch (data.ContentType)
                    {
                        case SheetContentType.Header:
                            InsertHeader(doc, data.Content);
                            break;
                        case SheetContentType.Paragraph:
                            InsertParagraph(doc, data.Content);
                            break;
                        case SheetContentType.Image:
                            InsertImage(doc,_download.DownloadFile(data.Content, data.Content.Split('/')[^1]));
                            break;
                        case SheetContentType.HyperLink:
                            InsertHyperLink(doc, data.Content);
                            break;
                    }
                }
            }

            var even = doc.Footers.Even.InsertParagraph("Page №");
            even.Alignment = Alignment.center;
            even.AppendPageNumber(PageNumberFormat.normal);
            var odd = doc.Footers.Odd.InsertParagraph("Page №");
            odd.Alignment = Alignment.center;
            odd.AppendPageNumber(PageNumberFormat.normal);

            doc.Save();
        }
        
        private void InsertContents(DocX doc, Content note)
        {
            var fontSize = 14d;
            var indentation = 4f;
            var bookmarkAnchor = note.Link.Split('#')[^1];
            if (!note.Text.Contains(BooksKeyWords.Сhapter))
            {
                indentation = 6f;
                fontSize = 12;
            }

            if (note.Text.Contains(BooksKeyWords.BookPart))
            {
                indentation = 0f;
                fontSize = 19;
            }
            CreateHyperLink(indentation,fontSize,bookmarkAnchor,note.Text,doc);
        }

        private void CreateHyperLink(float indentation, double fontSize, string bookmarkAnchor, string text, DocX doc)
        {
            var h3 = doc.AddHyperlink(text, bookmarkAnchor);
            var p3 = doc.InsertParagraph();
            p3.IndentationBefore = indentation;
            p3.AddLinkToParagraph(h3,fontSize);
        }

        private void InsertHyperLink(DocX doc, string text)
        {
            if (String.IsNullOrEmpty(text)) return;
            var paragraph = doc.InsertParagraph();
            paragraph.AppendBookmark( text );
        }
        private void InsertParagraph(DocX doc, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var paragraph = doc.InsertParagraph();
            paragraph.Append(text).IndentationFirstLine = 1;
            paragraph.SpacingAfter(5);
            paragraph.Alignment = Alignment.both;
            if (Equals(text.First(), '—'))
                paragraph.Italic();
        }

        private void InsertHeader(DocX doc, string text)
        {
            var header = doc.InsertParagraph();
            header.Append(text);
            header.FontSize(20);
            header.Alignment = Alignment.center;
            header.Bold();
            header.SpacingBefore(15);
            header.SpacingAfter(13);
        }

        private void InsertImage(DocX doc, string imgPath)
        {
            var image = doc.AddImage(imgPath);
            var picture = image.CreatePicture();
            picture.Width = 400;
            var p = doc.InsertParagraph();
            p.Alignment = Alignment.center;
            p.AppendPicture(picture);
        }

        
    }

    public interface IDocService
    {
        void Save(Book book);
        void SaveAs(Book book, string path);
    }
}