using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class APIDefine
{
    public string action;
    public string prefix;
    public APIMethod method;
    public string contentType;
}
public enum APIMethod
{
    GET,
    GET_QUERY,
    POST,
    PUT,
    DELETE,
}

public class API : APIDefine
{
    public static API User = new API
    {
        action = "user/profile",
        method = APIMethod.GET,
    };

    public static API GameInfo = new API
    {
        action = "{0}",
        prefix = "game",
        method = APIMethod.GET,
    };

    public static API GameResult = new API
    {
        action = "{0}",
        prefix = "game/result",
        method = APIMethod.GET,
    };

    public static API UpdateGameResult = new API
    {
        action = "game/result",
        method = APIMethod.POST,
    };

    public static API GetUserAsset = new API
    {
        action = "user/asset",
        method = APIMethod.GET
    };

    public static API UnlockUserAsset = new API
    {
        action = "user/asset/unlock",
        method = APIMethod.POST,
    };

    public static API GetGameDetail = new API
    {
        action = "gameId={0}",
        prefix = "game/details",
        method = APIMethod.GET_QUERY,
    };

    public static API Rent = new API
    {
        action = "user/asset/rent",
        method = APIMethod.POST,
    };

    public static API UpdateCurrentAsset = new API
    {
        action = "game/current-asset",
        method = APIMethod.PUT,
    };

    public static API Play = new API
    {
        action = "game/play",
        method = APIMethod.POST,
    };
}

