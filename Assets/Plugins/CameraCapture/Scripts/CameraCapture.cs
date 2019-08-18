using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CameraCapture : MonoBehaviour {

	#if !UNITY_EDITOR && UNITY_ANDROID
	private static AndroidJavaClass mPVC = null;
	private static AndroidJavaClass PVC
	{
		get
		{
			if( mPVC == null )
			mPVC = new AndroidJavaClass( "com.Wili.CameraCapture.CameraCapture" );
			return mPVC;
		}
	}
	#endif


	#if !UNITY_EDITOR && UNITY_IOS

	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void iCaptureVideo();


	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void iTakePhoto();

	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void iPlayVideo();

	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void iPickVideo();

	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void iPickPhoto();


	#endif

	public delegate void MediaDelegate(string path);
	public delegate void ErrorDelegate(string message);

	public event MediaDelegate PickCompleted;

	public  event MediaDelegate TakePhotoCompleted;

	public  event MediaDelegate CaptureVideoCompleted;

	public event ErrorDelegate Failed;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void captureVideo()
	{
		CaptureVideo();
	}

	private void CaptureVideo()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(PVC != null)
		{
		PVC.CallStatic("CaptureVideo");
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		iCaptureVideo();
		#endif
	}

	public void takePhoto()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(PVC != null)
		{
		PVC.CallStatic("TakePhoto");
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		iTakePhoto();
		#endif
	}


	public void playVideo()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(PVC != null)
		{
			PVC.CallStatic("PlayVideo");
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		iPlayVideo();
		#endif
	}

	public void pickPhoto()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(PVC != null)
		{
			PVC.CallStatic("PickPhoto");
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		iPickPhoto();
		#endif
	}

	public void pickVideo()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		if(PVC != null)
		{
			PVC.CallStatic("PickVideo");
		}
		#elif !UNITY_EDITOR && UNITY_IOS
		iPickVideo();
		#endif
	}

	private void OnTakePhotoComplete(string path)
	{
		var handler = TakePhotoCompleted;
		if (handler != null)
		{
			handler(path);
		}
	}

	private void OnCaptureVideoComplete(string path)
	{
		var handler = CaptureVideoCompleted;
		if (handler != null)
		{
			handler(path);
		}
	}

	private void OnPickComplete(string path)
	{
		var handler = PickCompleted;
		if (handler != null)
		{
			handler(path);
		}
	}

	private void OnFailure(string message)
	{
		var handler = Failed;
		if (handler != null)
		{
			handler(message);
		}
	}


}
