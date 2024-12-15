using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ResponseError
{
    public string errorCode;
    public string message;
}

[Serializable]
public class Result<T>
{
    public T data;
    public int status;
}

[Serializable]
public class UserProfileRes
{
    public int id;
    public string name;
    public string avatar;
    public int point;
    public int extraSpin;
}


[Serializable]
public class CurrentGameAssetRes
{
    public int id;
    public int userId;
    public string gameId;
    public int map;
    public int level;
    public int star;
    public int score;
    public string currentAssetId;
    public List<string> currentAssetIds;
    public int playTurn;
    public long lastPlayDate;
    public int turnPlayed;
    public int maxMap;
    public int maxLevel;
    public string sessionId;
}

[Serializable]
public class UpdateResultRes
{
    public int id;
    public int userId;
    public string gameSessionId;
    public string gameId;
    public float playtime;
    public int map;
    public int level;
    public int star;
    public int before;
    public int score;
    public int after;
    public string assetError;
    public List<RewardAsset> rewardAssets;
}


[Serializable]
public class MapResult
{
    public int map;
    List<StarResult> stars;
}

[Serializable]
public class StarResult
{
    public int level;
    public int star;
}

[Serializable]
public class RewardAsset
{
    public string id;
    public string name;
    public List<string> gameIds;
    public string type;
    public string image;
    public string path;
    public int maxCard;
    public int rentPoint;
    public int winRate;
    public string description;
    public string mainAssetId;
}

[Serializable]
public class UserAsset
{
    public int userAssetId;
    public string assetId;
    public string assetName;
    public string type;
    public string token;
    public List<string> gameIds;
    public long expiredAt;
    public int card;
    public bool status;
    public int maxCard;
    public string path;
    public int rentPoint;
    public bool rentStatus;
    public string image;
    public string description;
    public bool selling;
    public int orderId;
    public string mainAssetId;
    public List<string> games;
}

[Serializable]
public class AssetRes
{
    public int id;
    public string userId;
    public string assetId;
    public long expiredAt;
    public bool status;
    public string tokenId;
    public bool isFrozen;
}

[Serializable]
public class GameDetailRes
{
    public string gameId;
    public string name;
    public string logo;
    public string banner;
    public string description;
    public string genre;
    public string about;
    public string howToPlay;
    public string url;
    public int maxLevel;
    public int oneStarPoint;
    public int twoStarPoint;
    public int threeStarPoint;
}

[Serializable]
public class UpdateResultData
{
    public string gameId;
    public string gameSessionId;
    public float playtime;
    public int map;
    public int level;
    public int star;
    public int score;
}

[Serializable]
public class AssetData
{
    public string assetId;
}

[Serializable]
public class RentAssetData
{
    public string assetId;
    public string gameId;
}

[Serializable]
public class CurrentAssetData
{
    public string currentAssetIds;
    public string gameId;
}

[Serializable]
public class PlayData
{
    public string gameId;
    public List<string> assetIds;
}
