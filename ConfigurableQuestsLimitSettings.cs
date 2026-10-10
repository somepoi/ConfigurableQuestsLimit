namespace ConfigurableQuestsLimit
{
    internal static class ConfigurableQuestsLimitSettings
    {
        internal const int DefaultMaxActiveQuests = 10;
        internal static int MaxActiveQuests { get; private set; } = DefaultMaxActiveQuests;
        internal static bool UnlimitedQuests { get; private set; }
        internal static int EffectiveLimit => UnlimitedQuests ? int.MaxValue : MaxActiveQuests;

        internal static void Configure(int maxActiveQuests, bool unlimitedQuests)
        {
            MaxActiveQuests = maxActiveQuests > 0 ? maxActiveQuests : DefaultMaxActiveQuests;
            UnlimitedQuests = unlimitedQuests;
        }

        internal static string FormatCounter(int count)
        {
            return UnlimitedQuests ? $"Quest Log ({count})" : $"Quest Log ({count} / {MaxActiveQuests})";
        }
    }
}
