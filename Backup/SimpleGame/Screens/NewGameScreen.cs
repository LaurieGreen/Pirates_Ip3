#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion

namespace SimpleGame
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class NewGameScreen : MenuScreen
    {
        #region Fields

        SpriteBatch spriteBatch;
        SpriteFont font;
        public ContentManager content;
        Texture2D backgroundTexture;
        Rectangle viewportRect;
        public int currentimage;
        int previousimage;
        bool canswitch;
        
        enum Ungulate
        {
            BactrianCamel,
            Dromedary,
            Llama,
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public NewGameScreen()
            : base("New Game")
        {
            MenuEntry startgameMenuEntry = new MenuEntry("skip");

            startgameMenuEntry.Selected += StartGameMenuEntrySelected;

            MenuEntries.Add(startgameMenuEntry);

            currentimage = 0;
            canswitch = true;
        }

        void StartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        private void DrawText()
        {
            if (currentimage < 4)
            {
                spriteBatch.DrawString(font, "Press enter to skip ", new Vector2(10, 640), Color.Black);
                spriteBatch.DrawString(font, "Press space for next ", new Vector2(720, 640), Color.Black);
            }
            else
            {
                spriteBatch.DrawString(font, "Press enter to begin", new Vector2(350, 640), Color.Black);
            }
        }
        
        void SetMenuEntryText()
        {
            
        }
        public override void LoadContent()
        {
            content = new ContentManager(ScreenManager.Game.Services, "Content");
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            font = content.Load<SpriteFont>("Fonts/gamefont");
            backgroundTexture = content.Load<Texture2D>("intro1");
            viewportRect = new Rectangle(0, 65, backgroundTexture.Width, backgroundTexture.Height);           
        }
        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, fullscreen, Color.White);
            DrawText();
            spriteBatch.End();
        }
        protected override void Update(GameTime gameTime)
        {
            previousimage = currentimage;

            base.Update(gameTime);
            
        }
        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {

            KeyboardState newState = Keyboard.GetState();

            if ((newState.IsKeyDown(Keys.Space)) && currentimage == 0 && canswitch == true)
            {
                backgroundTexture = content.Load<Texture2D>("intro2");
                canswitch = false;
                currentimage++;
            }
            else if ((newState.IsKeyDown(Keys.Space)) && currentimage == 1 && canswitch == true)
            {
                backgroundTexture = content.Load<Texture2D>("intro3");
                canswitch = false;
                currentimage++;
            }
            else if ((newState.IsKeyDown(Keys.Space)) && currentimage == 2 && canswitch == true)
            {
                backgroundTexture = content.Load<Texture2D>("intro4");
                canswitch = false;
                currentimage++;
            }
            else if ((newState.IsKeyDown(Keys.Space)) && currentimage == 3 && canswitch == true)
            {
                backgroundTexture = content.Load<Texture2D>("intro5");
                canswitch = false;
                currentimage++;
            }
            else if ((newState.IsKeyUp(Keys.Space)))
            {
                canswitch = true;
            }
            base.HandleInput(input);
        }


        #endregion
    }
}
