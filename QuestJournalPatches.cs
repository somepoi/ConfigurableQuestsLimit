using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ConfigurableQuestsLimit
{
    [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Handle_CellUpdate))]
    internal static class QuestJournalUpdate
    {
        private static void Prefix(QuestMenuCell __instance, ref QuestMenuCellSlot[] ____questMenuCellSlots,
            ref int ____selectedQuestSlotIndex, ScriptableQuest ____selectedQuest,
            Button ____shareQuestButton, Button ____abandonQuestButton)
        {
            if (!Player._mainPlayer || !Player._mainPlayer._pQuest ||
                ____questMenuCellSlots == null || ____questMenuCellSlots.Length == 0)
                return;

            var view = __instance.GetComponent<QuestLogView>();
            int count = Player._mainPlayer._pQuest._scriptableQuests.Count;
            if (!view && count <= ____questMenuCellSlots.Length)
                return;
            if (!view)
                view = __instance.gameObject.AddComponent<QuestLogView>();

            view.Resize(ref ____questMenuCellSlots, count, ____shareQuestButton, ____abandonQuestButton);
            ____selectedQuestSlotIndex = Mathf.Clamp(____selectedQuestSlotIndex, 0, ____questMenuCellSlots.Length - 1);
            SynchronizeSelection(__instance, ____questMenuCellSlots, ____selectedQuestSlotIndex, ____selectedQuest);
        }

        private static void Postfix(QuestMenuCell __instance, Text ____questLogCounterText,
            QuestMenuCellSlot[] ____questMenuCellSlots, int ____selectedQuestSlotIndex, ScriptableQuest ____selectedQuest)
        {
            if (____questLogCounterText && Player._mainPlayer && Player._mainPlayer._pQuest)
                ____questLogCounterText.text = ConfigurableQuestsLimitSettings.FormatCounter(Player._mainPlayer._pQuest._questProgressData.Count);
            if (__instance.GetComponent<QuestLogView>())
                SynchronizeSelection(__instance, ____questMenuCellSlots, ____selectedQuestSlotIndex, ____selectedQuest);
        }

        private static void SynchronizeSelection(QuestMenuCell cell, QuestMenuCellSlot[] slots, int index, ScriptableQuest selected)
        {
            // NPC quest previews also use Apply_QuestInfo
            if (!Player._mainPlayer || !Player._mainPlayer._pQuest || !TabMenu._current ||
                !TabMenu._current._isOpen || TabMenu._current._selectedMenuCell != cell || TabMenu._current._questPanelMode ||
                slots == null || index < 0 || index >= slots.Length)
                return;

            var questing = Player._mainPlayer._pQuest;
            var quest = index < questing._scriptableQuests.Count ? questing._scriptableQuests[index] : null;
            if (selected == quest)
                return;

            // refresh before the original gamepad handler
            // cuz Share uses _selectedQuest and Abandon uses the index
            // so we'll refer them to the same current quest.
            slots[index]._scriptQuest = quest;
            slots[index]._questProgressStruct = index < questing._questProgressData.Count
                ? questing._questProgressData[index] : new QuestProgressStruct();
            cell.Select_QuestSlot(index);
        }
    }

    [HarmonyPatch(typeof(QuestMenuCell), nameof(QuestMenuCell.Select_QuestSlot))]
    internal static class QuestJournalSelection
    {
        private static void Postfix(QuestMenuCell __instance, int _index)
        {
            var view = __instance.GetComponent<QuestLogView>();
            if (view)
                view.ShowSelection(_index);
        }
    }
}
