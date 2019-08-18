using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimelineController : ScreenController 
{
	public GameObject postCard, NMPostCard, noPostsCard, moreCard;

	private int START_POST_INDEX,
				END_POST_INDEX;

	public void Start ()
	{
		TutorialService.CheckTutorial("Timeline");

		previousView = "Home";

		postCard.SetActive(false);
		NMPostCard.SetActive(false);
		noPostsCard.SetActive(false);
		moreCard.SetActive(false);

		StartCoroutine(_GetTimelinePosts());
	}

	public void ShowMorePosts ()
	{
		START_POST_INDEX = END_POST_INDEX;
		END_POST_INDEX = END_POST_INDEX + 5;

		if (END_POST_INDEX > TimelineService.posts.Length)
			END_POST_INDEX = TimelineService.posts.Length;

		CreatePostsCards();
	}

	private IEnumerator _GetTimelinePosts ()
	{
		AlertsService.makeLoadingAlert("Recebendo postagens");
		WWW postsRequest = TimelineService.GetTimelinePosts();

		while (!postsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + postsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + postsRequest.text);

		if (postsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			TimelineService.UpdateLocalPosts(postsRequest.text);

			START_POST_INDEX = 0;
			END_POST_INDEX = (TimelineService.posts.Length < 5) ? TimelineService.posts.Length : 5;

			CreatePostsCards();
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "");
			yield return new WaitForSeconds(3f);
			LoadView("Home");
		}

		yield return null;
	}

	private void CreatePostsCards ()
    {
    	Vector3 position = new Vector3(0f, 0f, 0f);

     	if (TimelineService.posts.Length > 0)
     	{
     		postCard.SetActive(true);
     		NMPostCard.SetActive(true);
     		noPostsCard.SetActive(false);
     	}
     	else
     	{
     		NMPostCard.SetActive(false);
     		postCard.SetActive(false);
     		noPostsCard.SetActive(true);
     	}

     	if (END_POST_INDEX >= TimelineService.posts.Length)
     		moreCard.SetActive(false);
     	else
     		moreCard.SetActive(true);

     	for (int i = START_POST_INDEX; i < END_POST_INDEX; i++)
        {
        	Post currentPost = TimelineService.posts[i];
        	GameObject currentCard;

        	if (currentPost.text_msg.Length >= 3)
        		currentCard = postCard;
        	else
        		currentCard = NMPostCard;

        	position = currentCard.transform.position;
            position = new Vector3(position.x, position.y, position.z);
            GameObject card = (GameObject) Instantiate(currentCard, position, Quaternion.identity);
        	card.transform.SetParent(GameObject.Find("List").transform, false);

        	PostCard postCardScript = card.GetComponent<PostCard>();
        	postCardScript.UpdatePost(TimelineService.posts[i]);
        }

        GameObject showMoreCard = (GameObject) Instantiate(moreCard, position, Quaternion.identity);
        showMoreCard.transform.SetParent(GameObject.Find("List").transform, false);

        Destroy(moreCard);
        moreCard = showMoreCard;

        postCard.SetActive(false);
        NMPostCard.SetActive(false);

        AlertsService.removeLoadingAlert();
    }
}
