// <copyright file="ComposeExtensionTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    /// <summary>
    /// Compose extension tests.
    /// </summary>
    [TestClass]
    public class ComposeExtensionTests
    {
        /// <summary>
        /// Tests IsComposeExtension logic by providing a valid compose extension file.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ComposeExtension_IsComposeExtensionValidComposeExtensionAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityComposeExtension.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Assert.IsTrue(teamsExtension.IsRequestComposeExtensionQuery());
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests IsComposeExtension logic by providing an invalid compose extension file.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ComposeExtension_IsComposeExtensionInvalidComposeExtensionAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityInvoke.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    Assert.IsFalse(teamsExtension.IsRequestComposeExtensionQuery());
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests get compose extension data logic.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task ComposeExtension_GetComposeExtensionDataAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityComposeExtension.json"));

            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsExtension) =>
                {
                    ComposeExtensionQuery query = teamsExtension.GetComposeExtensionQueryData();
                    Assert.AreEqual("testQuery", query.CommandId);
                    Assert.IsTrue(query.Parameters != null && query.Parameters.Count == 1);
                    Assert.AreEqual("selectedQueryJson", query.Parameters[0].Name);
                    Assert.AreEqual("Value", query.Parameters[0].Value.ToString());

                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }
    }
}
