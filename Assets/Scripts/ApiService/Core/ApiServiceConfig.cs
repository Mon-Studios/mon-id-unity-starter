using UnityEngine;

namespace Services.ApiService.Core
{
    [CreateAssetMenu(menuName = "ApiService/ApiServiceConfig")]
    public class ApiServiceConfig : ScriptableObject
    {

        public bool IsMockData;
        public bool IsLocalHost;
        public bool IsStaging;
        public ApiEnvironmentVariables LocalHostVariables;
        public ApiEnvironmentVariables StagingVariables;
        public string AuthToken;
        public bool IsDebugURL;
        public bool IsDebugFullDescription;
    }
}
