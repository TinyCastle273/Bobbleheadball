using UnityEditor;
using UnityEngine;

namespace GamePix.Editor
{
    public class BuildWindow
    {
        private static string buildDirectory = "html5";
        private static readonly string errorTitle = "GamePix Builder";
        private static readonly string errorMessage = "Can't build the game";
        private static readonly string errorComment = "Unity WebGL Platform is not installed";
        private static readonly string errorButtonTitle = "Ok";

        [MenuItem("GamePix/Build and Run", false, -1)]
        static void Build()
        {
            Build(true, true);
        }

        [MenuItem("GamePix/Safe builds/Build and Run (Default resources)", false, 1)]
        static void DefaultResourceBuild()
        {
            Build(true, false);
        }

        private static void Build(bool safeMode, bool changeResources)
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL)) {
                AlertWindow.Show(new AlertParameters()
                {
                    Title = errorTitle,
                    Message = errorMessage,
                    Comment = errorComment,
                    Button = errorButtonTitle,
                });
                return;
            }
            
            if (Emscripten.editorVersion != Emscripten.pluginVersion)
            {
                Debug.LogError("This Gamepix plugin version [emscripten: " + Emscripten.pluginVersion +
                               "] cannot be used for Unity " + ApplicationInfo.unityVersion +
                               " [emscripten: " + Emscripten.editorVersion +
                               "]. Check Gamepix plugin for Unity: " + Updater.defaultUrl);
                return;
            }
            Updater.TryUpdatePlugin(() =>
            {
                UnityConfigurator.SetWebGLSettings(safeMode, changeResources);
                Builder.BuildArchive(buildDirectory, changeResources);
            });
        }

        [MenuItem("GamePix/Check update", false, 2)]
        static void CheckUpdate()
        {
            Updater.TryUpdatePlugin(() =>
            {
                Debug.Log("The latest GamePix Unity3D plugin version is used");
            });
        }
    }
}