using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class VersionChecker : MonoBehaviour {
    /*private const string LATEST_URL = "https://api.github.com/repos/Parseri/Kellopeli/releases/latest";
    private AppUpdateManager appUpdateManager;

    void Start() {
        Debug.Log("startUp scene");
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdate());
    }


    private IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }
        SceneManager.LoadScene(1);
    }*/
}
