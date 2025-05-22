namespace Services.ApiService.Core
{
    public interface IApiCall
    {
        void Send<T>(WebRequestCallbacks<T> callbacks, int retryAttempts = -1);
    }
}
