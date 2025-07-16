using System.Collections.Generic;
using BestHTTP;

namespace Services.ApiService.Core
{
    public class WebRequestConfig
    {
        public string EndPoint;
        public HTTPMethods Method;
        public Dictionary<string, string> Fields;
        public Dictionary<string, string> Headers;
        public bool NeedAuthorization;
        public bool NeedExternalApiKey;
        public bool NeedServiceApiKey;
        public string RawBody;
    }
}
