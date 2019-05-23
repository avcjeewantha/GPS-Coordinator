using UnityEngine;
using System.Collections;

public enum LocationState
{
    Disabled,
    TimedOut,
    Failed,
    Enabled
}

public class GPSManager : MonoBehaviour
{
    public static int SCREEN_DENSITY;
    private LocationState state;  
    private float latitude;         // Position on earth (in degrees)
    private float longitude;

    IEnumerator Start()
    {
        if (Screen.dpi > 0f)
        {
            SCREEN_DENSITY = (int)(Screen.dpi / 160f);
        }
        else
        {
            SCREEN_DENSITY = (int)(Screen.currentResolution.height / 600);
        }

        state = LocationState.Disabled;
        latitude = 0f;
        longitude = 0f;

        if (Input.location.isEnabledByUser)
        {
            Input.location.Start();
            int waitTime = 15;
            while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
            {
                yield return new WaitForSeconds(1);
                waitTime--;
            }
            if (waitTime == 0)
            {
                state = LocationState.TimedOut;
            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                state = LocationState.Failed;
            }
            else
            {
                state = LocationState.Enabled;
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
            }
        }
    }

    IEnumerator OnApplicationPause(bool pauseState)
    {
        if (pauseState)
        {
            Input.location.Stop();
            state = LocationState.Disabled;
        }
        else
        {
            Input.location.Start();
            int waitTime = 15;
            while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
            {
                yield return new WaitForSeconds(1);
                waitTime--;
            }
            if (waitTime == 0)
            {
                state = LocationState.TimedOut;
            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                state = LocationState.Failed;
            }
            else
            {
                state = LocationState.Enabled;
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
            }
        }
    }

    void Update()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
    }

    void OnGUI()
    {
        Rect guiBoxRect = new Rect(40, 20, Screen.width - 80, Screen.height - 40);

        float buttonHeight = guiBoxRect.height / 7;

        switch (state)
        {
            case LocationState.Enabled:
                Rect coordinateTextRect = new Rect(guiBoxRect.x + 40, guiBoxRect.y + guiBoxRect.height / 2,
                                                 guiBoxRect.width - 80, buttonHeight * 2);
                GUI.skin.label.fontSize = 40 * SCREEN_DENSITY;
                GUI.skin.label.alignment = TextAnchor.UpperCenter;
                GUI.Label(coordinateTextRect, "latitude: " + latitude.ToString() + "\n" +
                    "longitude: " + longitude.ToString());
                break;

            case LocationState.Disabled:
                Rect disabledTextRect = new Rect(guiBoxRect.x + 40, guiBoxRect.y + guiBoxRect.height / 2,
                                                 guiBoxRect.width - 80, buttonHeight * 2);
                GUI.Label(disabledTextRect, "GPS is disabled. GPS must be enabled\n" +
                    "in order to use this application.");
                break;

            case LocationState.Failed:
                Rect failedTextRect = new Rect(guiBoxRect.x + 40, guiBoxRect.y + guiBoxRect.height / 2,
                                               guiBoxRect.width - 80, buttonHeight * 2);
                GUI.Label(failedTextRect, "Failed to initialize location service.\n" +
                    "Try again later.");
                break;

            case LocationState.TimedOut:
                Rect timeOutTextRect = new Rect(guiBoxRect.x + 40, guiBoxRect.y + guiBoxRect.height / 2,
                                                 guiBoxRect.width - 80, buttonHeight * 2);
                GUI.Label(timeOutTextRect, "Connection timed out. Try again later.");
                break;
        }
    }
}