using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP;
using Newtonsoft.Json;

namespace Services.ApiService.Core
{
    public class WebRequestHandler : IWebRequestHandler
    {
        public void CreateRequestAndSend<T>(string url, HTTPMethods method, Dictionary<string, string> fields, Dictionary<string, string> headers, WebRequestCallbacks<T> callbacks, ApiServiceConfig serviceConfig, string description, string rawData = null, int retryAttempts = -1, bool isWithCredentialsAndCookies = false, bool isMultiObjectResponse = false)
        {
            HTTPRequest request = new HTTPRequest(new Uri(url), method);
            request.IsCookiesEnabled = isWithCredentialsAndCookies;

            SetMaxRetries(request, retryAttempts);
            AddRawData(request, rawData);
            AddFields(request, fields);
            AddHeaders(request, headers);
            AddCallbacks<T>(url, request, callbacks, serviceConfig, description);
            request.Send();
        }

        private void SetMaxRetries(HTTPRequest request, int retryAttempts)
        {
            // Use default values in Best HTTP 
            if (retryAttempts == -1)
            {
                return;
            }

            request.MaxRetries = retryAttempts;
        }

        private void AddRawData(HTTPRequest request, string rawData)
        {
            if (rawData is null)
                return;

            request.RawData = System.Text.Encoding.UTF8.GetBytes(rawData);
        }

        private void AddFields(HTTPRequest request, Dictionary<string, string> fields)
        {
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    request.AddField(field.Key, field.Value);
                }
            }
        }

        private void AddHeaders(HTTPRequest request, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    Debug.Log($"K{header.Key}  V={header.Value} ");
                    request.SetHeader(header.Key, header.Value);
                }
            }
        }

        private void AddCallbacks<T>(string url, HTTPRequest request, WebRequestCallbacks<T> callbacks, ApiServiceConfig config, string description, bool isMultiObjectResponse = false)
        {
            request.Callback = (request, response) =>
            {
                string logtext = "";
                switch (request.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        Debug.Log("LOG :" + request.Response.DataAsText);
                        if (response.IsSuccess)
                        {
                            if (config != null && config.IsDebugFullDescription)
                            {
                                Debug.Log($"{description}  Response url = {url} " +
                                $" response: {request.Response.DataAsText}");
                            }

                            try
                            {
                                T deserializedClass = Deserialize<T>(request.Response.DataAsText, isMultiObjectResponse);
                                
                                callbacks.OnFinishedSuccess?.Invoke(request, deserializedClass);
                            }
                            catch (Exception e)
                            {
                                var dataAsText = request.Response.DataAsText;
                                Debug.LogError("Serialize error: " + e);
                                if (dataAsText == "Invalid JWT")
                                {
                                    //Invalid JWT
                                    Debug.LogError("messageData: " + dataAsText);
                                }
                                else
                                {
                                    var messageData = Deserialize<MessageData>(request.Response.DataAsText, isMultiObjectResponse);
                                    Debug.LogError("messageData: " + messageData.message);
                                    callbacks.OnSerialiseError?.Invoke(request, messageData.message);
                                }

                                callbacks.OnFinishedSuccess?.Invoke(request, default);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (config != null && config.IsDebugFullDescription)
                                {
                                    Debug.LogWarning($"Description = {description} \nRequest finished Successfully, but the server sent an error.\n" +
                                    $"Status Code: {request.Response.StatusCode}  Message: {request.Response.Message}  DataAsText: {request.Response.DataAsText}");
                                }

                                var messageData = Deserialize<MessageData>(request.Response.DataAsText, false);
                                Debug.LogError("messageData: " + messageData.message);

                                if (messageData.message == "Unauthorized")
                                {
                                    callbacks.OnUnauthorised?.Invoke(request, messageData.message);
                                }
                                else
                                {
                                    callbacks.OnFinishedFail.Invoke(request);
                                }
                            }
                            catch (Exception e)
                            {
                                callbacks.OnSerialiseError?.Invoke(request, e.Message);
                            }

                            if (callbacks.OnBeforeFinishedFail != null)
                            {
                                Debug.Log("OnBeforeFinishedFail");
                                callbacks.OnBeforeFinishedFail.Invoke(request);
                            }

                            callbacks.OnFinishedFail?.Invoke(request);
                        }

                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        logtext = "Request finished with error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception");

                        if (config.IsDebugFullDescription)
                            Debug.LogError(logtext);
                        callbacks.OnError?.Invoke(request);
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        logtext = "Request aborted!";
                        if (config.IsDebugFullDescription)
                            Debug.LogWarning(logtext);
                        callbacks.OnAborted?.Invoke(request);
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        logtext = "Connection timed out!";
                        if (config.IsDebugFullDescription)
                            Debug.LogError(logtext);
                        callbacks.OnConnectionTimedOut?.Invoke(request);
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        logtext = "Processing request timed out!";
                        if (config.IsDebugFullDescription)
                            Debug.LogError(logtext);
                        callbacks.OnTimedOut?.Invoke(request);
                        break;
                }
            };
        }

        public static T Deserialize<T>(string text, bool isMultiObjectResponse = false)
        {
            if (isMultiObjectResponse)
            {
                List<T> deserializedListClass = JsonConvert.DeserializeObject<List<T>>(text);
                return deserializedListClass[0];

            }

            T deserializedClass = JsonConvert.DeserializeObject<T>(text);
            return deserializedClass;
        }

        [Serializable]
        public class MessageData 
        {
            public string message { get; set; }
        }
    }

}
