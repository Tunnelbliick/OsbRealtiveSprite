using System;
using OpenTK;
using StorybrewCommon.Animations;
using StorybrewCommon.Storyboarding;

namespace storyboard.scriptslibrary
{
    public class VectorCommand
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public Vector2 Offset { get; set; }
        public OsbEasing Easing { get; set; }

        public VectorCommand(OsbEasing easing, double startTime, double endTime, Vector2 offset)
        {
            StartTime = startTime;
            EndTime = endTime;
            Offset = offset;
            Easing = easing;
        }

        /// <summary>
        /// Returns the relative contribution of this command at a given time.
        /// If the command hasn't started, returns (0,0); if completed, returns full offset.
        /// Otherwise, returns the interpolated offset based on the easing function.
        /// </summary>
        public Vector2 GetContributionAt(double time)
        {
            // Before the command starts, no effect.
            if (time < StartTime)
                return new Vector2(0, 0);

            // After the command has finished, full contribution is applied.
            if (time >= EndTime)
                return Offset;

            // Otherwise, compute eased progress between 0 and 1.
            double progress = (time - StartTime) / (EndTime - StartTime);
            progress = Math.Min(progress, 1.0);

            float easedProgress = (float)Easing.Ease(progress);
            // Lerp between zero offset and full offset.
            return Vector2.Lerp(new Vector2(0, 0), Offset, easedProgress);
        }
    }
}