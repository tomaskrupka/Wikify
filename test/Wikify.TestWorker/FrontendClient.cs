using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Wikify.Archive;
using Wikify.Common;
using Wikify.Display;
using Wikify.Parsing;

namespace Wikify.TestWorker
{
    [TestClass]
    public class FrontendClient
    {
        [TestMethod]
        public async Task FullRoutineAsync()
        {
            var wikiObjectParser = new ObjectIdParser();
            var articleId = wikiObjectParser.GetObjectIdentifier("") as AArticleIdentifier;

            var articleProvider = new WikiArticleProvider();
            var html = await articleProvider.GetArticleHtmlAsync(articleId);

            var htmlProcessor = new HtmlProcessor();
            var article = await htmlProcessor.ProcessHtmlAsync(html);

            var imageGenerator = new ImageGenerator();
            imageGenerator.CreateArticleImage(article, null);
        }
    }
}
