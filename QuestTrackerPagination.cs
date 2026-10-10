using System;
using System.Collections.Generic;

namespace ConfigurableQuestsLimit
{
    internal readonly struct TrackerPage
    {
        internal readonly int Start;
        internal readonly int Count;
        internal readonly float Height;
        internal TrackerPage(int start, int count, float height)
        {
            Start = start;
            Count = count;
            Height = height;
        }
    }

    internal static class QuestTrackerPagination
    {
        internal static (float ScreenRight, float Width) FitHorizontalBounds(float preferredWidth, float safeLeft,
            float safeRight, float inset, float pixelsPerUnit)
        {
            float screenWidth = Math.Max(1, safeRight - safeLeft);
            float padding = Math.Max(0, Math.Min(inset, screenWidth * 0.25f));
            float availableWidth = (screenWidth - padding * 2) / Math.Max(0.01f, pixelsPerUnit);
            return (safeRight - padding, Math.Min(Math.Max(1, preferredWidth), Math.Max(1, availableWidth)));
        }

        internal static List<TrackerPage> BuildPages(IReadOnlyList<float> heights, float availableHeight, float spacing)
        {
            var pages = new List<TrackerPage>();
            float budget = Math.Max(1, availableHeight);
            float gap = Math.Max(0, spacing);
            int start = 0;
            int count = 0;
            float height = 0;
            for (int i = 0; i < heights.Count; i++)
            {
                float rowHeight = Math.Max(1, heights[i]);
                float nextHeight = height + (count > 0 ? gap : 0) + rowHeight;
                if (count > 0 && nextHeight > budget)
                {
                    pages.Add(new TrackerPage(start, count, height));
                    start = i;
                    count = 0;
                    height = 0;
                }
                height += (count > 0 ? gap : 0) + rowHeight;
                count++;
            }
            if (count > 0)
                pages.Add(new TrackerPage(start, count, height));
            return pages;
        }

        internal static int MovePage(int page, int direction, int pageCount)
        {
            if (pageCount <= 0)
                return 0;
            page = Math.Max(0, Math.Min(page, pageCount - 1));
            int next = (page + direction) % pageCount;
            return next < 0 ? next + pageCount : next;
        }
    }
}
