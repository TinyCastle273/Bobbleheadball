using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchWidth : MonoBehaviour
{
    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 10;

    Camera _camera;
    void Start()
    {
        _camera = GetComponent<Camera>();
        CalculateRatio();
    }

    //// Adjust the camera's height so the desired scene width fits in view
    //// even if the screen/window size changes dynamically.
    //void Update()
    //{
        
    //}

    private void CalculateRatio()
    {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        _camera.orthographicSize = desiredHalfHeight;
    }
}
