using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeAutomation.Tools
{
    public static class ExtensionMethods
    {
        public static void RefreshToken(this UserCredential credential)
        {
            if (credential.Token.IsStale)
                credential.RefreshTokenAsync(CancellationToken.None);
        }

    }
}
