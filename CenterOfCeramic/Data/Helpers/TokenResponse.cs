﻿namespace CenterOfCeramic.Data.Helpers
{
    public class TokenResponse
    {
        public string Token { get; set; }

        public TokenResponse()
        {

        }
        public TokenResponse(string token)
        {
            this.Token = token;
        }
    }
}
