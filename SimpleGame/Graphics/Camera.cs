using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework.Content;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework.Input;

namespace SimpleGame
{
    public class Camera
    {
        public Camera camera;
        public Camera()
        
        {
            Position = Vector2.Zero;
            Zoom = 60.0f;
            camera.Zoom =+ 100.0f;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(Position.X, Position.Y, 0);
            }
        }

    }

}
