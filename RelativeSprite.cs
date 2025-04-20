using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewScripts;

namespace storyboard.scriptslibrary
{
    public class RelativeSprite
    {
        private readonly List<VectorCommand> movementCommands = new List<VectorCommand>();

        private readonly List<VectorCommand> ScaleCommands = new List<VectorCommand>();

        private readonly List<DoubleCommand> RotationCommands = new List<DoubleCommand>();

        private readonly List<DoubleCommand> FadeCommands = new List<DoubleCommand>();

        private readonly KeyframedValue<Vector2> movement = new KeyframedValue<Vector2>(null);
        private readonly KeyframedValue<Vector2> scale = new KeyframedValue<Vector2>(null);
        private readonly KeyframedValue<float> rotation = new KeyframedValue<float>(null);
        private readonly KeyframedValue<float> fade = new KeyframedValue<float>(null);

        private StoryboardLayer layer { get; set; }
        private OsbOrigin origin { get; set; }
        private string path { get; set; }
        private Vector2 initialPosition { get; set; }
        private OsbSprite sprite;

        protected StoryboardObjectGenerator ctx;

        // Cache for computed values
        private Dictionary<double, Vector2> positionCache = new Dictionary<double, Vector2>();
        private Dictionary<double, Vector2> scaleCache = new Dictionary<double, Vector2>();
        private Dictionary<double, double> rotationCache = new Dictionary<double, double>();
        private Dictionary<double, double> opacityCache = new Dictionary<double, double>();

        // Optional: Add cache size limits to prevent memory issues
        private const int MAX_CACHE_SIZE = 1000;

        public RelativeSprite(StoryboardLayer layer, string path)
            : this(layer, path, OsbOrigin.Centre, new Vector2(0, 0))
        {
        }

        public RelativeSprite(StoryboardLayer layer, string path, OsbOrigin origin)
            : this(layer, path, origin, new Vector2(0, 0))
        {
        }

        public RelativeSprite(StoryboardLayer layer, string path, OsbOrigin origin, Vector2 initialPosition, StoryboardObjectGenerator ctx = null)
        {
            this.layer = layer;
            this.origin = origin;
            this.path = path;
            this.initialPosition = initialPosition;
            sprite = layer.CreateSprite(path, origin, initialPosition);
            if (ctx != null)
            {
                this.ctx = ctx;
            }
        }

        public void Move(OsbEasing easing, double startTime, double endTime, Vector2 offset)
        {
            movementCommands.Add(new VectorCommand(easing, startTime, endTime, offset));
            InvalidateCache();
        }

        public void Move(double startTime, double endTime, Vector2 offset)
        {
            Move(OsbEasing.None, startTime, endTime, offset);
        }

        public void Move(double startTime, Vector2 offset)
        {
            Move(OsbEasing.None, startTime, startTime, offset);
        }

        public void MoveX(OsbEasing easing, double startTime, double endTime, double offset)
        {
            movementCommands.Add(new VectorCommand(easing, startTime, endTime, new Vector2((float)offset, 0)));
            InvalidateCache();
        }

        public void MoveX(double startTime, double endTime, double offset)
        {
            MoveX(OsbEasing.None, startTime, endTime, offset);
        }

        public void MoveX(double startTime, double offset)
        {
            MoveX(OsbEasing.None, startTime, startTime, offset);
        }

        public void MoveY(OsbEasing easing, double startTime, double endTime, double offset)
        {
            movementCommands.Add(new VectorCommand(easing, startTime, endTime, new Vector2(0, (float)offset)));
            InvalidateCache();
        }

        public void MoveY(double startTime, double endTime, double offset)
        {
            MoveX(OsbEasing.None, startTime, endTime, offset);
        }

        public void MoveY(double startTime, double offset)
        {
            MoveX(OsbEasing.None, startTime, startTime, offset);
        }

        public void ScaleVec(OsbEasing easing, double startTime, double endTime, Vector2 offset)
        {
            ScaleCommands.Add(new VectorCommand(easing, startTime, endTime, offset));
            InvalidateCache();
        }

        public void ScaleVec(double startTime, double endTime, Vector2 offset)
        {
            ScaleVec(OsbEasing.None, startTime, endTime, offset);
        }


        public void ScaleVec(double startTime, Vector2 offset)
        {
            ScaleVec(OsbEasing.None, startTime, startTime, offset);
        }

        public void ScaleVert(OsbEasing easing, double startTime, double endTime, double offset)
        {
            ScaleCommands.Add(new VectorCommand(easing, startTime, endTime, new Vector2((float)offset, 0)));
            InvalidateCache();
        }

        public void ScaleVert(double startTime, double endTime, double offset)
        {
            ScaleVert(OsbEasing.None, startTime, endTime, offset);
        }

        public void ScaleVert(double startTime, double offset)
        {
            ScaleVert(OsbEasing.None, startTime, startTime, offset);
        }

        public void ScaleHoriz(OsbEasing easing, double startTime, double endTime, double offset)
        {
            ScaleCommands.Add(new VectorCommand(easing, startTime, endTime, new Vector2(0, (float)offset)));
            InvalidateCache();
        }

