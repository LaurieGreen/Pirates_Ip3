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
    class Player 
    {
       
        public Vector2 origin;

        public Texture2D currentTexture;
        protected Body body;
        protected Geom geom;
        public TimeSpan redtime;

       
        protected float rotation;
        float jumpspeed;
        float movespeed;
        public int spriteWidth;
        public int spriteHeight;
        public float health;
        public int nukes;

        public bool cannuke;
        public bool isrunningleft;
        public bool isrunningright;
        public bool isjumping;
        public bool isfacingleft;
        public bool isfacingright;
        public bool canjump;
        public bool canshoot;
        public bool isidle;
        public bool isinbossfight;
        public bool ishit;


        public float jumpSpeed
        {
            get { return jumpspeed; }
            set { jumpspeed = value; }
        }
        public float moveSpeed
        {
            get { return movespeed; }
            set { movespeed = value; }
        }
        public Player()
            : base()
        {
            jumpspeed = 25000.0f;
            nukes = 0;
            redtime = TimeSpan.FromSeconds(0.5f);
            cannuke = true;
            movespeed = 150.0f;
            health = 100.0f;
            rotation = 0.0f;
            origin = new Vector2(0.0f, 0.0f);
            currentTexture = null;
            body = null;
            geom = null;
            isidle = true;
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
            set { position = value;
                    if (body != null)
                    { 
                    body.Position = value;
                }

            }
        }
        Vector2 position;

        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if (body != null)
                {
                    body.Rotation = value;
                }
            }
        }


        public bool IsStatic
        {
            get { return body.IsStatic; }
            set { body.IsStatic = value; }
        }


        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }
        int lives;

        public void AddGun()
        {
            Gun gun = new Gun();
        }

        //public virtual bool Load(string assetName, ContentManager content, PhysicsSimulator simulator, Vector2 startPosition1)
        //{
        //    currentTexture = content.Load<Texture2D>("Smaller Sheets/Idle/rightidle");
        //    spriteWidth = 28;
        //    spriteHeight = 42;
        //    origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
        //    body = BodyFactory.Instance.CreateRectangleBody(spriteHeight, spriteWidth, 1);
        //    geom = GeomFactory.Instance.CreateRectangleGeom(body, spriteWidth, spriteHeight);
        //    body.Position = startPosition1;
        //    body.Tag = this;
        //    geom.Tag = this;
        //    isAlive = true;
        //    isinbossfight = false;
        //    simulator.Add(body);
        //    simulator.Add(geom);
        //    return true;
        //}



        public void Update(GameTime time)
        {
            position.X = body.Position.X;
            position.Y = body.Position.Y;
            body.Rotation = 0.0f;
            GetInput();

            if (health <= 0.0f)
            {
                isAlive = false;
                Killplayer();
            }

            if (isAlive == false)
                health = 0.0f;
        }

        public void Killplayer()
        {

        }


        private void GetInput()
        {


        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //spriteBatch.Draw(arm.currentTexture,arm.position,null,Color.White,arm.rotation,arm.origin,1.0f,flip,0);
        }


    }
}
