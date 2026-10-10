using HarmonyLib;

namespace ConfigurableQuestsLimit
{
    // Void prefixes preserve the original validation, rewards and save/network calls.
    [HarmonyPatch(typeof(PlayerQuesting), "Awake")]
    internal static class ConfigurableQuestsLimitOnAwake
    {
        private static void Prefix(ref int ____questLogLimit)
        {
            ____questLogLimit = ConfigurableQuestsLimitSettings.EffectiveLimit;
        }
    }

    [HarmonyPatch(typeof(PlayerQuesting), nameof(PlayerQuesting.Accept_Quest))]
    internal static class ConfigurableQuestsLimitOnAccept
    {
        private static void Prefix(ref int ____questLogLimit)
        {
            ____questLogLimit = ConfigurableQuestsLimitSettings.EffectiveLimit;
        }
    }

    [HarmonyPatch(typeof(QuestSelectionManager), nameof(QuestSelectionManager.OnClick_QuestAcceptButton))]
    internal static class ConfigurableQuestsLimitOnDialogAccept
    {
        private static void Prefix()
        {
            if (Player._mainPlayer && Player._mainPlayer._pQuest)
                Player._mainPlayer._pQuest._questLogLimit = ConfigurableQuestsLimitSettings.EffectiveLimit;
        }
    }
}
