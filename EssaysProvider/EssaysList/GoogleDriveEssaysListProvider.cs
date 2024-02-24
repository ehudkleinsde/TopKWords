using Common.Config;
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
            List<Uri> essaysList;

            try
            {
                HttpResponseMessage response = await client.GetAsync(_essaysListUri);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
                essaysList = PopulateQueue(content);
            }
            catch (Exception ex)
            {
                _logger.LogFatalError(nameof(GetEssaysListAsync), $"Unable to get essays list. Exception: {ex.GetType()}, {ex.Message}");
                throw;
            }

            _logger.LogInfo(nameof(GetEssaysListAsync), $"Got {essaysList.Count} essays.");

            return essaysList;
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
            string cleanLine;

            foreach (string line in lines)
            {
                cleanLine = CleanUri(line);

                try
                {
                    uri = new Uri(cleanLine);
                    result.Add(uri);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(nameof(PopulateQueue), $"Failed to convert string to Uri: {cleanLine}, Exception: {ex.GetType()}, {ex.Message}");
                }
            }

            return result;
        }

        private string CleanUri(string uri)
        {
            uri = uri.TrimEnd();
            uri = uri.TrimEnd('/');
            uri = uri.TrimStart();

            return uri;
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
