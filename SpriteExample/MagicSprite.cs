using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;

namespace SpriteExample
{
    /// <summary>
    /// A class representing a slime ghost
    /// </summary>
    public class MagicSprite
    {


        private Texture2D texture;

        private bool flipped;

        public Vector2 position;

        public Vector2 velocity;

        public float size;


        private BoundingCircle bounds;
        public BoundingCircle Bounds => bounds;
        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("magic");
            Vector2 temp = new Vector2((int)position.X - (int)(size*256), (int)position.Y - (int)(size*256));
      
            this.bounds = new BoundingCircle(temp, size*256);
        }

        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {
            position += velocity*(float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 temp = new Vector2((int)position.X - (int)(size * 256), (int)position.Y - (int)(size * 256));

            bounds.Center = temp;
            bounds.Radius = size * 256;
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, position, null, new Color(size,size*2,size/2), 0, new Vector2(256, 256), size, spriteEffects, 0);
            var rect = new Rectangle((int)position.X- (int)(size * 256), (int)position.Y- (int)(size * 256), (int)(size * 256), (int)(size * 256));
            //spriteBatch.Draw(texture, rect, Color.Red);
        }
    }
}
