using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace WindowsGame2
{
    class Example
    {
        //List of Textures that are used,
        List<Texture2D> textureList = new List<Texture2D>();
        int levelWidth = 25;
        int levelHeight = 15;
        //Array that will hold the stuff that gets grabbed from the file!
        int[,] map;
        //The size of one texture in pixels!
        int textureSize = 32;

        public Example(ContentManager contentManager)
        {
            //Stores it in the "WindowsGame2\WindowsGame2\bin\x86\Debug\Levels"
            String filename = @"Levels/1.txt";
            map = loadLevel(filename);

            textureList.Add(contentManager.Load<Texture2D>("example"));
        }
        public void Update(GameTime gameTime)
        {

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

        public void DrawTexturesOnScreen(int[,] levelMap, int textureSize, SpriteBatch spriteBatch, List<Texture2D> textures)
        {
            //This function is what you would put in place of your line of code that draws using mappy!! 
            for (int y = 0; y < levelHeight; y++)
            {
                //Reads a whole line of the Text file in, and stores it in the String.
                for (int x = 0; x < levelWidth; x++)
                {
                    //repeat this if statement for however many textures you have! I usually leave 0 as empty space, 
                    //then just feed the right index into the textures list, depending on which one you want to draw!
                    if (levelMap[y, x] == 1)
                    {
                        spriteBatch.Draw(textures[0], new Rectangle((x * textureSize), (y * textureSize), textureSize, textureSize), Color.White);
                    }
                }
            }
        }
    }
}
