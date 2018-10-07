using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Abstractions.Teams.Invoke;
using Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine.Models;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    public class WikipediaSearchHandler : ISearchHandler
    {
        /// <summary>
        /// Gets the url of Wikipedia search API.
        /// </summary>
        /// <value>
        /// The url of Wikipedia search API.
        /// </value>
        private const string WikiSearchUrlFormat = "https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch={0}&srlimit={1}&sroffset={2}&format=json&formatversion=2";

        /// <summary>
        /// Gets the url of image search.
        /// </summary>
        /// <value>
        /// The url of image search API.
        /// </value>
        private const string ImageSearchUrl = "https://en.wikipedia.org/w/api.php?action=query&formatversion=2&format=json&prop=pageimages&piprop=thumbnail&pithumbsize=400&pageids=";

        /// <summary>
        /// Gets the default url of image.
        /// </summary>
        /// <value>
        /// The default url of image.
        /// </value>
        private const string DefaultImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b3/Wikipedia-logo-v2-en.svg/1200px-Wikipedia-logo-v2-en.svg.png";

        private readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Gets the search result from wikipedia.
        /// </summary>
        /// <param name="messagingExtensionActivityAction">The messaging extension activity action.</param>
        /// <returns></returns>
        public async Task<ComposeExtensionResult> GetSearchResultAsync(MessagingExtensionActivityAction messagingExtensionActivityAction)
        {
            ComposeExtensionResult composeExtensionResult = new ComposeExtensionResult
            {
                Type = "result",
                AttachmentLayout = "list",
                Attachments = new List<ComposeExtensionAttachment>()
            };
            IList<WikipediaResult> searchResults = new List<WikipediaResult>();

            // Search Wikipedia
            string apiUrl = GenerateSearchAPIUrl(messagingExtensionActivityAction.ComposeExtensionQuery);
            WikipediaQueryResult queryResult = await this.InvokeWikipediaAPIAsync(apiUrl);
            searchResults = queryResult.Query.Results;

            // Grab pageIds so that we can batch query to fetch image urls of the pages
            IList<string> pageIds = new List<string>();
            foreach (WikipediaResult searchResult in searchResults)
            {
                pageIds.Add(searchResult.Pageid);
            }

            IDictionary<string, string> imageResults = await this.GetImageUrlAsync(pageIds);

            // Genereate results
            foreach (WikipediaResult searchResult in searchResults)
            {
                string imageUrl = DefaultImageUrl; // Always set a default image url in case of failure, or image doesn't exist
                if (imageResults.ContainsKey(searchResult.Pageid))
                {
                    imageUrl = imageResults[searchResult.Pageid];
                }

                ThumbnailCard previewCard = new ThumbnailCard
                {
                    Title = HttpUtility.HtmlEncode(searchResult.Title),
                    Text = searchResult.Snippet
                };
                previewCard.Images = new CardImage[] { new CardImage(imageUrl, searchResult.Title) };

                // Generate cards with links in the titles - preview cards don't have links
                ThumbnailCard card = new ThumbnailCard
                {
                    Title = "<a href='" +
                        HttpUtility.HtmlAttributeEncode("https://en.wikipedia.org/wiki/" +
                        Uri.EscapeDataString(searchResult.Title)) +
                        "' target='_blank'>" +
                        HttpUtility.HtmlEncode(searchResult.Title) +
                        "</a>",
                    Text = searchResult.Snippet,
                    Images = previewCard.Images
                };
                composeExtensionResult.Attachments.Add(card.ToAttachment().ToComposeExtensionAttachment(previewCard.ToAttachment()));
            }

            return composeExtensionResult;
        }

        private async Task<IDictionary<string, string>> GetImageUrlAsync(IList<string> pageIds)
        {
            string pageIdQuery = string.Join("|", pageIds);
            IDictionary<string, string> result = new Dictionary<string, string>();
            WikipediaQueryResult queryResult = await this.InvokeWikipediaAPIAsync(ImageSearchUrl + pageIdQuery);
            if (queryResult != null && queryResult.Query != null)
            {
                foreach (WikipediaPage page in queryResult.Query.Pages)
                {
                    if (page.Thumbnail != null)
                    {
                        result.Add(page.Pageid, page.Thumbnail.Source);
                    }
                }
            }

            return result;
        }

        private static string GenerateSearchAPIUrl(ComposeExtensionQuery query)
        {
            return string.Format(
                WikiSearchUrlFormat,
                Uri.EscapeDataString(query.Parameters[0].Value.ToString()),
                query.QueryOptions.Count,
                query.QueryOptions.Skip);
        }

        private async Task<WikipediaQueryResult> InvokeWikipediaAPIAsync(string apiUrl)
        {
            var response = await this.httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WikipediaQueryResult>(responseBody);
        }
    }
}
