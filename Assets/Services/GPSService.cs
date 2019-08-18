using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using SingleShadePlugin;

public static class GPSService
{
	private static double[] _location = new double[2];
	public static double[] location { get { return _location; } }

	public static void StartGPS()
	{
		InputLocation.Start();
	}

	public static void StopGPS()
	{
		if (IsActive())
			InputLocation.Stop();
	}

	public static bool IsActive()
	{
		return (InputLocation.isEnabledByUser);
	}

	public static bool ReceivePlayerLocation()
	{
		_location = new double[2];

		_location[0] = System.Convert.ToDouble(InputLocation.lastData.latitude);
		_location[1] = System.Convert.ToDouble(InputLocation.lastData.longitude);

		return true;
	}

}