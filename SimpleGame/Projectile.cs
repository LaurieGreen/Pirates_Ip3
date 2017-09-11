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
    class Projectile
    {
        public Vector2 origin;
        public Texture2D currentTexture;
        protected Body body;
        protected Geom geom;
        public bool Active;
        public Vector2 Direction;
        public Vector2 movespeed;
        public int spriteWidth;
        public int spriteHeight;

        public Vector2 moveSpeed
        {
            get { return movespeed; }
            set { movespeed = value; }
        }

        public Projectile()
            : base()
        {
        }

        public Body Body
        {
            get { return body; }
        }

        public Geom Geom
        {
            get { return geom; }
        }

        public Vector2 Origin
        {
            get { return origin; }
        }

        public Texture2D CurrentTexture
        {
            get { return currentTexture; }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (body != null)
                {
                    body.Position = value;
                }
            }
        }
        Vector2 position;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        public virtual bool Load(string assetName, ContentManager content, PhysicsSimulator simulator)
        {
            Active = true;
            currentTexture = content.Load<Texture2D>((assetName));
            spriteWidth = 3;
            spriteHeight = 3;
            origin = new Vector2(currentTexture.Width / 2, currentTexture.Height / 2);
            body = BodyFactory.Instance.CreateRectangleBody(simulator, currentTexture.Width, currentTexture.Height, 1);
            geom = GeomFactory.Instance.CreateRectangleGeom(body, currentTexture.Width, currentTexture.Height);
            body.Position = position;
            body.Tag = this;
            geom.Tag = this;
            simulator.Add(body);
            simulator.Add(geom);
            body.IgnoreGravity = true;
            return true;    
        }

        public void Update(GameTime time, Viewport viewport)
        {
            body.LinearVelocity.Y = 0;
            //body.LinearVelocity.X = movespeed;
            position.X = body.Position.X;
            position.Y = body.Position.Y;
            position += Direction * movespeed * (float)time.ElapsedGameTime.TotalSeconds;
        }
    }
}
