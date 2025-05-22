using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.ApiService.Core
{
    [Serializable]
    public struct ApiEnvironmentVariables
    {
        public string ApiUrl;
        public string AlternateApiUrl;
        public string ExternalApiKey;
        public string ServiceApiKey;
        public string Env;
    }

    public static class ApiService
    {
        private static ApiEnvironmentVariables _environmentVariables;
        private static ApiServiceConfig _serviceConfig;
        private static string _token;
        private static IWebRequestHandler _webRequestHandler;

        public static void Initialise(ApiEnvironmentVariables envVariables, ApiServiceConfig serviceConfig)
        {
            //SetApiEnvironmentVariables(envVariables);
            //SetServiceConfig(serviceConfig);
            _webRequestHandler = new WebRequestHandler();
        }

        public static void Initialise()
        {
            _webRequestHandler = new WebRequestHandler();
        }

        public static void SetApiEnvironmentVariables(ApiEnvironmentVariables envVariables)
        {
            _environmentVariables = envVariables;
        }

        public static ApiEnvironmentVariables GetApiEnvironmentVariables()
        {
            return _environmentVariables;
        }

        public static void SetServiceConfig(ApiServiceConfig serviceConfig)
        {
            _serviceConfig = serviceConfig;
        }

        public static void SetToken(string token)
        {
            _token = token;
        }

        public static string GetToken()
        {
            return _token;
        }
        
        public static void Send<T>(string baseUrl, WebRequestConfig config, WebRequestCallbacks<T> callbacks, string description, int retryAttempts = -1, bool isMultiObjectResponse = false)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                Debug.LogError("BaseURL is missing");
                return;
            }

            if (config.NeedExternalApiKey)
            {
                if (string.IsNullOrEmpty(_environmentVariables.ExternalApiKey))
                {
                    Debug.LogError("External Api key is missing");
                    return;
                }

                if (config.Headers == null)
                {
                    config.Headers = new Dictionary<string, string>();
                }

                config.Headers.Add("x-external-api-key", _environmentVariables.ExternalApiKey);
            }

            if (config.NeedServiceApiKey)
            {
                if (string.IsNullOrEmpty(_environmentVariables.ServiceApiKey))
                {
                    Debug.LogError("Service Api key is missing");
                    return;
                }

                if (config.Headers == null)
                {
                    config.Headers = new Dictionary<string, string>();
                }

                config.Headers.Add("x-service-api-key", _environmentVariables.ServiceApiKey);
            }

            if (config.NeedAuthorization)
            {
                if (string.IsNullOrEmpty(_token))
                {
                    Debug.LogError("Token is missing");
                    return;
                }

                if (config.Headers == null)
                {
                    config.Headers = new Dictionary<string, string>();
                }

                config.Headers.Add("Cookie", $"auth_token={_token}");

                if (config.Headers.ContainsKey("Authorization") == false)
                {
                    config.Headers.Add("Authorization", $"Bearer {_token}");
                }
            }

            _webRequestHandler.CreateRequestAndSend<T>(baseUrl + config.EndPoint, config.Method, config.Fields, config.Headers, callbacks, _serviceConfig, description, config.RawBody, retryAttempts, isMultiObjectResponse);
        }
    }
}
