using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public static class TimelineService
{
	private static Post[] _posts;
	public static Post[] posts
	{
        get { return _posts; }
    }

    public static WWW RemovePost (Post post)
    {
    	WWWForm removeForm = new WWWForm ();

		WebService.route = ENV.POSTS_ROUTE;
		WebService.action = ENV.REMOVE_ACTION;
		WebService.id = post._id.ToString();

		return WebService.Post(removeForm);
    }

    public static WWW UpdatePostPoints (Post post, int newPoints)
    {
    	WWWForm postForm = new WWWForm ();
		postForm.AddField ("_id", post._id);
		postForm.AddField ("points", post.points + newPoints);

		WebService.route = ENV.POSTS_ROUTE;
		WebService.action = ENV.UPDATE_ACTION;
		WebService.id = post._id.ToString();

		return WebService.Post(postForm);
    }

    public static WWW NewPost (int userID, string imageBase64, string message)
	{
		WWWForm postForm = new WWWForm ();
		postForm.AddField ("_user", userID);
		postForm.AddField ("text_msg", message);
		postForm.AddField ("picture", imageBase64);

		WebService.route = ENV.POSTS_ROUTE;

		return WebService.Post(postForm);
	}

	public static WWW GetTimelinePosts ()
	{
		WebService.route = ENV.POSTS_ROUTE;

		return WebService.Get();
	}

	public static WWW GetTimelinePost (int postId)
	{
		WebService.route = ENV.POSTS_ROUTE;
		WebService.id = postId.ToString();

		return WebService.Get();
	}

	public static void UpdateLocalPosts (Post[] posts)
	{
		_posts = posts;
	}

	public static void UpdateLocalPosts (string json)
	{
		_posts = UtilsService.GetJsonArray<Post>(json);
	}

}
