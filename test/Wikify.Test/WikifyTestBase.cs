﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikify.Archive;
using Wikify.Common.Content;
using Wikify.Common.Id;
using Wikify.Common.Network;
using Wikify.License;
using Wikify.License.Copyright;
using Wikify.Parsing.Content;
using Wikify.Parsing.MwParser;

namespace Wikify.Test
{
    public abstract class WikifyTestBase
    {
        private static readonly ServiceProvider _serviceProvider;

        static WikifyTestBase()
        {
            var services = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"Logs/TestLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSingleton<INetworkingProvider, NetworkingProvider>();
            services.AddSingleton<IArticleIdentifierFactory, ArticleIdentifierFactory>();
            services.AddSingleton<IArticleArchive, MediaWikiArticleDownloader>();
            services.AddSingleton<IArticleLicenseProvider, ArticleLicenseProvider>();
            services.AddSingleton<ILicenseFactory, LicenseFactory>();
            services.AddSingleton<ICopyrightResolver, CopyrightResolver>();
            services.AddSingleton<ICopyrightFactory, CopyrightFactory>();
            services.AddSingleton<IWikiMediaFactory, WikiMediaFactory>();
            services.AddSingleton<IAstTranslator, MwAstTranslator>();
            services.AddSingleton<IWikiContentFactory, WikiContentFactory>();
            services.AddLogging(x => x.AddSerilog(dispose: true));

            _serviceProvider = services.BuildServiceProvider();
            ;
        }

        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}