        public void ScaleHoriz(double startTime, double endTime, double offset)
        {
            ScaleHoriz(OsbEasing.None, startTime, endTime, offset);
        }

        public void ScaleHoriz(double startTime, double offset)
        {
            ScaleHoriz(OsbEasing.None, startTime, startTime, offset);
        }

        public void Scale(OsbEasing easing, double startTime, double endTime, double offset)
        {
            ScaleCommands.Add(new VectorCommand(easing, startTime, endTime, new Vector2((float)offset, (float)offset)));
            InvalidateCache();
        }

        public void Scale(double startTime, double endTime, double offset)
        {
            Scale(OsbEasing.None, startTime, endTime, offset);
        }

        public void Scale(double startTime, double offset)
        {
            Scale(OsbEasing.None, startTime, startTime, offset);
        }

        public void Rotate(OsbEasing easing, double startTime, double endTime, double offset)
        {
            RotationCommands.Add(new DoubleCommand(easing, startTime, endTime, offset));
            InvalidateCache();
        }

        public void Rotate(double startTime, double endTime, double offset)
        {
            Rotate(OsbEasing.None, startTime, endTime, offset);
        }

        public void Rotate(double startTime, double offset)
        {
            Rotate(OsbEasing.None, startTime, startTime, offset);
        }

        public void Fade(OsbEasing easing, double startTime, double endTime, double start, double end)
        {
            sprite.Fade(easing, startTime, endTime, start, end);
        }

        public void Fade(double startTime, double endTime, double start, double end)
        {
            sprite.Fade(startTime, endTime, start, end);
        }

        public void Fade(double startTime, double start)
        {
            sprite.Fade(startTime, start);
        }

        public void Color(OsbEasing easing, double startTime, double endTime, Color4 start, Color4 end)
        {
            sprite.Color(easing, startTime, endTime, start, end);
        }

        public void Color(double startTime, double endTime, Color4 start, Color4 end)
        {
            sprite.Color(startTime, endTime, start, end);
        }

        public void Color(double startTime, Color4 start)
        {
            sprite.Color(startTime, start);
        }


        public Vector2 PositionAt(double time)
        {
            if (positionCache.TryGetValue(time, out Vector2 cachedPosition))
                return cachedPosition;

            Vector2 position = initialPosition;
            foreach (var command in movementCommands)
            {
                position += command.GetContributionAt(time);
            }

            // Cache the result
            if (positionCache.Count > MAX_CACHE_SIZE)
                positionCache.Clear();
            positionCache[time] = position;

            return position;
        }

        public Vector2 ScaleAt(double time)
        {
            if (scaleCache.TryGetValue(time, out Vector2 cachedScale))
                return cachedScale;

            Vector2 scale = Vector2.Zero;
            foreach (var command in ScaleCommands)
            {
                scale += command.GetContributionAt(time);
            }

            if (scaleCache.Count > MAX_CACHE_SIZE)
                scaleCache.Clear();
            scaleCache[time] = scale;

            return scale;
        }

        public double RotationAt(double time)
        {
            if (rotationCache.TryGetValue(time, out double cachedRotation))
                return cachedRotation;

            double rotation = 0;
            foreach (var command in RotationCommands)
            {
                rotation += command.GetContributionAt(time);
            }

            if (rotationCache.Count > MAX_CACHE_SIZE)
                rotationCache.Clear();
            rotationCache[time] = rotation;

            return rotation;
        }

        // Add method to clear caches when new commands are added
        private void InvalidateCache()
        {
            positionCache.Clear();
            scaleCache.Clear();
            rotationCache.Clear();
        }

        public void WriteToSprite(double startTime, double endTime, double stepSize = 1, Simplification simplification = null)
        {
            // Use default simplification if none provided
            simplification = simplification ?? new Simplification();

            // Clear existing keyframes
            movement.Clear();
            scale.Clear();
            rotation.Clear();

            // Generate keyframes for each time step
            for (double time = startTime; time <= endTime; time += stepSize)
            {
                movement.Add(time, PositionAt(time));
                scale.Add(time, ScaleAt(time));
                rotation.Add(time, (float)RotationAt(time));
            }

            // Apply simplification values
            movement.Simplify(simplification.Movement);
            scale.Simplify(simplification.Scale);
            rotation.Simplify(simplification.Rotation);

            // Apply keyframes to sprite
            movement.ForEachPair((start, end) =>
            {
                sprite.Move(start.Time, end.Time, start.Value, end.Value);
            });

            scale.ForEachPair((start, end) =>
            {
                sprite.ScaleVec(start.Time, end.Time, start.Value, end.Value);
            });

            rotation.ForEachPair((start, end) =>
            {
                sprite.Rotate(start.Time, end.Time, start.Value, end.Value);
            });

            // Clear commands and cache after writing
            movementCommands.Clear();
            ScaleCommands.Clear();
            RotationCommands.Clear();
            InvalidateCache();
        }
    }
}