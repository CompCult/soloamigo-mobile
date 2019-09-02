using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterController : ScreenController
{
	public GameObject[] buttons;
	public GameObject buttonsMenu;

	private Color greenColor = new Color(0.07450981f, 0.7568628f, 0.3333333f),
				  greyColor = new Color(0.4470589f, 0.4470589f, 0.4470589f);

	public void Start()
	{
		string view = currentTab;

		foreach (GameObject button in buttons)
			if (button.name == GetViewName())
				view = GetViewName();

		if (view != currentTab)
			currentTab = view;

		foreach (GameObject button in buttons)
			if (button.name == view)
				MarkButton(button, greenColor);
			else
				MarkButton(button, greyColor);
	}

	public void LoadViewAndMark(string viewName)
	{
		PlaySound("click_1");
		LoadView(viewName);
	}

	private void MarkButton(GameObject button, Color color)
	{
		Image icon = button.GetComponentsInChildren<Image>()[1];
		Text label = button.GetComponentsInChildren<Text>()[0];

		icon.color = color;
		label.color = color;
	}
}
