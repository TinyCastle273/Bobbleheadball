using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json;

public class ApiManager : SingletonMono<ApiManager>
{
    public static string game_id = "bobblehead_ball";
    const string base_url = "https://prod.roseon.finance/roseon/point-system/";

    // got this token from somewhere???
    private string token = "2691ee6c351344719a986200d107b5ac";
    public string Token
    {
        get { return token; }
        set { token = value; }
    }

    #region Process API
    private JsonSerializerSettings apiSerializerSetting = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) {
            UnityEngine.Debug.LogException(args.ErrorContext.Error);
            args.ErrorContext.Handled = true;
        }
    };

    private void RequestAPI<T>(APIDefine api, string param = null, object json = null, Action<Result<T>> OnSucces = null, Action<ResponseError> OnError = null)
    {
        string url = GetUrl(api, param);
#if UNITY_EDITOR
        Debug.LogError("request url: " + url);
#endif
        UnityWebRequest request = null;
        switch (api.method) { 
            case APIMethod.GET:
            case APIMethod.GET_QUERY:
                {
                    request = UnityWebRequest.Get(url);
                } break;
            case APIMethod.POST:
                {
                    WWWForm form = new WWWForm();
                    request = UnityWebRequest.Post(url, form);
                    if (json != null)
                    {
                        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                        request.downloadHandler = new DownloadHandlerBuffer();
                        request.SetRequestHeader("Content-Type", "application/json");
                    }
                } break;
            case APIMethod.PUT:
                {
                    if (json != null)
                    {
                        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                        request = UnityWebRequest.Put(url, bodyRaw);
                        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                        request.downloadHandler = new DownloadHandlerBuffer();
                        request.SetRequestHeader("Content-Type", "application/json");
                    }
                    break;
                }
        }

        // Set content type if needed
        if (!string.IsNullOrEmpty(api.contentType))
        {
            request.SetRequestHeader("Content-Type", api.contentType);
        }

        request.certificateHandler = new SelfSignedCertificateHandler();

        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("x-access-token", Token);
        StartCoroutine(IERequest<T>(request, OnSucces, OnError));
    }

    private IEnumerator IERequest<T>(UnityWebRequest request, Action<Result<T>> OnSucces = null, Action<ResponseError> OnError = null)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OnError?.Invoke(new ResponseError { errorCode = "", message = "Cannot Connect To Internet"});
            request.Abort();
            yield break;
        }

        if (request != null)
        {
            yield return request.SendWebRequest();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.LogError("res: " + request.downloadHandler.text);
#endif
            if (request.responseCode == 200)
            {
                var res = JsonConvert.DeserializeObject<Result<T>>(request.downloadHandler.text, apiSerializerSetting);
                OnSucces?.Invoke(res);
            }
            else
            {
                var error = JsonConvert.DeserializeObject<ResponseError>(request.downloadHandler.text, apiSerializerSetting);
                OnError.Invoke(error);
            }
        }
        else if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            var error = JsonConvert.DeserializeObject<ResponseError>(request.downloadHandler.text, apiSerializerSetting);
            OnError.Invoke(error);
        }
        else
        {
            OnError?.Invoke(new ResponseError { errorCode = "", message = request.result.ToString() });
        }
        request.Dispose();
    }
    
    private string GetUrl(APIDefine api, string param)
    {
        string url = null;
        if (api.method == APIMethod.GET_QUERY)
        {
            var query = string.Format(api.action, param);
            url = string.Format("{0}?{1}", api.prefix,  query);
        }
        else if (!string.IsNullOrEmpty(api.prefix))
        {
            url = string.Format("{0}/{1}", api.prefix, api.action);
            url = string.Format(url, param);
        }
        else
        {
            url = api.action;
        }
        return base_url + url;
    }
    #endregion

    #region Main API
    public void GetUserProfile(Action<Result<UserProfileRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<UserProfileRes>(API.User, OnSucces: OnSuccess, OnError: OnError);
    }

    public void GetUserProfileForGame(Action<Result<CurrentGameAssetRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<CurrentGameAssetRes>(API.GameInfo, param: game_id, OnSucces: OnSuccess, OnError: OnError);
    }

    public void GetGameResult(Action<Result<UpdateResultRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        
        RequestAPI<UpdateResultRes>(API.GameResult, param: game_id, OnSucces: OnSuccess, OnError: OnError);
    }

    public void UpdateLevel(int coin, Action<Result<UpdateResultRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        var profile = GameManager.Instance.GetProfile();
        var sessionId = string.IsNullOrEmpty(profile.sessionId) ? "session1" : profile.sessionId;
        UpdateResultData data = new UpdateResultData
        {
            gameId = game_id,
            gameSessionId = sessionId,
            level = profile.level,
            map = profile.map,
            playtime = GameManager.Instance.GetPlayedTime(),
            score = coin,
            star = coin,
        };
        RequestAPI<UpdateResultRes>(API.UpdateGameResult, json: data, OnSucces: OnSuccess, OnError: OnError);
    }

    public void GetUserAsset(Action<Result<List<UserAsset>>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<List<UserAsset>>(API.GetUserAsset, OnSucces: OnSuccess, OnError: OnError);
    }

    public void UnlockCharacter(AssetData jsonData, Action<Result<AssetRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<AssetRes>(API.UnlockUserAsset, json: jsonData, OnSucces: OnSuccess, OnError: OnError);
    }

    public void GetGameDetail(Action<Result<GameDetailRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<GameDetailRes>(API.GetGameDetail, param:game_id, OnSucces: OnSuccess, OnError: OnError);
    }

    public void Rent(RentAssetData jsonData, Action<Result<AssetRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<AssetRes>(API.Rent, json: jsonData, OnSucces: OnSuccess, OnError: OnError);
    }

    public void UpdateCurrentAsset(CurrentAssetData jsonData, Action<Result<CurrentGameAssetRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        RequestAPI<CurrentGameAssetRes>(API.UpdateCurrentAsset, json: jsonData, OnSucces: OnSuccess, OnError: OnError);
    }

    public void Play(Action<Result<CurrentGameAssetRes>> OnSuccess, Action<ResponseError> OnError = null)
    {
        var profile = GameManager.Instance.GetProfile();
        if (profile == null)
        {
            OnError(new ResponseError { errorCode = "6969", message = "Cannot get your user data" });
        }
        else
        {
            PlayData jsonData = new PlayData() { assetIds = profile.currentAssetIds, gameId = game_id };
            RequestAPI<CurrentGameAssetRes>(API.Play, json: jsonData, OnSucces: OnSuccess, OnError: OnError);
        }
    }
    #endregion

#if UNITY_EDITOR
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        UpdateLevel(3, (res) =>
    //        {
    //            Debug.LogError(res.data.id);
    //        },
    //        (error) =>
    //        {
    //            Debug.LogError(error.message);
    //        });
    //    }
    //}
#endif
}

public class SelfSignedCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
