﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wikify.Archive;
using Wikify.Common;
using Wikify.Common.Network;

namespace Wikify.Sample.Archive
{
    public class WikiDownloader : IArticleArchive, IImageArchive
    {
        private HttpClient _httpClient;
        private Action _renewHttpClient;
        private ILogger _logger;

        public WikiDownloader(INetworkingProvider networkingProvider, ILogger logger)
        {
            _logger = logger;  
            _httpClient = networkingProvider.GetHttpClient();

            // assuming that the networkingProvider handles concurrency
            // no need to manage who and when triggers this
            _renewHttpClient += () => networkingProvider.GetHttpClient();
        }

        public async Task<string> GetArticleHtmlAsync(AArticleIdentifier articleIdentifier)
        {
            try
            {
                var url = articleIdentifier.GetUrl();
                var articleContent = await _httpClient.GetStringAsync(url);

                return articleContent;
            }

            // TODO: only _renewHttpClient() when appropriate exception catched.
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _renewHttpClient();
            }

            return null;
        }

        public async Task<Image> GetImageAsync(AImageIdentifier imageIdentifier)
        {
            try
            {
                var url = imageIdentifier.GetUrl();
                var imageStream = await _httpClient.GetStreamAsync(url);
                var image = Image.FromStream(imageStream, true);

                return image;
            }

            // TODO: only _renewHttpClient() when appropriate exception catched.
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                _renewHttpClient();
            }

            return null;
        }
    }
}
