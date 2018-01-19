using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;

namespace Microsoft.Bot.Connector.Teams
{
    internal class BotFrameworkErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            if (ex as HttpOperationException != null && (ex as HttpOperationException).Response != null)
            {
                if ((int)(ex as HttpOperationException).Response.StatusCode == 429)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
