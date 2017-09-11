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
    class Tile
    {
        protected Texture2D currentTexture;
        public Body body;
        public Geom geom;
        public bool Active;
        public Vector2 origin;
        public int spriteWidth;
        public int spriteHeight;

        public Tile()
            : base()
        {
            currentTexture = null;
            origin = new Vector2(0.0f, 0.0f);
            body = null;
            geom = null;
            Active = true;
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

        public bool IsStatic
        {
            get { return body.IsStatic; }
            set { body.IsStatic = value; }
        }


        public virtual bool Load(string assetName, ContentManager content, PhysicsSimulator simulator)
        {
            currentTexture = content.Load<Texture2D>("Tiles/facebook");
            spriteWidth = 32;
            spriteHeight = 32;
            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            body = BodyFactory.Instance.CreateRectangleBody(spriteWidth, spriteHeight, 1);
            geom = GeomFactory.Instance.CreateRectangleGeom(body, spriteWidth, spriteHeight);
            body.Position = position;
            body.Tag = this;
            geom.Tag = this;
            simulator.Add(body);
            simulator.Add(geom);
            IsStatic = true;
            return true;
        }


        public void Update(GameTime gameTime)
        {
            position.X = body.Position.X;
            position.Y = body.Position.Y;
        }
    }
}
