using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsText : MonoBehaviour
{
	public Text text;

	private void Start ()
	{
		this.gameObject.GetComponent<Text>().text = ENV.POINT;
	}
}
