using UnityEngine;
using UnityEngine.UI;
using System;

public static class AlertsService
{
	public static void makeToast(string toast)
	{
		makeAlert("Aviso", toast, "OK");
	}

	public static void makeAlert(string title, string message, string button)
	{
        GameObject alertPrefab = (GameObject) Resources.Load("Prefabs/Alert"),
                   alertInstance = (GameObject) GameObject.Instantiate(alertPrefab, Vector3.zero, Quaternion.identity);
        
        alertInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        alertInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);

        Button actionButton = GameObject.Find("Button").GetComponent<Button>();
        Text titleText = GameObject.Find("Alert Tittle Text").GetComponent<Text>(),
             messageText = GameObject.Find("Alert Text").GetComponent<Text>(),
             buttonText = GameObject.Find("Alert Button Text").GetComponent<Text>();

        titleText.text = title.ToUpper();
        messageText.text = message;

        if (button.Length > 1)
            actionButton.interactable = true;
        else
            actionButton.interactable = false;

        buttonText.text = button.ToUpper();
	}

    public static void makeLoadingAlert(string loadingText)
    {
        GameObject loadingPrefab = (GameObject) Resources.Load("Prefabs/Loading");
        GameObject loadingInstance = (GameObject) GameObject.Instantiate(loadingPrefab, Vector3.zero, Quaternion.identity);
        
        loadingInstance.transform.SetParent (GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        loadingInstance.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);

        Text loadingTextObj = GameObject.Find("Loading Text").GetComponent<Text>();
        loadingTextObj.text = loadingText;
    }

    public static void removeLoadingAlert()
    {
        GameObject[] loadingAlerts = GameObject.FindGameObjectsWithTag("LoadingAlert");

        foreach (GameObject loadingObj in loadingAlerts)
            GameObject.Destroy(loadingObj);
    }
}