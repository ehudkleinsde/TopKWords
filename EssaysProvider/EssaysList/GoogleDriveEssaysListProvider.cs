﻿using EssaysProvider.Config;
using Logger;

namespace EssaysProvider.EssaysList
{
    public class GoogleDriveEssaysListProvider : IEssaysListProvider
    {
        private Uri _essaysListUri;
        private ILogger _logger;

        public GoogleDriveEssaysListProvider(IEssaysProviderConfigProvider essaysProviderConfigProvider, ILogger logger)
        {
            _logger = logger;
            _essaysListUri = ConvertGoogleDriveSharingUriToDownloadUri(essaysProviderConfigProvider.GetEssaysListUri());
        }

        public async Task<List<Uri>> GetEssaysListAsync()
        {
            HttpClient client = new HttpClient();
            string content;
            List<Uri> essaysQueue;

            try
            {
                HttpResponseMessage response = await client.GetAsync(_essaysListUri);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
                essaysQueue = PopulateQueue(content);
            }
            catch (Exception ex)
            {
                _logger.LogFatalError(nameof(GetEssaysListAsync), $"Unable to get essays list. Exception: {ex.GetType()}, {ex.Message}");
                throw;
            }

            _logger.LogInfo(nameof(GetEssaysListAsync), $"Got {essaysQueue.Count} essays.");

            return essaysQueue;
        }

        private List<Uri> PopulateQueue(string content)
        {
            if (content == null || content == string.Empty)
            {
                throw new ArgumentException(content, "content");
            }

            List<Uri> result = new();
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            Uri uri;

            foreach (string line in lines)
            {
                try
                {
                    uri = new Uri(line);
                    result.Add(uri);
                    _logger.LogInfo(nameof(PopulateQueue), $"Got essay - {line}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(nameof(PopulateQueue), $"Failed to convert string to Uri: {line}, Exception: {ex.GetType()}, {ex.Message}");
                }
            }

            return result;
        }

        private Uri ConvertGoogleDriveSharingUriToDownloadUri(Uri uri)
        {
            string uriStr = uri.ToString();
            uriStr = uriStr.Replace("https://drive.google.com/file/d/", string.Empty);
            string[] segments = uriStr.Split(new char[] { '/' });
            Uri downloadUri = new("https://drive.google.com/uc?export=download&id=" + segments[0]);
            _logger.LogInfo(nameof(ConvertGoogleDriveSharingUriToDownloadUri), $"Google drive download link: {downloadUri.AbsoluteUri}");
            return downloadUri;
        }
    }
}
