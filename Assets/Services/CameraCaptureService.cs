using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCaptureService : MonoBehaviour {

	public CameraCapture CamCap;

	public Text pathText;

	public RawImage pickPreiveimage;

	public string photoBase64,
				  videoBase64;

	private int mode = 1;

	private void Start()
	{
		if (CamCap == null) {
			CamCap = GameObject.FindObjectOfType<CameraCapture> ();
		}

		videoBase64 = null;
		photoBase64 = null;

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

	public void resetFields ()
	{
		resetFields("");
	}

	public void resetFields (string icon)
	{
		if (icon.Length <= 1)
			icon = null;
		else
			icon = "Icons/" + icon;

		Texture2D texture = Resources.Load(icon) as Texture2D;
		pickPreiveimage.texture = texture;
		photoBase64 = null;
		videoBase64 = null;
	}

	public void rotateImage ()
	{
		Texture2D texture = pickPreiveimage.texture as Texture2D;
 		texture = UtilsService.rotateImage(texture);

		pickPreiveimage.texture = texture;
 		photoBase64 = System.Convert.ToBase64String(texture.EncodeToJPG());
	}

	private void Completetd(string patha)
	{
		//pathText.text = pathText.text + "\n" + patha;
		if (this.mode == 4 || this.mode == 3)
		{
			StartCoroutine(LoadImage(patha));
		}
		else if (this.mode == 1 || this.mode == 2)
		{
			StartCoroutine(LoadVideo(patha));
		}
	}

	private void ErrorInfo(string errorInfo)
	{
		Debug.Log("\n<color=#ff0000>" + errorInfo +"</color>");
	}


	private IEnumerator LoadImage(string path)
	{
		var url = "file://" + path;
		#if UNITY_EDITOR || UNITY_STANDLONE
		url = "file:/"+path;
		#endif
		Debug.Log ("current photo path is " + url);
		var www = new WWW(url);
		yield return www;

		var texture = UtilsService.ResizeTexture(www.texture, "Average", 0.3f);
		texture.Apply();

		if (texture == null)
		{
			Debug.LogError("Failed to load texture url:" + url);
		}

		pickPreiveimage.texture = texture;
		UtilsService.SizeToParent(pickPreiveimage, 0f);

		photoBase64 = System.Convert.ToBase64String(texture.EncodeToJPG());
	}

	private IEnumerator LoadVideo(string path)
	{
		var url = "file://" + path;
		#if UNITY_EDITOR || UNITY_STANDLONE
		url = "file:/"+path;
		#endif
		Debug.Log ("current video path is " + url);
		var www = new WWW(url);
		yield return www;

		videoBase64 = System.Convert.ToBase64String(www.bytes);
	}
}
