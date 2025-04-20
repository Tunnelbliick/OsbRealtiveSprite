using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace storyboard.scriptslibrary
{
    public class Simplification
    {
        public float Movement { get; private set; } = 0.1f;
        public float Rotation { get; private set; } = 0.01f;
        public float Scale { get; private set; } = 0.01f;

        // Default constructor with typical values
        public Simplification() { }

        // Custom values constructor
        public Simplification(float movement, float rotation, float scale)
        {
            Movement = Math.Max(0, movement);
            Rotation = Math.Max(0, rotation);
            Scale = Math.Max(0, scale);
        }

        // Disable all simplification
        public static Simplification None => new Simplification(0, 0, 0);

        // Preset for high precision
        public static Simplification HighPrecision => new Simplification(0.01f, 0.001f, 0.001f);

        // Preset for low precision (more optimization)
        public static Simplification LowPrecision => new Simplification(1f, 0.1f, 0.1f);

        // Individual setters that ensure non-negative values
        public void SetMovement(float value) => Movement = Math.Max(0, value);
        public void SetRotation(float value) => Rotation = Math.Max(0, value);
        public void SetScale(float value) => Scale = Math.Max(0, value);

        // Disable individual components
        public void DisableMovement() => Movement = 0;
        public void DisableRotation() => Rotation = 0;
        public void DisableScale() => Scale = 0;
    }
}