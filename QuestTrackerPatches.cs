using HarmonyLib;
using UnityEngine;

namespace ConfigurableQuestsLimit
{
    [HarmonyPatch(typeof(QuestTrackerManager), "Handle_TrackerVisibility")]
    internal static class QuestTrackerVisibility
    {
        private static void Postfix(QuestTrackerManager __instance, Transform ____questTrackContainer,
            CanvasGroup ____canvasGroup)
        {
            // game waits with _refreshingElements=true after the last quest is removed
            // clean up existing wrappers
            if (__instance._refreshingElements && __instance._questTrackElements.Count > 0)
                return;
            var pager = __instance.GetComponent<QuestTrackerPager>();
            if (!pager && __instance._questTrackElements.Count == 0)
                return;
            if (!pager)
                pager = __instance.gameObject.AddComponent<QuestTrackerPager>();
            pager.Apply(__instance, ____questTrackContainer as RectTransform, ____canvasGroup);
        }
    }
}
