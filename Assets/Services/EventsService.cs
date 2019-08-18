using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class EventsService
{
	private static Event[] _events;
	public static Event[] events { get { return _events; } }

	private static EventRequest[] _eventsRequests;
	public static EventRequest[] eventsRequests { get { return _eventsRequests; } }

	private static string PENDING_STATUS = "Pendente",
						  PENDING_MESSAGE = "Pedido de participação pendente";

	public static WWW JoinEvent (int userId, Event evt)
	{
		WWWForm joinForm = new WWWForm ();
		joinForm.AddField ("_user", userId);
		joinForm.AddField ("_appointment", evt._id);
		joinForm.AddField ("status", PENDING_STATUS);
		joinForm.AddField ("message", PENDING_MESSAGE);

		WebService.route = ENV.EVENTS_REQUESTS_ROUTE;

		return WebService.Post(joinForm);
	}

	public static WWW GetEvents ()
	{
		WebService.route = ENV.EVENTS_ROUTE;
		WebService.action = "";

		return WebService.Get();
	}

	public static WWW GetUserEvents (int userId)
	{
		WebService.route = ENV.EVENTS_REQUESTS_ROUTE;
		WebService.action = ENV.QUERY_ACTION +
							"_user=" + userId.ToString();

		return WebService.Get();
	}

	public static void UpdateLocalEvents (string json)
	{
		_events = UtilsService.GetJsonArray<Event>(json);
	}

	public static void UpdateUserEvents (string json)
	{
		_eventsRequests = UtilsService.GetJsonArray<EventRequest>(json);
	}
}
