﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wikify.Archive;
using Wikify.Common.Content;
using Wikify.Common.Id;
using Wikify.License;
using Wikify.Parsing.Content;

namespace Wikify.Test.Archive
{
    [TestClass]
    public class ArticleDownloaderTest
    {
        private static LoggerFactory _loggerFactory;
        private static ArticleDownloader _articleDownloader;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _loggerFactory = new LoggerFactory();
            _articleDownloader = new ArticleDownloader(
                _loggerFactory.CreateLogger<ArticleDownloader>(),
                new Wikify.Common.Network.NetworkingProvider(),
                new LicenseProvider(),
                new WikiMediaFactory());
        }

        //[TestMethod]
        //[DataRow("https://en.wikipedia.org/wiki/Giorgio_Moroder")]
        //public async Task TestDownloadArticleAsync(string url)
        //{
        //    await _articleDownloader.GetMediaAsync(new Identifier(url));
        //    ;
        //}

    }
}
