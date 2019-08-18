using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RAWIMAGERESIZER : MonoBehaviour 
{
	private RawImage currentImage;

	public void Start ()
	{
		currentImage = this.gameObject.GetComponent<RawImage>();
		UtilsService.SizeToParent(currentImage, 0f);
	}

}
