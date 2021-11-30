using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class VersionChecker : MonoBehaviour {
    private const string LATEST_URL = "https://api.github.com/repos/Parseri/Kellopeli/releases/latest";
    void Start() {
        Debug.Log("startUp scene");
        StartCoroutine(CheckVersion());
    }


    private IEnumerator CheckVersion() {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(LATEST_URL)) {
            yield return webRequest.SendWebRequest();
            Debug.Log("result: " + webRequest.result.ToString());
            if (webRequest.result == UnityWebRequest.Result.Success) {
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                var jsonObject = JObject.Parse(webRequest.downloadHandler.text);
                var tag = jsonObject["tag_name"];
                Debug.Log("tag: " + tag);
                string currentVersion = Application.version;
                Debug.Log("current: " + currentVersion);
                if (tag.ToString().StartsWith("v")) {
                    SceneManager.LoadScene(1);
                    yield break;
                }
            } else
                SceneManager.LoadScene(1);
        }
    }

}
