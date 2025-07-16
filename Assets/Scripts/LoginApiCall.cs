using BestHTTP;
using Services.ApiService.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginApiCall : IApiCallWithArgs
{
    private LoginData _loginData;
    public void Send<T>(WebRequestCallbacks<T> callbacks, int retryAttempts = -1)
    {
        var config = new WebRequestConfig()
        {
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            Method = HTTPMethods.Post,
            NeedExternalApiKey = false,
            NeedServiceApiKey = false,
            NeedAuthorization = false,
            RawBody = JsonUtility.ToJson(_loginData),
            
        };

        ApiService.Send("http://alb-game-platform-dev-449269248.ap-southeast-1.elb.amazonaws.com/v1/users/login", config, callbacks, "Login", retryAttempts);
    }

    public void SetArgs<T>(T args)
    {
        if (args is Arguments arguments)
        {
            _loginData = new LoginData()
            {
                external = new LoginExternalData()
                {
                    external_jwt = arguments.JWTFromMon,
                    external_source = arguments.Source,
                },
                game = arguments.Game
            };
        }
    }

    [System.Serializable]
    public class Arguments
    {
        public string JWTFromMon;
        public string Source;
        public string Game;

    }

    [System.Serializable]
    public class LoginData
    {
        public LoginExternalData external;
        public string game;
    }

    [System.Serializable]
    public class LoginExternalData
    {
        public string external_jwt;
        public string external_source;
    }


    public class LoginResponse
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
