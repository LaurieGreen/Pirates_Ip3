using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Storage;


namespace SimpleGame
{
    class Map
    {
        //List of Textures that are used,
        public List<Texture2D> textureList = new List<Texture2D>();

        public List<Tile> tileList = new List<Tile>();
        public List<Tile> grayList = new List<Tile>();
        public List<Tile> boundryList = new List<Tile>();
        public List<Tile> endList = new List<Tile>();
        public List<Tile> firewallspawnList = new List<Tile>();
        public List<Tile> enemy1spawnList = new List<Tile>();
        public List<Tile> firespawnList = new List<Tile>();
        public List<Tile> oiledList = new List<Tile>();
        public List<Tile> wikiList = new List<Tile>();
        //Tile tile;
        public int levelWidth = 268;
        public int levelHeight = 18;
        PhysicsSimulator simulator = new PhysicsSimulator(new Vector2(0, 2000));
        //Array that will hold the stuff that gets grabbed from the file!
        int[,] map;
        //The size of one texture in pixels!
        public int textureSize = 32;
        public int currentlevel = 1;

        public Map(ContentManager content)
        {
            //Stores it in the "WindowsGame2\WindowsGame2\bin\x86\Debug\Levels"
            LoadLevel(content);
            textureList.Add(content.Load<Texture2D>("Tiles/facebook"));
            textureList.Add(content.Load<Texture2D>("Tiles/Folder"));
            textureList.Add(content.Load<Texture2D>("Tiles/Fire"));
            textureList.Add(content.Load<Texture2D>("Tiles/Wikipedia"));
            textureList.Add(content.Load<Texture2D>("Tiles/Firewall"));
            textureList.Add(content.Load<Texture2D>("Tiles/gray"));
            textureList.Add(content.Load<Texture2D>("Tiles/FacebookOiled"));

            
        }
        public void Update(GameTime gameTime)
        {

        }

        public void LoadLevel(ContentManager content)
        {
            if (currentlevel == 1)
            {
                String filename = @"Content/1.txt";
                map = loadLevel(filename);
            }
            if (currentlevel == 2)
            {
                String filename = @"Content/2.txt";
                map = loadLevel(filename);
            }
            if (currentlevel == 3)
            {
                String filename = @"Content/3.txt";
                map = loadLevel(filename);
            } 
            MakeList(content, map);

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            DrawTexturesOnScreen(map, textureSize, spriteBatch, textureList);
        }
        public int[,] loadLevel(String filename)
        {
            //Array that will hold the level by the end of this function.
            int[,] loadedLevel = new int[levelHeight, levelWidth];

            //Creates a File Stream of the file and passes it to the StreamReader, the Create Filemode means that 
            FileStream f = new FileStream(filename, FileMode.Open);
            StreamReader s = new StreamReader(f);
            for (int y = 0; y < levelHeight; y++)
            {
                //Reads a whole line of the Text file in, and stores it in the String.
                String lineOfFile = s.ReadLine();

                for (int x = 0; x < levelWidth; x++)
                {
                    //Grabs whatevers inbetween the commas, then parses it to an int and adds it to the 2d array!
                    String[] row = lineOfFile.Split(',');
                    loadedLevel[y, x] = int.Parse(row[x]);

                }
            }
            s.Close();
            return loadedLevel;
        }

        public void MakeList(ContentManager content, int[,] levelbody)
        {

            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    if (levelbody[y, x] == 01)//normal
                    {
                        Tile tile = new Tile();
                        tile.Load("assetname", content, simulator);
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        tileList.Add(tile);
                    }
                    if (levelbody[y, x] == 13)//normal
                    {
                        Tile tile = new Tile();
                        tile.Load("assetname", content, simulator);
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        wikiList.Add(tile);
                    }
                    if (levelbody[y, x] == 02)//oiled
                    {
                        Tile tile = new Tile();
                        tile.Load("assetname", content, simulator);
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        oiledList.Add(tile);
                    }
                    if (levelbody[y, x] == 03)//fire
                    {
                        Tile tile = new Tile();
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        firespawnList.Add(tile);
                    }
                    if (levelbody[y, x] == 10)//enemy spawn
                    {
                        Tile tile = new Tile();
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        enemy1spawnList.Add(tile);                       
                    }
                    if (levelbody[y, x] == 22)//exit
                    {
                        Tile tile = new Tile();
                        tile.Load("assetname", content, simulator);
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        endList.Add(tile);
                    }
                    if (levelbody[y, x] == 23)//boundry
                    {
                        Tile tile = new Tile();
                        tile.Load("checkpoint", content, simulator);
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        boundryList.Add(tile);                    
                    }
                    if (levelbody[y, x] == 24)//firewall
                    {
                        Tile tile = new Tile();
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        firewallspawnList.Add(tile);
                    }
                    if (levelbody[y, x] == 25)//gray
                    {
                        Tile tile = new Tile();
                        tile.Load("assetname", content, simulator);
                        tile.Position = new Vector2((x * textureSize), (y * textureSize));
                        grayList.Add(tile);
                    }
                }
            }
        }

        public void Dispose()
        {
            textureList.Clear();
            tileList.Clear();
        }

        //int i = 0;
        public void DrawTexturesOnScreen(int[,] levelMap, int textureSize, SpriteBatch spriteBatch, List<Texture2D> textures)
        {
            //This function is what you would put in place of your line of code that draws using mappy!! 
            for (int y = 0; y < levelHeight; y++)
            {
                //Reads a whole line of the Text file in, and stores it in the String.
                for (int x = 0; x < levelWidth; x++)
                {

                    if (levelMap[y, x] == 01)//normal
                    {
                        spriteBatch.Draw((textures[0]), new Vector2(x * textureSize, y * textureSize), Color.White);
                    }
                    else if (levelMap[y, x] == 02)//folder
                    {
                        spriteBatch.Draw((textures[6]), new Vector2(x * textureSize, y * textureSize), Color.White);
                    }
                    else if (levelMap[y, x] == 09)//folder
                    {
                        spriteBatch.Draw((textures[1]), new Vector2(x * textureSize, y * textureSize), Color.White);
                    }
                    else if (levelMap[y, x] == 13)//folder
                    {
                        spriteBatch.Draw((textures[3]), new Vector2(x * textureSize, y * textureSize), Color.White);
                    }
                    else if (levelMap[y, x] == 25)//gray
                    {
                        spriteBatch.Draw((textures[2]), new Vector2(x * textureSize, y * textureSize), Color.White);
                    }
                    else if (levelMap[y, x] == 22)//exit
                    {
                        spriteBatch.Draw((textures[5]), new Vector2(x * textureSize, y * textureSize), Color.White);
                    } 
                }
            }
        }
    }
}
