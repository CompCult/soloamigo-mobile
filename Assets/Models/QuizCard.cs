using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizCard : MonoBehaviour
{
	#pragma warning disable 0108 0472

	public GameObject binaryAnswerIcon, multipleAnswerIcon, infinityChancesIcon, singleChancesIcon;
	public Text title, description, date, points;
	public Quiz quiz;

	public void UpdateQuizCard (Quiz quiz)
	{
		ResetIcons();

		this.quiz = quiz;

		title.text = quiz.title;
		date.text = "Até " + UtilsService.GetDate(quiz.end_time);
		points.text = quiz.points.ToString();

		if (quiz.description.Length > 60)
			description.text = quiz.description.Substring(0, 57) + "...";
		else
			description.text = quiz.description;

		if (quiz.correct_answer != null && quiz.correct_answer.Length > 0)
			binaryAnswerIcon.SetActive(true);
		else
			multipleAnswerIcon.SetActive(true);

		if (quiz.single_answer != null && quiz.single_answer)
			singleChancesIcon.SetActive(true);
		else
			infinityChancesIcon.SetActive(true);
	}

   	public void OpenQuiz ()
	{
		QuizzesService.UpdateQuiz(quiz);
		SceneManager.LoadScene("Quiz");
	}

	private void ResetIcons()
	{
		binaryAnswerIcon.SetActive(false);
		multipleAnswerIcon.SetActive(false);
		infinityChancesIcon.SetActive(false);
		singleChancesIcon.SetActive(false);
	}
}
