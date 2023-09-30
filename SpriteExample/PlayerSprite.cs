using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SpriteExample
{
    /// <summary>
    /// A class representing a slime ghost
    /// </summary>
    public class PlayerSprite
    {
        private GamePadState gamePadState;

        private KeyboardState keyboardState;

        private Texture2D texture, texture2;

        public float zoom=1;

       




        private bool flipped;

        public Vector2 position = new Vector2(300, 200);

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {


            texture = content.Load<Texture2D>("rudeus");
            texture2 = content.Load<Texture2D>("hand");
        }

      
        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {
            gamePadState = GamePad.GetState(0);
            keyboardState = Keyboard.GetState();

            // Apply the gamepad movement with inverted Y axis
            position += gamePadState.ThumbSticks.Left * new Vector2(1, -1);
            if (gamePadState.ThumbSticks.Left.X < 0) flipped = true;
            if (gamePadState.ThumbSticks.Left.X < 0) flipped = false;

            // Apply keyboard movement
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) position += new Vector2(0, -1/zoom);
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) position += new Vector2(0, 1/zoom);
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                position += new Vector2(-1/zoom, 0);
                flipped = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                position += new Vector2(1/zoom, 0);
                flipped = false;
            }

        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 mousePosition)
        {
            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            // Calculate the angle between the position of texture2 and the mouse cursor
            mousePosition = mousePosition / zoom;
            Vector2 direction = mousePosition - position;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);

            // Draw texture1
            //spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(64, 64), 0.25f, spriteEffects, 0);

            // Draw texture2 with rotation towards the mouse
            spriteBatch.Draw(texture2, position+new Vector2(0,0), null, Color.White, rotation, new Vector2(16, 16), 2f, SpriteEffects.None, 0);
        }

    }
}
