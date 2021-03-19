using System;
using Microsoft.Xna.Framework;

namespace Particles.Particles
{
    /// <summary>
    /// Expands upon some of the features the .NET Random class does:
    ///
    /// *NextRange : Generate a random number within some range
    /// *NextGaussian : Generate a normally distributed random number
    ///
    /// </summary>
    public class ParticleRandom : Random
    {
        /// <summary>
        /// Keep this around to optimize gaussian calculation performance.
        /// </summary>
        private double _y2;
        // same with this
        private bool _usePrevious;

        /// <summary>
        /// Generate a random vector about a unit circle
        /// </summary>
        public Vector2 NextCircleVector()
        {
            float angle = (float)(this.NextDouble() * 2.0 * Math.PI);
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Generates a random vector about a unit circle, given an angle and a width.
        /// </summary>
        public Vector2 NextAngleVector(float angle, float width)
        {
            // get the valid ratio between the width and the full unit circle
            var validRatio = width / MathHelper.TwoPi;
            // get a random angle that falls within range of angle
            var thisAngle = (float) (this.NextDouble() * validRatio * MathHelper.TwoPi + angle - width / 2f);

            // get xy coordinates
            var x = (float) Math.Cos(thisAngle);
            // reverse MonoGame coordinates
            var y = -1 * (float) Math.Sin(thisAngle);

            // return a Vector2 with these coordinates
            return new Vector2(x, y);
        }

        /// <summary>
        /// Generates a normalized random vector about a unit circle, given an angle and a width.
        /// </summary>
        public Vector2 NextAngleVectorNormalized(float angle, float width)
        {
            // get the valid ratio between the width and the full unit circle
            var validRatio = width / MathHelper.TwoPi;
            // get a random angle that falls within range of angle
            var thisAngle = (float) (this.NextGaussian(0.5,0.15)
                * validRatio * MathHelper.TwoPi + angle - width / 2f);

            // get xy coordinates
            var x = (float) Math.Cos(thisAngle);
            // reverse MonoGame coordinates
            var y = -1 * (float) Math.Sin(thisAngle);

            // return a Vector2 with these coordinates
            return new Vector2(x, y);
        }

        /// <summary>
        /// Generate a normally distributed random number.  Derived from a Wiki reference on
        /// how to do this.
        /// </summary>
        public double NextGaussian(double mean, double stdDev)
        {
            if (this._usePrevious)
            {
                this._usePrevious = false;
                return mean + _y2 * stdDev;
            }
            this._usePrevious = true;

            double x1;
            double x2;
            double z;

            do
            {
                x1 = 2.0 * this.NextDouble() - 1.0;
                x2 = 2.0 * this.NextDouble() - 1.0;
                z = (x1 * x1) + (x2 * x2);
            }
            while (z >= 1.0);

            z = Math.Sqrt((-2.0 * Math.Log(z)) / z);

            var y1 = x1 * z;
            _y2 = x2 * z;

            return mean + y1 * stdDev;
        }
    }
}
