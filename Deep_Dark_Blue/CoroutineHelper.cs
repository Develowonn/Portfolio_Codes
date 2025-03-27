using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{
	private static Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();

	public static WaitForSeconds WaitForSeconds(float value)
	{
		if (!waitForSeconds.TryGetValue(value, out var wait))
		{
			waitForSeconds.Add(value, wait = new WaitForSeconds(value));
		}

		return wait;
	}
}