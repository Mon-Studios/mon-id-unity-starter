
using UnityEngine;

using Services.ApiService.Core;
using System.Threading.Tasks;
using System;
namespace Thirdweb.Unity.Examples
{
    public class LoginController : MonoBehaviour
    {
        private ApiCaller apiCaller;

        public string JwtFromMonId;


        private void Awake()
        {
            apiCaller = new ApiCaller();
            ApiService.Initialise();
        }

        [ContextMenu("Send Request")]
        public void SendWebRequest()
        {
            var args = new LoginApiCall.Arguments() { Source = "monid", JWTFromMon = JwtFromMonId, Game = "pixelmon-tcg" };

            apiCaller.MakeApiCall<LoginApiCall, LoginApiCall.Arguments, LoginApiCall.LoginResponse>(args, OnSuccess, OnFail);
        }

        public async Task<LoginApiCall.LoginResponse> SendWebRequestAsync()
        {
            var args = new LoginApiCall.Arguments()
            {
                Source = "monid",
                JWTFromMon = JwtFromMonId,
                Game = "pixelmon-tcg"
            };

            var tcs = new TaskCompletionSource<LoginApiCall.LoginResponse>();

            void OnSuccess(LoginApiCall.LoginResponse response)
            {
                tcs.TrySetResult(response);
            }

            void OnFail()
            {
                Debug.Log("fail");
            }

            apiCaller.MakeApiCall<LoginApiCall, LoginApiCall.Arguments, LoginApiCall.LoginResponse>(
                args, OnSuccess, OnFail
            );


            return await tcs.Task;
        }

        private void OnFail()
        {
            Debug.Log("Failed to login");
        }

        private void OnSuccess(LoginApiCall.LoginResponse response)
        {
            Debug.Log(response.user_id);
            Debug.Log(response.access_token);
        }
    }
}






