using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ConfigurableQuestsLimit
{
    [BepInPlugin("somepoi.ConfigurableQuestsLimit", "ConfigurableQuestsLimit", ModVersion.Value)]
    [BepInProcess("ATLYSS.exe")]
    public class Core : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get; private set; }
        private ConfigEntry<int> maxActiveQuests;
        private ConfigEntry<bool> unlimitedQuests;

        private void Awake()
        {
            Log = Logger;
            maxActiveQuests = Config.Bind("ConfigurableQuestsLimit", "MaxActiveQuests", ConfigurableQuestsLimitSettings.DefaultMaxActiveQuests,
                "Maximum simultaneous quests when UnlimitedQuests is false. Must be at least 1.");
            unlimitedQuests = Config.Bind("ConfigurableQuestsLimit", "UnlimitedQuests", false,
                "Allow unlimited simultaneous quests, ignoring MaxActiveQuests.");
            ApplySettings();
            Config.Save();
            new Harmony(Info.Metadata.GUID).PatchAll();
            Logger.LogInfo($"Active quest limit: {(ConfigurableQuestsLimitSettings.UnlimitedQuests ? "unlimited" : ConfigurableQuestsLimitSettings.MaxActiveQuests.ToString())}.");
        }

        private void ApplySettings()
        {
            if (maxActiveQuests == null || unlimitedQuests == null)
                return;
            if (maxActiveQuests.Value < 1)
            {
                Logger.LogWarning($"MaxActiveQuests must be positive, using {ConfigurableQuestsLimitSettings.DefaultMaxActiveQuests}.");
                maxActiveQuests.Value = ConfigurableQuestsLimitSettings.DefaultMaxActiveQuests;
            }
            ConfigurableQuestsLimitSettings.Configure(maxActiveQuests.Value, unlimitedQuests.Value);
        }
    }
}
