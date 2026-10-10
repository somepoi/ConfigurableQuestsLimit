using Steamworks;
using UnityEngine;

public class SteamAchievementManager : MonoBehaviour
{
	public static SteamAchievementManager current;

	public bool _disableManager;

	private void Start()
	{
		current = this;
		Invoke("Init_DownloadStats", 0.85f);
	}

	public void Init_DownloadStats()
	{
		if (SteamManager.Initialized && !_disableManager)
		{
			SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
		}
	}

	public void AddStatValue(string statTag)
	{
		if (SteamManager.Initialized && !_disableManager)
		{
			SteamUserStats.GetStat(statTag, out int pData);
			int nData = pData + 1;
			SteamUserStats.SetStat(statTag, nData);
			SteamUserStats.StoreStats();
		}
	}

	public void Init_EarnAchievement(string achievementTag)
	{
		if (SteamManager.Initialized && !_disableManager)
		{
			SteamUserStats.GetAchievement(achievementTag, out var pbAchieved);
			if (!pbAchieved)
			{
				SteamUserStats.SetAchievement(achievementTag);
				SteamUserStats.StoreStats();
			}
		}
	}

	public void ResetAllStats()
	{
		if (SteamManager.Initialized && !_disableManager)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
			SteamUserStats.StoreStats();
		}
	}
}
