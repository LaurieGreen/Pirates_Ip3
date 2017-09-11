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
    class Enemy4 : Enemy
    {
        public Enemy4()
            : base()
        {
            movespeed = 50.0f;
            health = 20.0f;
            damage = 10.0f;
            body = null;
            geom = null;
        }

        public override bool Load(string assetName, ContentManager content, PhysicsSimulator simulator)
        {
            currentTexture = content.Load<Texture2D>(("pirate"));
            spriteWidth = currentTexture.Width;
            spriteHeight = currentTexture.Height;
            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            body = BodyFactory.Instance.CreateRectangleBody(currentTexture.Height, currentTexture.Width, 1);
            geom = GeomFactory.Instance.CreateRectangleGeom(body,currentTexture.Width, currentTexture.Height);
            body.Position = position;
            body.Tag = this;
            geom.Tag = this;
            simulator.Add(body);
            simulator.Add(geom);
            return true;
        }
        
        public void Update(GameTime time)
        {
            position.X = body.Position.X;
            position.Y = body.Position.Y;
           
        }   
    }
}
