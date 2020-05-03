using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Domain.Books;
using BetterRead.Shared.Helpers;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BetterRead.Shared.Infrastructure.Services
{
    // why not just use static class? ReSharper is right, you really never instantiate it. :)
    // also, probably good idea will be to specify in more details why exactly you disabling validation    
    
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class BookDocumentBuilder
    {
        public static async Task<byte[]> Build(Book book)
        {
            // c# 8.0 is nice. :)
            await using var ms = new MemoryStream();
            var doc = DocX.Create(ms);
            var sheetContents = book.Sheets
                .SelectMany(s => s.SheetContents)
                .ToArray();
            
            doc.DifferentFirstPage = true;
            doc.AddFooters();

            await InsertImage(book.Info.ImageUrl, doc);

            // this extension method is pure side-effect. Which doesn't follow functional paradigm that well
            book.Contents.Each(content => InsertContents(content, doc));
          
            BuildSheets(doc, sheetContents);
             
            doc.Footers.Even.InsertParagraph("Page №")
                .AppendPageNumber(PageNumberFormat.normal)
                .Alignment = Alignment.center;
            doc.Footers.Odd.InsertParagraph("Page №")
                .AppendPageNumber(PageNumberFormat.normal)
                .Alignment = Alignment.center;
            
            doc.Save();
            return ms.ToArray();
        }

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
                // value tuples sucks
                .PipeForward(t => t.Item1.BuildActions)
                .Each(action => action(docx));

        // function name should probably contain a verb
        private static Func<SheetContent[], (BuildResult, SheetContent[])> SheetContentBuilder(
            Predicate<SheetContentType> predicate,
            Action<SheetContent, DocX> buildAction) =>
            contents =>
            {
                if (contents.Length <= 0)
                    return (
                        new BuildResult(BuildResultType.Failed, new Action<DocX>[0]), 
                        new SheetContent[0]);

                // First & Skip would work as well. :)
                var head = contents[0];
                var tail = contents[1..];

                // exception handling?
                if (predicate(head.ContentType))
                    return (
                        new BuildResult(BuildResultType.Success, new[] {buildAction.Curry(head)}), 
                        tail);

                return (
                    new BuildResult(BuildResultType.Failed, new Action<DocX>[0]), 
                    new SheetContent[0]);
            };

        private static Func<SheetContent[], (BuildResult, SheetContent[])> ComposeBuilders(
            Func<SheetContent[], (BuildResult, SheetContent[])> builder1,
            Func<SheetContent[], (BuildResult, SheetContent[])> builder2) =>
            contents =>
            {
                // you can extract additional function to avoid code duplication and nasty Item1 usage
                var res1 = builder1(contents);
                if (res1.Item1.Type == BuildResultType.Success)
                    return res1;

                var res2 = builder2(contents);
                if (res2.Item1.Type == BuildResultType.Success)
                    return res2;

                return (
                    new BuildResult(BuildResultType.Failed, new Action<DocX>[0]),
                    new SheetContent[0]);
            };

        private static Func<SheetContent[], (BuildResult, SheetContent[])> ComposeBuilders(
            params Func<SheetContent[], (BuildResult, SheetContent[])>[] builders) =>
            builders.Aggregate(ComposeBuilders);

        // verb again. It is not a collection of builders, it is operation of some kind. Which isn't reflected in name
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
            new BuildResult(
                BuildResultType.Success, 
                // breaks deferred evaluation
                result.BuildActions.With(buildResult.BuildActions).ToArray());

        private static void InsertHyperLink(SheetContent sheetContent, DocX doc) => 
            doc.InsertParagraph()
                .AppendBookmark(sheetContent.Content);

        private static void InsertParagraph(SheetContent sheetContent, DocX doc) =>
            doc.InsertParagraph()
                .Append(sheetContent.Content)
                .SpacingAfter(5)
                .PipeForward(paragraph =>
                {
                    paragraph.IndentationFirstLine = 1;
                    paragraph.Alignment = Alignment.both;
                });

        private static void InsertHeader(SheetContent sheetContent, DocX doc) =>
            doc.InsertParagraph()
                .Append(sheetContent.Content)
                .FontSize(20)
                .Bold()
                .SpacingBefore(15)
                .SpacingAfter(13)
                .Alignment = Alignment.center;

        private static void InsertImage(SheetContent sheetContent, DocX doc) =>
            InsertImage(sheetContent.Content, doc)
                // this is unnecessary
                .PipeForward(task => task.GetAwaiter().GetResult());
        
        private static async Task InsertImage(string url, Document doc) => 
            (await new HttpClient().GetStreamAsync(url))
                .PipeForward(stream => stream.ToMemoryStream())
                // you should probably check aspect ratio of loaded image and 
                .PipeForward(stream => doc.AddImage(stream).CreatePicture(400, 400))
                .PipeForward(picture => doc.InsertParagraph()
                    .AppendPicture(picture)
                    .Alignment = Alignment.center);
        
        private static void InsertContents(Content note, Document doc)
        {
            var bookmarkAnchor = note.Link.Split('#')[^1];
            var (fontSize, indentation) = note.Text switch
            {
                var str when str.Contains(BooksKeyWords.Сhapter) => (6f, 12),
                var str when str.Contains(BooksKeyWords.BookPart) => (0f, 19),
                _ => (14d, 4f)
            };
            
            var h3 = doc.AddHyperlink(note.Text, bookmarkAnchor);
            var p3 = doc.InsertParagraph();
            p3.IndentationBefore = indentation;
            p3.AddLinkToParagraph(h3, fontSize);
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
        
        private enum BuildResultType
        {
            Success,
            Failed
        }
    }
}