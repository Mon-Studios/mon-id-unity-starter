using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using BestHTTP;
using System;
using Services.ApiService.Core;
public class LoginController : MonoBehaviour
{
    public ApiCaller apiCaller;

    private void Awake()
    {
        apiCaller = new ApiCaller();
        ApiService.Initialise();
    }

    public string JwtFromMonId;


    [ContextMenu("Send Request")]
    public void SendWebRequest()
    {
        var args = new LoginApiCall.Arguments() { Source = "monid", JWTFromMon = JwtFromMonId, Game = "pixelmon-tcg" };

        apiCaller.MakeApiCall<LoginApiCall, LoginApiCall.Arguments, LoginApiCall.LoginResponse>(args, OnSuccess, OnFail);

            
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





