using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoScene : MonoBehaviour {

	public CameraCapture CamCap;

	public Text pathText;

	public RawImage pickPreiveimage;

	private int mode = 1;

	private void Start()
	{
		if (CamCap == null) {
			CamCap = GameObject.FindObjectOfType<CameraCapture> ();
		}

		this.CamCap.CaptureVideoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.TakePhotoCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.PickCompleted += new CameraCapture.MediaDelegate(this.Completetd);
		this.CamCap.Failed += new CameraCapture.ErrorDelegate(this.ErrorInfo);
	}

	private void Update()
	{
		
	}

	public void captureVideo()
	{
		this.mode = 1;
		this.CamCap.captureVideo();
	}

	public void pickVideo()
	{
		this.mode = 2;
		this.CamCap.pickVideo();
	}

	public void takePhoto()
	{
		this.mode = 3;
		this.CamCap.takePhoto();
	}

	public void pickPhoto()
	{
		this.mode = 4;
		this.CamCap.pickPhoto();
	}

	public void playVideo()
	{
		this.mode = 5;
		this.CamCap.playVideo();
	}


	private void Completetd(string patha)
	{
		pathText.text = pathText.text + "\n" + patha;
		if (this.mode == 4|| this.mode ==3)
		{
			base.StartCoroutine(this.LoadImage(patha));
		}
	}

	private void ErrorInfo(string errorInfo)
	{
		pathText.text = pathText.text + "\n<color=#ff0000>" + errorInfo +"</color>";
	}


	IEnumerator LoadImage(string path)
	{

		var url = "file://" + path;
		#if UNITY_EDITOR || UNITY_STANDLONE
		url = "file:/"+path;
		#endif
		Debug.Log ("current path is " + url);
		var www = new WWW(url);
		yield return www;

		var texture = www.texture;
		if (texture == null)
		{
			Debug.LogError("Failed to load texture url:" + url);
		}

		pickPreiveimage.texture = texture;
		texture = null;
	}
}
