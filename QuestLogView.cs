using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ConfigurableQuestsLimit
{
    // only when the original journal slots can no longer hold all quests
    internal sealed class QuestLogView : MonoBehaviour
    {
        private RectTransform viewport;
        private RectTransform content;
        private ScrollRect scroll;
        private int originalSlotCount;
        private float rowHeight;
        private float stride;
        private float rowWidth;
        private bool failed;
        private bool layoutWarningLogged;

        internal void Resize(ref QuestMenuCellSlot[] slots, int questCount, Button shareButton, Button abandonButton)
        {
            if (failed)
                return;
            try
            {
                if (!content && !Initialize(slots, shareButton, abandonButton))
                    return;
                int required = Math.Max(originalSlotCount, questCount);
                if (slots.Length == required)
                    return;

                var resized = new QuestMenuCellSlot[required];
                Array.Copy(slots, resized, Math.Min(slots.Length, required));
                for (int i = slots.Length; i < required; i++)
                {
                    var clone = Object.Instantiate(slots[0].gameObject, content, false);
                    clone.name = $"QuestSlot_{i}";
                    var slot = clone.GetComponent<QuestMenuCellSlot>();
                    slot._index = i;
                    slot._scriptQuest = null;
                    slot._questProgressStruct = new QuestProgressStruct();
                    slot._isHighlighted = false;
                    PositionRow((RectTransform)clone.transform, i);
                    resized[i] = slot;
                    clone.SetActive(true);
                }
                for (int i = required; i < slots.Length; i++)
                {
                    slots[i].gameObject.SetActive(false);
                    Object.Destroy(slots[i].gameObject);
                }
                slots = resized;
                content.sizeDelta = new Vector2(rowWidth,
                    QuestLogLayout.ContentHeight(required, rowHeight, stride, viewport.rect.height));
                float maxOffset = Mathf.Max(0, content.rect.height - viewport.rect.height);
                content.anchoredPosition = new Vector2(0, Mathf.Clamp(content.anchoredPosition.y, 0, maxOffset));
            }
            catch (Exception error)
            {
                failed = true;
                MelonLogger.Error($"Unable to expand quest journal: {error}");
            }
        }

        private bool Initialize(QuestMenuCellSlot[] slots, Button shareButton, Button abandonButton)
        {
            Canvas.ForceUpdateCanvases();
            var parent = slots[0].transform.parent as RectTransform;
            if (!parent)
                throw new InvalidOperationException("Quest slot parent is not a RectTransform.");
            Bounds first = RowBounds(parent, (RectTransform)slots[0].transform);
            Bounds area = first;
            for (int i = 1; i < slots.Length; i++)
                area.Encapsulate(RowBounds(parent, (RectTransform)slots[i].transform));
            rowHeight = first.size.y;
            rowWidth = first.size.x;
            stride = float.PositiveInfinity;
            for (int i = 1; i < slots.Length; i++)
            {
                float verticalDistance = Mathf.Abs(first.center.y - RowBounds(parent,
                    (RectTransform)slots[i].transform).center.y);
                if (verticalDistance > 0.01f)
                    stride = Mathf.Min(stride, verticalDistance);
            }
            if (float.IsInfinity(stride))
                stride = rowHeight;
            if (rowHeight <= 0 || rowWidth <= 0 || stride <= 0 ||
                float.IsNaN(rowHeight) || float.IsInfinity(rowHeight) ||
                float.IsNaN(rowWidth) || float.IsInfinity(rowWidth) ||
                float.IsNaN(stride) || float.IsInfinity(stride))
            {
                if (!layoutWarningLogged)
                {
                    layoutWarningLogged = true;
                    var firstRect = (RectTransform)slots[0].transform;
                    MelonLogger.Warning($"Quest journal layout is not ready, will retry. Slots: {slots.Length}, " +
                        $"parent rect: {parent.rect.size}, first slot rect: {firstRect.rect.size}, " +
                        $"measured row: {rowWidth}x{rowHeight}, stride: {stride}.");
                }
                return false;
            }

            float viewportHeight = ArrangeActionPanel(parent, slots, area, shareButton, abandonButton);
            originalSlotCount = slots.Length;
            var viewportObject = new GameObject("UnlimitedQuestViewport", typeof(RectTransform),
                typeof(Image), typeof(RectMask2D), typeof(ScrollRect), typeof(LayoutElement));
            viewport = (RectTransform)viewportObject.transform;
            viewport.SetParent(parent, false);
            viewport.SetSiblingIndex(slots[0].transform.GetSiblingIndex());
            viewport.anchorMin = viewport.anchorMax = viewport.pivot = new Vector2(0, 1);
            viewport.sizeDelta = new Vector2(rowWidth, viewportHeight);
            viewport.anchoredPosition = new Vector2(first.min.x - parent.rect.xMin, area.max.y - parent.rect.yMax);
            viewportObject.GetComponent<Image>().color = Color.clear;
            viewportObject.GetComponent<LayoutElement>().ignoreLayout = true;

            content = (RectTransform)new GameObject("UnlimitedQuestContent", typeof(RectTransform)).transform;
            content.SetParent(viewport, false);
            content.anchorMin = content.anchorMax = content.pivot = new Vector2(0, 1);
            content.sizeDelta = viewport.sizeDelta;
            scroll = viewportObject.GetComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = stride;
            scroll.inertia = false;
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].transform.SetParent(content, false);
                slots[i]._index = i;
                PositionRow((RectTransform)slots[i].transform, i);
            }
            return true;
        }

        private void PositionRow(RectTransform row, int index)
        {
            row.anchorMin = row.anchorMax = row.pivot = new Vector2(0, 1);
            row.localRotation = Quaternion.identity;
            row.localScale = Vector3.one;
            row.sizeDelta = new Vector2(rowWidth, rowHeight);
            row.anchoredPosition = new Vector2(0, -index * stride);
        }

        private float ArrangeActionPanel(RectTransform parent, QuestMenuCellSlot[] slots, Bounds area,
            Button shareButton, Button abandonButton)
        {
            if (!shareButton || !abandonButton)
                return area.size.y;

            // Move the shared panel with frame and gamepad hints
            Transform panel = shareButton.transform;
            while (panel && !abandonButton.transform.IsChildOf(panel))
                panel = panel.parent;
            if (!panel)
                return area.size.y;

            bool containsSlots = false;
            foreach (var slot in slots)
                containsSlots |= slot.transform.IsChildOf(panel);

            var panelRoots = new System.Collections.Generic.List<Transform>();
            if (!containsSlots)
                panelRoots.Add(panel);
            else
            {
                // some prefabs place both buttons directly alongside the quest list so move 'em
                foreach (var button in new[] { shareButton, abandonButton })
                {
                    Transform branch = button.transform;
                    while (branch.parent && branch.parent != panel)
                        branch = branch.parent;
                    if (!panelRoots.Contains(branch))
                        panelRoots.Add(branch);
                }
            }

            Bounds panelBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, panelRoots[0]);
            for (int i = 1; i < panelRoots.Count; i++)
                panelBounds.Encapsulate(RectTransformUtility.CalculateRelativeRectTransformBounds(parent, panelRoots[i]));

            float gap = Mathf.Max(4f, stride - rowHeight);
            float availableHeight = area.size.y - panelBounds.size.y - gap;
            float panelTop = availableHeight >= rowHeight // keep at least one complete row
                ? area.min.y + panelBounds.size.y
                : area.min.y - gap;
            Vector3 displacement = parent.TransformVector(new Vector3(0, panelTop - panelBounds.max.y, 0));
            foreach (var root in panelRoots)
            {
                if (root.parent && root.parent.GetComponent<LayoutGroup>())
                {
                    var element = root.GetComponent<LayoutElement>() ?? root.gameObject.AddComponent<LayoutElement>();
                    element.ignoreLayout = true;
                }
                root.position += displacement;
            }
            return availableHeight >= rowHeight ? availableHeight : area.size.y;
        }

        private static Bounds RowBounds(RectTransform parent, RectTransform row)
        {
            // fuck Decorative/text
            var corners = new Vector3[4];
            row.GetWorldCorners(corners);
            var bounds = new Bounds(parent.InverseTransformPoint(corners[0]), Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
                bounds.Encapsulate(parent.InverseTransformPoint(corners[i]));
            return bounds;
        }

        internal void ShowSelection(int index)
        {
            if (!content || failed)
                return;
            scroll.StopMovement();
            float offset = QuestLogLayout.SelectionOffset(index, rowHeight, stride,
                viewport.rect.height, content.rect.height, content.anchoredPosition.y);
            content.anchoredPosition = new Vector2(0, offset);
        }
    }
}
