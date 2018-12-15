// <copyright file="SigninAuthTests.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    /// <summary>
    /// Signin card auth flow tests.
    /// </summary>
    [TestClass]
    public class SigninAuthTests
    {
        /// <summary>
        /// Tests IsRequestSigninStateVerificationQuery logic by providing a file where the invoke payload is valid.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task SigninAuthTests_IsSigninAuthValidStateVerificationInvokeAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivitySigninAuthStateVerification.json"));
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsContext) =>
                {
                    Assert.IsTrue(teamsContext.IsRequestSigninStateVerificationQuery());
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests IsRequestSigninStateVerificationQuery logic by providing a file where the invoke payload is invalid.
        /// </summary>
        /// <returns>Task tracking operation.</returns>
        [TestMethod]
        public async Task SigninAuthTests_IsSigninAuthInvalidStateVerificationInvokeAsync()
        {
            Activity sampleActivity = JsonConvert.DeserializeObject<Activity>(File.ReadAllText(@"Jsons\SampleActivityInvoke.json"));
            await TestHelpers.RunTestPipelineWithActivityAsync(
                sampleActivity,
                (teamsContext) =>
                {
                    Assert.IsFalse(teamsContext.IsRequestSigninStateVerificationQuery());
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
        }
    }
}
