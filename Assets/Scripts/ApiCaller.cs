using System;
using System.Diagnostics;
using Services.ApiService.Core;

// Facade for all Api calls
public class ApiCaller : IApiCaller
{
    public void MakeApiCall<T, TResponse>(Action<TResponse> onSuccess, Action onFail, int retryAttempts = -1) where T : IApiCallWithoutArgs, new()
    {
        var callbacks = CreateCallbacks<TResponse>(onSuccess, onFail);
        var apiCall = new T();
        apiCall.Send(callbacks, retryAttempts);
    }

    public void MakeApiCall<T, TArgs, TResponse>(TArgs args, Action<TResponse> onSuccess, Action onFail, int retryAttempts = -1) where T: IApiCallWithArgs, new()
    {
        var callbacks = CreateCallbacks<TResponse>(onSuccess, onFail);
        var apiCall = new T();
        apiCall.SetArgs<TArgs>(args);
        apiCall.Send(callbacks, retryAttempts);
    }

    private WebRequestCallbacks<T> CreateCallbacks<T>(Action<T> onSuccess, Action onFail)
    {
        var callbacks = new WebRequestCallbacks<T>()
        {
            OnFinishedSuccess = (request, response) => 
            {
                onSuccess?.Invoke(response);
            },

            OnFinishedFail = (request) =>
            {
                onFail?.Invoke();
            },

            OnSerialiseError = (request, message) =>
            {
                UnityEngine.Debug.LogError("Serialization error " + message);
            },

            OnUnauthorised = (request, message) =>
            {
                UnityEngine.Debug.LogError("Serialization error " + message);
            },

            OnError = (request) => 
            {
                onFail?.Invoke();

                UnityEngine.Debug.LogError("Request finished with error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
            }
        };

        return callbacks;
    }
}
