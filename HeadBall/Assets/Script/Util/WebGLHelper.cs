using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLHelper : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern void QuitWebGL();

    [DllImport("__Internal")]
    public static extern void registerVisibilityChangeEvent();
#else
    public static void QuitWebGL() { }

    public static void registerVisibilityChangeEvent() { }
#endif
}
