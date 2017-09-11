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
    class Enemy
    {
        public Vector2 origin;
        public Texture2D currentTexture;
        protected Body body;
        protected Geom geom;
        public bool Active;
        public float movespeed;
        public int spriteWidth;
        public int spriteHeight;
        public float health;
        public float damage;
        public TimeSpan damageCooldown;
        public TimeSpan hitAt;
        EnemyState enemyState;
        public enum EnemyState {Patroling,Chasing};
        public float initialposition;
        public int currentTime;
        protected float defaultmovespeed;
        public bool canattack;


        public float moveSpeed
        {
            get { return movespeed; }
            set { movespeed = value; }
        }

        public Enemy()
            : base()
        {
            defaultmovespeed = 80.0f;
            movespeed = 80.0f;
            health = 5.0f;
            damage = 20.0f;
            damageCooldown = TimeSpan.FromSeconds(3.0f);
            hitAt = TimeSpan.FromSeconds(0.0f);
            origin = new Vector2(0.0f, 0.0f);
            currentTexture = null; 
            Active = true;
            body = null;
            geom = null;
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
        public Vector2 position;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        public virtual bool Load(string assetName, ContentManager content, PhysicsSimulator simulator)
        {
            currentTexture = content.Load<Texture2D>((assetName));
            spriteWidth = 51;
            spriteHeight = 51;
            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            body = BodyFactory.Instance.CreateRectangleBody(currentTexture.Height, currentTexture.Width/5, 1);
            geom = GeomFactory.Instance.CreateRectangleGeom(body,currentTexture.Width/5, currentTexture.Height);
            body.Position = position;
            body.Tag = this;
            geom.Tag = this;
            simulator.Add(body);
            simulator.Add(geom);
            return true;
        }

        public void DoSwitch(Player playerSprite)
        {
            switch (enemyState)
            {
                case EnemyState.Patroling:
                    this.Body.LinearVelocity.X = -moveSpeed;
                    if (((position.X <= (initialposition - 20)) || (position.X >= (initialposition + 20))) && (currentTime != DateTime.Now.Second))
                    {
                        movespeed = -movespeed;
                        currentTime = DateTime.Now.Second;
                    }
                    break;
                case EnemyState.Chasing:
                    if (playerSprite.Position.X > this.Position.X)
                        this.Body.LinearVelocity.X = this.defaultmovespeed;
                    else if (playerSprite.Position.X < this.Position.X)
                        this.Body.LinearVelocity.X = -this.defaultmovespeed;
                    break;                 
            }
        }
        
        public void Update(GameTime time, Player playerSprite)
        {
            position.X = body.Position.X;
            position.Y = body.Position.Y;     
            if ((playerSprite.Body.Position.X <= (position.X + 250)) && (playerSprite.Body.Position.X >= (position.X - 250)))
            {
                enemyState = EnemyState.Chasing;
            }
            DoSwitch(playerSprite);

            if (Active == false)
            {

            }
           
        }   
    }
}
