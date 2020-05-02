using System;
using System.Linq;
using System.Net.Http;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Domain.Books;
using BetterRead.Shared.Helpers;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BetterRead.Shared.Infrastructure.Services
{
    internal class DocumentBuilder
    {
        private enum BuildResultType
        {
            Success,
            Failed
        }

        private class BuildResult
        {
            public BuildResult(
                BuildResultType type, 
                Action<DocX>[] buildActions)
            {
                Type = type;
                BuildActions = buildActions;
            }

            public BuildResultType Type { get; }
            public Action<DocX>[] BuildActions { get; }
        }

        private static Func<SheetContent[], (BuildResult, SheetContent[])> SheetContentBuilder(
            Predicate<SheetContentType> predicate,
            Action<SheetContent, DocX> buildAction) =>
            contents =>
            {
                if (contents.Length <= 0)
                    return (new BuildResult(BuildResultType.Failed, new Action<DocX>[0]),
                        new SheetContent[0]);

                var head = contents[0];
                var tail = contents[1..];

                if (predicate(head.ContentType))
                    return (new BuildResult(BuildResultType.Success, new[] {buildAction.Curry(head)}), tail);

                Console.WriteLine(head.Content);
                
                return (new BuildResult(BuildResultType.Failed, new Action<DocX>[0]), new SheetContent[0]);
            };

        private static Func<SheetContent[], (BuildResult, SheetContent[])> ComposeBuilders(
            Func<SheetContent[], (BuildResult, SheetContent[])> builder1,
            Func<SheetContent[], (BuildResult, SheetContent[])> builder2) =>
            contents =>
            {
                var res1 = builder1(contents);
                if (res1.Item1.Type == BuildResultType.Success)
                    return res1;

                var res2 = builder2(contents);
                if (res2.Item1.Type == BuildResultType.Success)
                    return res2;

                return (new BuildResult(BuildResultType.Failed, new Action<DocX>[0]),
                    new SheetContent[0]);
            };

        private static Func<SheetContent[], (BuildResult, SheetContent[])> ComposeBuilders(
            params Func<SheetContent[], (BuildResult, SheetContent[])>[] builders) =>
            builders.Aggregate(ComposeBuilders);

        private static Func<SheetContent[], (BuildResult, SheetContent[])> RecursiveBuilders(
            Func<BuildResult, BuildResult, BuildResult> merger,
            Func<SheetContent[], (BuildResult, SheetContent[])> builder) =>
            chars =>
            {
                var currentParseResult = builder(chars);
                if (currentParseResult.Item1.Type == BuildResultType.Failed)
                    return currentParseResult;

                var (buildResult, sheetContents) = RecursiveBuilders(merger, builder)(currentParseResult.Item2);
                return (merger(currentParseResult.Item1, buildResult), sheetContents);
            };

        private static BuildResult MergeBuildResult(BuildResult result, BuildResult buildResult) =>
            new BuildResult(BuildResultType.Success, result.BuildActions.With(buildResult.BuildActions).ToArray());

        private static void BuildSheets(DocX docx, SheetContent[] contents) =>
            ComposeBuilders(
                    SheetContentBuilder(type => type == SheetContentType.Header, InsertHeader),
                    SheetContentBuilder(type => type == SheetContentType.Image, InsertImage),
                    SheetContentBuilder(type => type == SheetContentType.Paragraph, InsertParagraph),
                    SheetContentBuilder(type => type == SheetContentType.HyperLink, InsertHyperLink))
                .PipeForward(
                    ((Func<Func<BuildResult, BuildResult, BuildResult>,
                        Func<SheetContent[], (BuildResult, SheetContent[])>,
                        Func<SheetContent[], (BuildResult, SheetContent[])>>) RecursiveBuilders)
                    .Curry(MergeBuildResult))
                .Apply(contents)
                .PipeForward(t => t.Item1.BuildActions)
                .Each(action => action(docx));
        
        public void BuildDocument(Book book)
        {
            using var doc = DocX.Create("test.docx");
            doc.DifferentFirstPage = true;
            doc.AddFooters();

            //InsertImage(doc, _download.DownloadFile(book.Info.ImageUrl, bookName));

            book.Contents.ToList().ForEach(content => InsertContents(doc, content));
            var sheetContents = book.Sheets
                .SelectMany(s => s.SheetContents);
            
            BuildSheets(doc, sheetContents.ToArray());
             
            var even = doc.Footers.Even.InsertParagraph("Page №");
            even.Alignment = Alignment.center;
            even.AppendPageNumber(PageNumberFormat.normal);
            var odd = doc.Footers.Odd.InsertParagraph("Page №");
            odd.Alignment = Alignment.center;
            odd.AppendPageNumber(PageNumberFormat.normal);

            doc.Save();
        }
        
        private static void InsertContents(DocX doc, Content note)
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

            var h3 = doc.AddHyperlink(note.Text, bookmarkAnchor);
            var p3 = doc.InsertParagraph();
            p3.IndentationBefore = indentation;
            p3.AddLinkToParagraph(h3, fontSize);
        }

        private static void InsertHyperLink(SheetContent sheetContent, DocX doc) => 
            doc.InsertParagraph().AppendBookmark(sheetContent.Content);

        private static void InsertParagraph(SheetContent sheetContent, DocX doc)
        {
            var text = sheetContent.Content;
            if (string.IsNullOrEmpty(text)) return;
            var paragraph = doc.InsertParagraph();
            paragraph.Append(text).IndentationFirstLine = 1;
            paragraph.SpacingAfter(5);
            paragraph.Alignment = Alignment.both;
            if (Equals(text.First(), '—'))
                paragraph.Italic();
        }

        private static void InsertHeader(SheetContent sheetContent, DocX doc) =>
            doc.InsertParagraph()
                .Append(sheetContent.Content)
                .FontSize(20)
                .Bold()
                .SpacingBefore(15)
                .SpacingAfter(13)
                .Alignment = Alignment.center;

        private static void InsertImage(SheetContent sheetContent, DocX doc)
        {
            using var httpClient = new HttpClient();
            var imageStream = httpClient.GetStreamAsync(sheetContent.Content)
                .GetAwaiter()
                .GetResult()
                .ToMemoryStream();
            
            var image = doc.AddImage(imageStream);
            var picture = image.CreatePicture();
            picture.Width = 200;
            var p = doc.InsertParagraph();
            p.Alignment = Alignment.center;
            p.AppendPicture(picture);
        }
    }
}