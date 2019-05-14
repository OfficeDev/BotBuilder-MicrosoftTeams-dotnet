// <copyright file="MessageActionsPayloadBodyExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using System.Collections.Generic;
    using HtmlAgilityPack;

    /// <summary>
    /// MessageActionsPayloadBody extensions.
    /// </summary>

    public static class MessageActionsPayloadBodyExtensions
    {
        /// <summary>
        /// Strip HTML tags from MessageActionsPayloadBody content.
        /// </summary>
        /// <param name="body">The MessageActionsPayloadBody.</param>
        /// <returns>Plain text content.</returns>
        public static string GetPlainTextContent(this MessageActionsPayloadBody body)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(body.content);
            var TextRestrictedHtmlTags = new HashSet<string> { "at", "attachment" };
            return StripHtmlTagsHelper(doc.DocumentNode, TextRestrictedHtmlTags);
        }

        private static string StripHtmlTagsHelper(HtmlNode node, ISet<string> tags)
        {
            string result = "";
            if (tags.Contains(node.Name))
            {
                result += node.OuterHtml;
            }
            else
            {
                foreach (HtmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == HtmlNodeType.Text)
                    {
                        result += childNode.InnerText;
                    }
                    else
                    {
                        result += StripHtmlTagsHelper(childNode, tags);
                    }
                }
            }
            return result;
        }                            
    }
}
