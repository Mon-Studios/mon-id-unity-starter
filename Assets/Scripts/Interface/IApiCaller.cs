
using System;
using Services.ApiService.Core;

public interface IApiCaller 
{
    public void MakeApiCall<T, TResponse>(Action<TResponse> onSuccess, Action onFail, int retryAttempts = -1) where T : IApiCallWithoutArgs, new();

    public void MakeApiCall<T, TArgs, TResponse>(TArgs args, Action<TResponse> onSuccess, Action onFail, int retryAttempts = -1) where T: IApiCallWithArgs, new();
}

public interface IApiCallWithoutArgs : IApiCall
{

}

public interface IApiCallWithArgs : IApiCall, IArgsSettable
{

}