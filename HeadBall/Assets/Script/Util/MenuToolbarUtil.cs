using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class MenuToolbarUtil
{
    [MenuItem("Tools/Open ScenesManager")]
    public static void OpenScenesManager()
    {
        Debug.Log("===>>> Open scene: ScenesManager");
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/PersistentScene.unity");
    }
}
#endif