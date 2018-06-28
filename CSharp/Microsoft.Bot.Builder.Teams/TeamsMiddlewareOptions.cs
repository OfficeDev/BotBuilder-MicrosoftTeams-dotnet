// <copyright file="TeamsMiddlewareOptions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Teams Middleware Options.
    /// </summary>
    public class TeamsMiddlewareOptions
    {
        /// <summary>
        /// The whitelisted tenant dictionary. This is a dictionary representation of Whitelisted Tenants.
        /// </summary>
        private Dictionary<string, string> whitelistedTenantDictionary;

        /// <summary>
        /// Gets or sets a value indicating whether tenant filtering is enabled.
        /// </summary>
        public bool EnableTenantFiltering { get; set; } = false;

        /// <summary>
        /// Gets or sets the whitelisted tenants. Activities from these tenants will be accepted, everything else will be disregarded.
        /// </summary>
        public IEnumerable<string> WhitelistedTenants { get; set; } = new List<string>();

        /// <summary>
        /// Gets the whitelisted tenant dictionary. This is a dictionary representation of Whitelisted Tenants.
        /// This is just to enable faster lookups.
        /// </summary>
        internal Dictionary<string, string> WhitelistedTenantDictionary
        {
            get
            {
                if (this.whitelistedTenantDictionary == null)
                {
                    // Using IgnoreCase comparer here to ensure we can compare across case in incoming requests.
                    this.whitelistedTenantDictionary = new Dictionary<string, string>(this.WhitelistedTenants.Count(), StringComparer.OrdinalIgnoreCase);
                    foreach (string tenantId in this.WhitelistedTenants)
                    {
                        if (Guid.TryParse(tenantId, out Guid guidTenantId))
                        {
                            this.whitelistedTenantDictionary.Add(guidTenantId.ToString(), tenantId);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid Tenant Id '" + tenantId + "'. Tenant Id must a valid Guid", nameof(this.WhitelistedTenants));
                        }
                    }
                }

                return this.whitelistedTenantDictionary;
            }
        }
    }
}
