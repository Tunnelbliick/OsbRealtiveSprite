using System;
using StorybrewCommon.Animations;
using StorybrewCommon.Storyboarding;

namespace storyboard.scriptslibrary
{
    public class DoubleCommand
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public double Offset { get; set; }
        public OsbEasing Easing { get; set; }

        public DoubleCommand(OsbEasing easing, double startTime, double endTime, double offset)
        {
            StartTime = startTime;
            EndTime = endTime;
            Offset = offset;
            Easing = easing;
        }

        /// <summary>
        /// Returns the relative contribution of this command at a given time.
        /// If the command hasn't started, returns 0; if completed, returns full offset.
        /// Otherwise, returns the interpolated offset based on the easing function.
        /// </summary>
        public double GetContributionAt(double time)
        {
            // Before the command starts, no effect.
            if (time < StartTime)
                return 0;

            // After the command has finished, full contribution is applied.
            if (time >= EndTime)
                return Offset;

            // Otherwise, compute eased progress between 0 and 1.
            double progress = (time - StartTime) / (EndTime - StartTime);
            progress = Math.Min(progress, 1.0);

            // Apply easing and lerp between 0 and offset
            return Offset * Easing.Ease(progress);
        }
    }
}