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
#endregion

namespace SimpleGame
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class HighscoreScreen : MenuScreen
    {
        #region Fields

        
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
        public HighscoreScreen()
            : base("Highscore")
        {
           
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        /// 


        private void DrawText()
        {

        }

        void SetMenuEntryText()
        {
            
        }


        #endregion

        #region Handle Input




        #endregion
    }
}
