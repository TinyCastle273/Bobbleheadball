using System;
using UnityEngine.Networking;
using UnityEngine;

namespace GamePix.Editor
{
    public static class Updater
    {
        private static readonly string gpxBackend = "https://rt.gamepix.com/unity/plugin/get?unity={0}&plugin={1}";
        public static readonly string defaultUrl = "https://my.gamepix.com/sdk/doc/unity-plugin";
        
        private static readonly string title = "GamePix Update";
        private static readonly string errorMessage = "GamePix update service is unavailable";
        private static readonly string errorButton = "Ok";
        private static readonly string newVersionMessage = "A new version of GamePix Unity3D plugin is available";
        private static readonly string newVersionComment = "*Please remove old version of GamePix Unity3D plugin before update";
        private static readonly string newVersionButton = "Download";
        
        [Serializable]
        private class PluginUpdateResponse
        {
            public string url;
            public bool lastVersion;
        }
        
        public static void TryUpdatePlugin(Action onUseLatest)
        {
            var url = string.Format(gpxBackend, ApplicationInfo.unityVersion, ApplicationInfo.pluginVersion);
            var request = UnityWebRequest.Get(url);
            var requestOperation = request.SendWebRequest();

            requestOperation.completed += (operation) =>
            {
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isHttpError || request.isNetworkError || !string.IsNullOrEmpty(request.error))
#endif
                {
                    Debug.LogError(errorMessage + ": " + request.error);
                    AlertWindow.Show(new AlertParameters()
                    {
                        Title = title,
                        Message = errorMessage,
                        Comment = request.error,
                        Button = errorButton,
                    });
                    return;
                }

                var responseText = request.downloadHandler.text;
                try
                {
                    var response = JsonUtility.FromJson<PluginUpdateResponse>(responseText);
                    if (response != null)
                    {
                        if (response.lastVersion)
                        {
                            onUseLatest();
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(response.url))
                        {
                            Debug.LogWarning(newVersionMessage + ": " + response.url);
                            AlertWindow.Show(new AlertParameters()
                            {
                                Title = title,
                                Message = newVersionMessage,
                                Comment = newVersionComment,
                                Button = newVersionButton,
                                Action = () =>
                                {
                                    Application.OpenURL(response.url);
                                }
                            });
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Can't parse GamePix update service response: " + responseText + " Details: " + ex);
                }
            };
        }
    }
}