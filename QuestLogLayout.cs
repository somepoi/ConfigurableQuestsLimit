using System;

namespace ConfigurableQuestsLimit
{
    internal static class QuestLogLayout
    {
        internal static float ContentHeight(int count, float rowHeight, float stride, float viewportHeight)
        {
            return Math.Max(viewportHeight, Math.Max(0, count - 1) * stride + rowHeight);
        }

        internal static float SelectionOffset(int index, float rowHeight, float stride, float viewportHeight, float contentHeight, float offset)
        {
            float top = Math.Max(0, index) * stride;
            float bottom = top + rowHeight;
            if (top < offset)
                offset = top;
            else if (bottom > offset + viewportHeight)
                offset = bottom - viewportHeight;
            return Math.Max(0, Math.Min(offset, Math.Max(0, contentHeight - viewportHeight)));
        }
    }
}
