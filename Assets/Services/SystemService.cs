using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public static class SystemService
{
	public static bool in_maintenance;

	public static WWW CheckMaintenance ()
	{
		WebService.route = ENV.GENERAL_ROUTE;
		WebService.action = ENV.MAINTENANCE_ACTION;

		return WebService.Get();
	}
}
