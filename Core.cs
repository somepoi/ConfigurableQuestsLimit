using MelonLoader;

using System.IO;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(ConfigurableQuestsLimit.Core), "ConfigurableQuestsLimit", ConfigurableQuestsLimit.ModVersion.Value, "somepoi", null)]
[assembly: MelonGame("KisSoft", "ATLYSS")]

namespace ConfigurableQuestsLimit
{
    public class Core : MelonMod
    {
        private MelonPreferences_Entry<int> maxActiveQuests;
        private MelonPreferences_Entry<bool> unlimitedQuests;

        public override void OnInitializeMelon()
        {
            string configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "ConfigurableQuestsLimit.cfg");
            var category = MelonPreferences.CreateCategory("ConfigurableQuestsLimit");
            maxActiveQuests = category.CreateEntry("MaxActiveQuests", ConfigurableQuestsLimitSettings.DefaultMaxActiveQuests,
                description: "Maximum simultaneous quests when UnlimitedQuests is false. Must be at least 1.");
            unlimitedQuests = category.CreateEntry("UnlimitedQuests", false,
                description: "Allow unlimited simultaneous quests, ignoring MaxActiveQuests.");
            category.SetFilePath(configPath);
            ApplySettings();
            category.SaveToFile();
            LoggerInstance.Msg($"Active quest limit: {(ConfigurableQuestsLimitSettings.UnlimitedQuests ? "unlimited" : ConfigurableQuestsLimitSettings.MaxActiveQuests.ToString())}.");
        }

        private void ApplySettings()
        {
            if (maxActiveQuests == null || unlimitedQuests == null)
                return;
            if (maxActiveQuests.Value < 1)
            {
                LoggerInstance.Warning($"MaxActiveQuests must be positive, using {ConfigurableQuestsLimitSettings.DefaultMaxActiveQuests}.");
                maxActiveQuests.Value = ConfigurableQuestsLimitSettings.DefaultMaxActiveQuests;
            }
            ConfigurableQuestsLimitSettings.Configure(maxActiveQuests.Value, unlimitedQuests.Value);
        }
    }
}
