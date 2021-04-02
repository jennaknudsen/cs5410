using System;
using Microsoft.Xna.Framework;

namespace FinalProject_Tetris.Particles
{
    public class Particle
    {
        // the various properties of a Particle
        public readonly int Name;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Direction;
        public readonly float Speed;
        public TimeSpan Lifetime;

        // sets the attributes of this object
        public Particle(int name, Vector2 position, Vector2 direction, float speed, TimeSpan lifetime)
        {
            Name = name;
            Position = position;
            Direction = direction;
            Speed = speed;
            Lifetime = lifetime;
            Rotation = 0;
        }
    }
}
