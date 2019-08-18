using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivitiesController : ScreenController 
{
	public void Start ()
	{	
		TutorialService.CheckTutorial("Activities");
		TutorialService.CheckParentalAlert();

		previousView = "Home";
	}
}
