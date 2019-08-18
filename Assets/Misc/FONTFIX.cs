using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FONTFIX : MonoBehaviour 
{
	public Font[] fonts;

	// Use this for initialization
	public void Start () 
	{
		foreach (Font font in fonts)
			font.material.mainTexture.filterMode = FilterMode.Point;
	}
}
