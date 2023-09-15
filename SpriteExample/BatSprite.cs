using CollisionExample.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpriteExample
{
    public enum Direction
    {
        Down = 0,
        Right = 1,
        Up = 2,
        Left = 3,

    }
    /// <summary>
    /// A class representing a bat sprite
    /// </summary>
    public class BatSprite
    {
        private Texture2D texture;
        private Texture2D texture2;
        private double directionTimer;
        private double animationTimer;

        private short animationFrame = 1;

        /// <summary>
        /// The direction of the bat
        /// </summary>
        public Direction Direction;
        /// <summary>
        /// Position of the bat
        /// </summary>
        public Vector2 Position;
        private BoundingCircle bounds = new BoundingCircle(new Vector2(0,0), 32);
        public BoundingCircle Bounds => bounds;


        /// <summary>
        /// Loads the bat sprite texture
        /// </summary>
        /// <param name="content">The content manager ot load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("32x32-bat-sprite");
            texture2 = content.Load<Texture2D>("magic");
        }
        /// <summary>
        /// Updates the bat sprite to fly in a pattern
        /// </summary>
        /// <param name="gameTime"> Game time</param>
        public void Update(GameTime gameTime)
        {   //Update the direction timer
            bounds.Center = Position;
            directionTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //Switch directions every 2 seconds
            if (directionTimer > 2.0)
            {
                switch (Direction) {

                    case Direction.Up:
                        Direction = Direction.Down;
                        break;
                    case Direction.Down:
                        Direction = Direction.Right;
                        break;
                    case Direction.Right:
                        Direction = Direction.Left;
                        break;
                    case Direction.Left:
                        Direction = Direction.Up;
                        break;
                }
                directionTimer -= 2.0;
            }

            //Move the bat in the direction it is flying
            switch (Direction)
            {
                case Direction.Up:
                  //  Position += new Vector2(0, -1) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case Direction.Down:
                  //  Position += new Vector2(0, 1) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case Direction.Left:
                   // Position += new Vector2(-1, 0) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case Direction.Right:
                //    Position += new Vector2(1, 0) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }
        }

        /// <summary>
        /// Drfaws the animated bat sprite
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Update animation timer
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            //Update animation frame
            if (animationTimer > 0.3)
            {
                animationFrame++;
                if (animationFrame > 3) animationFrame = 1;
                animationTimer -= 0.3;
            }

            //Draw the sprite
            var source = new Rectangle(animationFrame*32, (int)Direction * 32, 32, 32);
            spriteBatch.Draw(texture, Position, source, Color.White);
            var circleCenter = new Vector2(Bounds.Center.X - Bounds.Radius, Bounds.Center.Y - Bounds.Radius);
            var circleTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            circleTexture.SetData(new[] { Color.White });

            spriteBatch.Draw(circleTexture, circleCenter, null, Color.Red * 0.5f, 0f, Vector2.Zero, Bounds.Radius * 2, SpriteEffects.None, 0f);
        }
        



    }
}
