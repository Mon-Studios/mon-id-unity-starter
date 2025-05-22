using System.Collections.Generic;
using BestHTTP;
using Services.ApiService.Core;

public interface IWebRequestHandler
{
    void CreateRequestAndSend<T>(string url, HTTPMethods method, Dictionary<string, string> fields, Dictionary<string, string> headers, WebRequestCallbacks<T> callbacks, ApiServiceConfig serviceConfig, string description, string rawData = null, int retryAttempts = -1, bool isWithCredentialsAndCookies = false, bool isMultiObjectResponse = false);
}
