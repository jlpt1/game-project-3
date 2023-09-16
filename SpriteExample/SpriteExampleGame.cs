using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace SpriteExample
{
    /// <summary>
    /// A game demonstrating the use of sprites
    /// </summary>
    public class SpriteExampleGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private PlayerSprite slimeGhost;
        private Texture2D atlas;
        private float zoom = 1.0f;
        private Matrix transformMatrix;

        List<EyeSprite> bats = new List<EyeSprite>();
        List<MagicSprite> magics = new List<MagicSprite>();
        private SpriteFont bangers;
        private MouseState previousMouseState;




        /// <summary>
        /// Constructs the game
        /// </summary>
        public SpriteExampleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {

            // TODO: Add your initialization logic here
            slimeGhost = new PlayerSprite();
            EyeSprite bat = new EyeSprite() { Position = new Vector2(300, 100), Direction = Direction.Down };
            bats.Add(bat);
       
            base.Initialize();
            transformMatrix = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
        }

        /// <summary>
        /// Loads game content
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            slimeGhost.LoadContent(Content);
            atlas = Content.Load<Texture2D>("colored_packed");
            foreach (var bat in bats) bat.LoadContent(Content);
            foreach (var magic in magics) magic.LoadContent(Content);
            bangers = Content.Load<SpriteFont>("bangers");
        }
        MagicSprite magic = null;
        int mouseHeld = 0;
        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">the measured game time</param>
        protected override void Update(GameTime gameTime)
        {
            Random r = new Random();
            if (r.NextDouble() < 0.01)
            {
                Direction[] possibleDirections = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

                // Create a random number generator
               
                // Choose a random direction from the array
                Direction randomDirection = possibleDirections[r.Next(possibleDirections.Length)];
                EyeSprite bat = new EyeSprite() { Position = new Vector2(r.Next(20,600), r.Next(20, 600)), Direction = randomDirection };
                bat.LoadContent(Content);
                bats.Add(bat);
            };
            foreach (var magic in magics.ToList())
            {
                foreach (var bat in bats.ToList())
                {
                    if (magic != null)
                    {
                        if (magic.Bounds.CollidesWith(bat.Bounds))
                        {
                            magic.size -= 0.02f;
                            bat.size -= magic.size * 10;
                            if (magic.size <= 0.01f)
                            {
                                magics.Remove(magic);
                            }
                            if (bat.size <= 0.01f)
                            {
                                bats.Remove(bat);
                            }
                            

                        }
                    }
                }
                
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState mouseState = Mouse.GetState();
            float zoomSpeed = 0.001f;
            Console.Write(mouseState.ScrollWheelValue);
           
            zoom += zoomSpeed * (mouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue) * zoom;
            slimeGhost.zoom = zoom;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {

                if (mouseHeld == 0)
                {
                    magic = new MagicSprite() { position = slimeGhost.position, velocity = new Vector2(0, 0), size = 0.01f };

                    magic.LoadContent(Content);

                    magics.Add(magic);
                }
                magic.size *= (1.1f * (float)gameTime.ElapsedGameTime.TotalSeconds)+1f;
                if (magic.size >= 0.1f)
                {
                   // magic.size = 1f;
                }
                Vector2 mousepos = new Vector2(mouseState.Position.X, mouseState.Position.Y);
                mousepos = mousepos / zoom;
                Vector2 directionToMouse = mousepos - slimeGhost.position;

                // Calculate the distance from slimeGhost to mouse cursor
                float distanceToMouse = directionToMouse.Length();

                // Calculate the normalized direction vector (unit vector)
                Vector2 direction = directionToMouse / distanceToMouse;
                
                // Set the magic bullet's position based on the direction and radius
                float radius = 25; // Adjust this value as needed
                magic.position = slimeGhost.position + direction * ((magic.size*256/2)+radius)+new Vector2(128 * magic.size, 128 * magic.size);
                //magic.position = slimeGhost.position + direction * ((magic.size * 5.25f + 1.5f)) * radius + new Vector2(128 * magic.size, 128 * magic.size);


                mouseHeld = 1;



            }
            if (mouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                if (magic != null)
                {
                    Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                    mousePosition = mousePosition / zoom;
                    Vector2 direction = Vector2.Normalize(mousePosition - slimeGhost.position);
                    magic.velocity = direction * 10000 *magic.size; // Adjust the magnitude (speed) as needed.
                  
                }
                mouseHeld = 0;
            }



            previousMouseState = mouseState;
            // TODO: Add your update logic here
            foreach (var magic1 in magics) magic1.Update(gameTime);
            slimeGhost.Update(gameTime);
            foreach (var bat in bats) bat.Update(gameTime);
            zoom = MathHelper.Clamp(zoom, 0.001f, 2000.0f);
            transformMatrix = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game world
        /// </summary>
        /// <param name="gameTime">the measured game time</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
       
            bangers.MeasureString("This is a string to measure");
            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,null, transformMatrix);
           
            //spriteBatch.Draw(atlas, new Vector2(50, 50), new Rectangle(96, 16, 16, 16), Color.White);
            foreach (var bat in bats) bat.Draw(gameTime, spriteBatch);
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            slimeGhost.Draw(gameTime, spriteBatch,mousePosition);
            foreach (var magic in magics) magic.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(bangers, "Zoom: "+Math.Round(zoom,2), new Vector2(2, 2), Color.Gold);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
