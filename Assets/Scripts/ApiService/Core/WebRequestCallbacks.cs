using System;
using BestHTTP;

namespace Services.ApiService.Core
{
    public class WebRequestCallbacks<T>
    {
        public Action<HTTPRequest, T> OnFinishedSuccess;
        public Action<HTTPRequest, T> OnBeforeFinishedSuccess;
        public Action<HTTPRequest, string> OnSerialiseError;
        public Action<HTTPRequest, string> OnUnauthorised;
        public Action<HTTPRequest> OnFinishedFail;
        public Action<HTTPRequest> OnBeforeFinishedFail;
        public Action<HTTPRequest> OnError;
        public Action<HTTPRequest> OnAborted;
        public Action<HTTPRequest> OnConnectionTimedOut;
        public Action<HTTPRequest> OnTimedOut;
    }
}
