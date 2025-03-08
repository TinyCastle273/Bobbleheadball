using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] float waitTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        AdManager.Instance.ShowAd();
        StartCoroutine(WaitForChangeScene());
    }

    IEnumerator WaitForChangeScene()
    {
        
        yield return new WaitForSeconds(waitTime);
        ScenesController.Instance.LoadLoadingScene();
    }
}
