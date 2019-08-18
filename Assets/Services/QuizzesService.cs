using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class QuizzesService
{
	private static Quiz[] _quizzes;
	public static Quiz[] quizzes { get { return _quizzes; } }

	private static Quiz _quiz;
	public static Quiz quiz { get { return _quiz; } }

	public static WWW SearchQuiz (int userId, string secretCode)
	{
		WebService.route = ENV.QUIZZES_ROUTE;
		WebService.action = ENV.SEARCH_PRIVATE +
							"user_id=" + userId +
							"&secret_code=" + secretCode;

		return WebService.Get();
	}

	public static WWW SendResponse (int _quiz, int _user, string answer)
	{
		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("_quiz", _quiz);
		requestForm.AddField ("_user", _user);
		requestForm.AddField ("answer", answer);

		WebService.route = ENV.QUIZ_ANSWERS_ROUTE;
		WebService.action = "";

		return WebService.Post(requestForm);
	}

	public static WWW GetQuizzes (int userId)
	{
		WebService.route = ENV.QUIZZES_ROUTE;
		WebService.action = ENV.SEARCH_PUBLIC +
							"user_id=" + userId;

		return WebService.Get();
	}

	public static void UpdateQuizzes (string json)
	{
		_quizzes = UtilsService.GetJsonArray<Quiz>(json);
		
		foreach (Quiz quiz in _quizzes)
			quiz.BuildAnswers();
	}

	public static void UpdateQuiz (string json)
	{
		_quiz = JsonUtility.FromJson<Quiz>(json);
		_quiz.BuildAnswers();
	}

	public static void UpdateQuiz (Quiz quiz)
	{
		_quiz = quiz;
		_quiz.BuildAnswers();
	}

}
