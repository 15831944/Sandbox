using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameStateManagement;


namespace CatapultGame
{
    class BackgroundScreen : GameScreen
    {
        Texture2D background;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            background = Load<Texture2D>("Textures/Backgrounds/title_screen");
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw Background
            spriteBatch.Draw(background, new Vector2(0, 0),
                new Color(255, 255, 255, TransitionAlpha));

            spriteBatch.End();
        }

    }
}
