using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ConfigurableQuestsLimit
{
    internal sealed class QuestTrackerPager : MonoBehaviour
    {
        private readonly List<QuestTrackElement> eligible = new List<QuestTrackElement>();
        private readonly List<QuestTrackElement> nextEligible = new List<QuestTrackElement>();
        private readonly List<float> heights = new List<float>();
        private readonly List<float> titleHeights = new List<float>();
        private readonly Dictionary<QuestTrackElement, RectTransform> rowRoots = new Dictionary<QuestTrackElement, RectTransform>();
        private readonly Dictionary<QuestTrackElement, int> indices = new Dictionary<QuestTrackElement, int>();
        private readonly HashSet<QuestTrackElement> liveElements = new HashSet<QuestTrackElement>();
        private readonly List<QuestTrackElement> staleRoots = new List<QuestTrackElement>();
        private List<TrackerPage> pages = new List<TrackerPage>();
        private RectTransform container;
        private Text indicator;
        private float width;
        private float preferredWidth;
        private float spacing;
        private float textGap;
        private float budget;
        private float headerHeight;
        private int page;
        private int layoutHash;
        private QuestTrackElement anchorQuest;
        private bool initialized;
        private bool failed;

        internal void Apply(QuestTrackerManager manager, RectTransform trackContainer, CanvasGroup group)
        {
            if (failed || !trackContainer)
                return;
            if (!initialized && manager._questTrackElements.Count == 0)
                return;
            try
            {
                if (!initialized)
                    Initialize(trackContainer, manager._questTrackElements[0]);

                // convert the screen budget to the container's own coordinates.
                // lower half stays available for stuff
                var canvas = container.GetComponentInParent<Canvas>();
                Camera camera = canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
                Vector2 top = RectTransformUtility.WorldToScreenPoint(camera, container.position);
                Vector2 unit = RectTransformUtility.WorldToScreenPoint(camera, container.TransformPoint(Vector3.down));
                Vector2 rightUnit = RectTransformUtility.WorldToScreenPoint(camera, container.TransformPoint(Vector3.right));
                float pixelsPerUnit = Mathf.Max(0.01f, Mathf.Abs(top.y - unit.y));
                float horizontalScale = Mathf.Max(0.01f, Mathf.Abs(rightUnit.x - top.x));
                Rect safeArea = Screen.safeArea;
                float edgeInset = Mathf.Max(8f, indicator.fontSize * 0.5f * horizontalScale);
                var bounds = QuestTrackerPagination.FitHorizontalBounds(preferredWidth,
                    safeArea.xMin, safeArea.xMax, edgeInset, horizontalScale);
                width = bounds.Width;
                // prefab's container edge may extend beyond the screen
                // rows must use the actual HUD edge.
                Vector2 screenAnchor = new Vector2(bounds.ScreenRight, Mathf.Min(top.y, safeArea.yMax - edgeInset));
                if (container.parent is RectTransform parent &&
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, screenAnchor, camera, out Vector3 worldAnchor))
                    container.position = worldAnchor;
                else
                    throw new InvalidOperationException("Unable to anchor quest tracker inside the screen.");
                top = screenAnchor;
                budget = Mathf.Max(headerHeight * 2f,
                    Mathf.Min(Screen.height * 0.5f, top.y - Screen.height * 0.4f) / pixelsPerUnit);
                container.sizeDelta = new Vector2(width, budget);

                int hash = Screen.width ^ (Screen.height << 8) ^ budget.GetHashCode() ^ width.GetHashCode();
                nextEligible.Clear();
                liveElements.Clear();
                foreach (var element in manager._questTrackElements)
                {
                    if (element)
                        liveElements.Add(element);
                    if (!element || element._isHidden || !element._trackQuestNameText || !element._trackElementText)
                        continue;
                    nextEligible.Add(element);
                    unchecked
                    {
                        hash = hash * 31 + element.GetInstanceID();
                        hash = hash * 31 + (element._trackQuestNameText.text ?? "").GetHashCode();
                        hash = hash * 31 + (element._trackElementText.text ?? "").GetHashCode();
                    }
                }
                RemoveStaleRows();
                bool changed = layoutHash != hash || eligible.Count != nextEligible.Count;
                if (changed)
                {
                    layoutHash = hash;
                    RebuildPages();
                }

                bool inputAllowed = group && group.alpha > 0 && Player._mainPlayer &&
                    !Player._mainPlayer._inChat && !Player._mainPlayer._inUI &&
                    (!DialogManager._current || !DialogManager._current._isDialogEnabled);
                int direction = inputAllowed
                    ? (Input.GetKeyDown(KeyCode.PageDown) ? 1 : Input.GetKeyDown(KeyCode.PageUp) ? -1 : 0)
                    : 0;
                if (direction != 0)
                {
                    page = QuestTrackerPagination.MovePage(page, direction, pages.Count);
                    changed = true;
                }
                ApplyPage(manager._questTrackElements, changed);
            }
            catch (Exception error)
            {
                failed = true;
                MelonLogger.Error($"Unable to paginate quest tracker: {error}");
            }
        }

        private void Initialize(RectTransform trackContainer, QuestTrackElement sample)
        {
            if (!sample || !sample._trackElementText || !sample._trackQuestNameText)
                throw new InvalidOperationException("Quest tracker text template is missing.");
            Canvas.ForceUpdateCanvases();
            container = trackContainer;
            width = container.rect.width;
            if (width <= 0)
                width = ((RectTransform)sample.transform).rect.width;
            if (width <= 0)
                throw new InvalidOperationException("Quest tracker width is zero.");
            preferredWidth = width;
            var corners = new Vector3[4];
            container.GetWorldCorners(corners);
            Vector3 topRight = corners[2];
            var layout = container.GetComponent<LayoutGroup>();
            spacing = Mathf.Max(0, layout is VerticalLayoutGroup vertical ? vertical.spacing : sample._trackElementText.fontSize * 0.5f);
            if (layout)
                layout.enabled = false;
            var fitter = container.GetComponent<ContentSizeFitter>();
            if (fitter)
                fitter.enabled = false;
            if (container.parent && container.parent.GetComponent<LayoutGroup>())
            {
                var element = container.GetComponent<LayoutElement>() ?? container.gameObject.AddComponent<LayoutElement>();
                element.ignoreLayout = true;
            }
            container.anchorMin = container.anchorMax = container.pivot = new Vector2(1, 1);
            container.position = topRight;
            textGap = Mathf.Max(2, sample._trackElementText.fontSize * 0.15f);
            headerHeight = sample._trackElementText.fontSize * 1.5f;

            var labelObject = new GameObject("QuestTrackerPageIndicator", typeof(RectTransform), typeof(Text), typeof(Shadow));
            labelObject.transform.SetParent(container, false);
            indicator = labelObject.GetComponent<Text>();
            indicator.font = sample._trackElementText.font;
            indicator.fontSize = sample._trackElementText.fontSize;
            indicator.fontStyle = FontStyle.Normal;
            indicator.alignment = TextAnchor.MiddleRight;
            indicator.color = Color.white;
            indicator.raycastTarget = false;
            indicator.horizontalOverflow = HorizontalWrapMode.Wrap;
            labelObject.GetComponent<Shadow>().effectDistance = new Vector2(1, -1);
            SetRect(indicator.rectTransform, width, headerHeight, 0);
            initialized = true;
        }

        private void RebuildPages()
        {
            eligible.Clear();
            eligible.AddRange(nextEligible);
            heights.Clear();
            titleHeights.Clear();
            indices.Clear();
            foreach (var element in eligible)
            {
                GetRowRoot(element);
                indices[element] = heights.Count;
                var title = element._trackQuestNameText;
                var body = element._trackElementText;
                title.resizeTextForBestFit = body.resizeTextForBestFit = false;
                title.horizontalOverflow = body.horizontalOverflow = HorizontalWrapMode.Wrap;
                title.verticalOverflow = body.verticalOverflow = VerticalWrapMode.Overflow;
                SetRect(title.rectTransform, width, title.fontSize * 2, 0);
                SetRect(body.rectTransform, width, body.fontSize * 2, 0);
                float titleHeight = Mathf.Max(title.fontSize, title.preferredHeight);
                float bodyHeight = Mathf.Max(body.fontSize, body.preferredHeight);
                titleHeights.Add(titleHeight);
                heights.Add(titleHeight + textGap + bodyHeight);
            }
            // get some space for when the page hint wraps at a narrow width.
            SetRect(indicator.rectTransform, width, headerHeight, 0);
            int digits = Math.Max(1, eligible.Count);
            indicator.text = $"Quests {digits} / {digits}   [PgUp / PgDn]";
            headerHeight = Mathf.Max(indicator.fontSize * 1.5f, indicator.preferredHeight + textGap);
            SetRect(indicator.rectTransform, width, headerHeight, 0);
            budget = Mathf.Max(budget, headerHeight * 2f);
            container.sizeDelta = new Vector2(width, budget);
            pages = QuestTrackerPagination.BuildPages(heights, budget, spacing);
            if (pages.Count > 1)
                pages = QuestTrackerPagination.BuildPages(heights, budget - headerHeight, spacing);
            page = Mathf.Clamp(page, 0, Mathf.Max(0, pages.Count - 1));
            int anchor = eligible.IndexOf(anchorQuest);
            if (anchor >= 0)
                for (int i = 0; i < pages.Count; i++)
                    if (anchor >= pages[i].Start && anchor < pages[i].Start + pages[i].Count)
                        page = i;
        }

        private void ApplyPage(List<QuestTrackElement> allElements, bool layoutChanged)
        {
            bool multiple = pages.Count > 1;
            indicator.gameObject.SetActive(multiple);
            indicator.text = multiple ? $"Quests {page + 1} / {pages.Count}   [PgUp / PgDn]" : "";
            if (pages.Count == 0)
            {
                foreach (var root in rowRoots.Values)
                    if (root)
                        root.gameObject.SetActive(false);
                anchorQuest = null;
                return;
            }

            var current = pages[page];
            anchorQuest = eligible[current.Start];
            // we'll scale only oversized quests, individually
            float available = Mathf.Max(1, budget - (multiple ? headerHeight : 0));
            float scale = Mathf.Min(1, available / Mathf.Max(1, current.Height));
            float offset = multiple ? headerHeight : 0;
            foreach (var element in allElements)
            {
                if (!element)
                    continue;
                bool indexed = indices.TryGetValue(element, out int index);
                bool visible = indexed && index >= current.Start && index < current.Start + current.Count && !element._isHidden;
                if (!rowRoots.TryGetValue(element, out var row))
                    continue;
                // i don't know what that means so i'll leave that to clanker
                // game keeps controlling activeSelf on the original element
                // page visibility belongs to its wrapper, so off-page descendants remain inactiveInHierarchy even during the game's visibility loop.
                if (row.gameObject.activeSelf != visible)
                    row.gameObject.SetActive(visible);
                if (!visible || !layoutChanged)
                    continue;
                SetRect(row, width, heights[index], offset);
                row.localScale = new Vector3(scale, scale, 1);
                SetRect(element._trackQuestNameText.rectTransform, width, titleHeights[index], 0);
                SetRect(element._trackElementText.rectTransform, width,
                    heights[index] - titleHeights[index] - textGap, titleHeights[index] + textGap);
                offset += (heights[index] + spacing) * scale;
            }
        }

        private RectTransform GetRowRoot(QuestTrackElement element)
        {
            if (rowRoots.TryGetValue(element, out var root))
                return root;
            root = (RectTransform)new GameObject("QuestTrackerPageRow", typeof(RectTransform)).transform;
            root.SetParent(container, false);
            root.gameObject.SetActive(false);
            element.transform.SetParent(root, false);
            // independent row-relative coordinates in every prefab
            element._trackQuestNameText.transform.SetParent(root, false);
            element._trackElementText.transform.SetParent(root, false);
            foreach (var transform in new[] { element.transform, element._trackQuestNameText.transform, element._trackElementText.transform })
            {
                var layout = transform.GetComponent<LayoutGroup>();
                if (layout)
                    layout.enabled = false;
                var fitter = transform.GetComponent<ContentSizeFitter>();
                if (fitter)
                    fitter.enabled = false;
            }
            rowRoots.Add(element, root);
            return root;
        }

        private void RemoveStaleRows()
        {
            staleRoots.Clear();
            foreach (var entry in rowRoots)
                if (!entry.Key || !liveElements.Contains(entry.Key))
                    staleRoots.Add(entry.Key);
            foreach (var element in staleRoots)
            {
                var root = rowRoots[element];
                if (root)
                {
                    root.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(root.gameObject);
                }
                rowRoots.Remove(element);
            }
        }

        private static void SetRect(RectTransform rect, float width, float height, float offset)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(0, -offset);
        }
    }
}
