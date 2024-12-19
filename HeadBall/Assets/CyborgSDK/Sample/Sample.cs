using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        Transform tfLayout = transform.GetChild(0);
        tfLayout.GetChild(0)
            .GetComponent<Button>()
            .onClick.AddListener(OnInitPressed);

        tfLayout.GetChild(1)
            .GetComponent<Button>()
            .onClick.AddListener(OnStartPressed);

        tfLayout.GetChild(2)
            .GetComponent<Button>()
            .onClick.AddListener(OnSendTrackingPressed);

        tfLayout.GetChild(3)
            .GetComponent<Button>()
            .onClick.AddListener(OnStopPressed);
    }

    public void SDKInitCallback(string userProfile)
    {
        Debug.Log(userProfile);
        Transform tfLayout = transform.GetChild(0);
        tfLayout.GetChild(1)
            .GetComponent<Button>()
            .interactable = true;

        tfLayout.GetChild(2)
            .GetComponent<Button>()
            .interactable = true;

        tfLayout.GetChild(3)
            .GetComponent<Button>()
            .interactable = true;
        Cyborg.GetUserProfile();
        Cyborg.VerifyJWT();
    }

    public void OnUserProfileCallback(string userProfile)
    {
        Debug.Log("userProfile: " + userProfile);
    }

    public void OnVerifyJWTCallback(string userProfile)
    {
        Debug.Log("JWT: " + userProfile);
    }

    public void OnInitPressed()
    {
        Cyborg cyborg = GetComponent<Cyborg>();
        cyborg.onSDKInitCallback.AddListener(this.SDKInitCallback);
        cyborg.onUserProfileCallback.AddListener(this.OnUserProfileCallback);
        cyborg.onVerifyJWTCallback.AddListener(this.OnVerifyJWTCallback);
        Cyborg.InitSDK("Sample");
    }
    public void OnStartPressed()
    {
        Cyborg.StartGame();
    }
    public void OnSendTrackingPressed()
    {
        Cyborg.SendTracking("TEST_EVENT", "{ \"param1\":\"testareta\", \"param2\":1231, \"param3\": true}");
    }
    public void OnStopPressed()
    {
        Cyborg.StopGame();
    }
}
