using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System;
using System.Collections.Generic;
using System.Drawing.Text;

using System.Linq;
using static System.Collections.Specialized.BitVector32;
using System.Security.Policy;

namespace SpriteExample
{
    /// <summary>
    /// A game demonstrating the use of sprites
    /// </summary>
    public class SpriteExampleGame : Game
    {
        private GameState currentGameState = GameState.MainMenu;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Song backgroundMusic;
        private int kills = 0;
        private PlayerSprite slimeGhost;
        private Texture2D atlas;
        private float zoom = 1.0f;
        private Matrix transformMatrix;
        private Texture2D mainMenuBackground;
        private Texture2D backgroundSky;
        List<EyeSprite> bats = new List<EyeSprite>();
        List<MagicSprite> magics = new List<MagicSprite>();
        private SpriteFont bangers;
        private MouseState previousMouseState;
        private SoundEffect magicCreatedSound;
        private SoundEffect enemyHitSound;




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
            EyeSprite bat = new EyeSprite() { Position = new Vector2(200, 200), Direction = Direction.Down };
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
            mainMenuBackground = Content.Load<Texture2D>("clouds");
            backgroundSky = Content.Load<Texture2D>("clouds2");
            backgroundMusic = Content.Load<Song>("music");
            MediaPlayer.Play(backgroundMusic);
            magicCreatedSound = Content.Load<SoundEffect>("magicCreated");
            enemyHitSound = Content.Load<SoundEffect>("enemyHit");

            slimeGhost.LoadContent(Content);
            atlas = Content.Load<Texture2D>("colored_packed");
            foreach (var bat in bats) bat.LoadContent(Content);
            foreach (var magic in magics) magic.LoadContent(Content);
            bangers = Content.Load<SpriteFont>("bangers");
        }
        MagicSprite magic = null;
        int mouseHeld = 0;

        private void UpdateMainMenu(GameTime gameTime)
        {
            // Implement logic for navigating the main menu here
            // For instance, you might check for key presses to start the game or exit
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                currentGameState = GameState.Playing;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            Random r = new Random();
            TimeSpan pos = MediaPlayer.PlayPosition;
            
            if (pos.Minutes == 1 && pos.Seconds == 0)
            {
                EyeSprite bat = new EyeSprite() { Position = new Vector2(r.Next(20, 600), r.Next(20, 600)), Direction = Direction.Right };
                bat.LoadContent(Content);
                bat.size = 2f;
                bats.Add(bat);
            }
            if (r.NextDouble() < 0.01)
            {
                Direction[] possibleDirections = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

                // Create a random number generator

                // Choose a random direction from the array
                Direction randomDirection = possibleDirections[r.Next(possibleDirections.Length)];
                EyeSprite bat = new EyeSprite() { Position = new Vector2(r.Next(20, 600), r.Next(20, 600)), Direction = randomDirection };
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
                                kills++;
                                enemyHitSound.Play();
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
                magic.size *= (1.1f * (float)gameTime.ElapsedGameTime.TotalSeconds) + 1f;
                if (magic.size >= 0.1f)
                {
                    zoom = 1f-(float)(Math.Sqrt(magic.size)/5);
                   
                }
                if (magic.size >= 10f)
                {
                    magic.size = 10f;
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
                magic.position = slimeGhost.position + direction * ((magic.size * 256 / 2) + radius) + new Vector2(128 * magic.size, 128 * magic.size);
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
                    magic.velocity = direction * 10000 * magic.size; // Adjust the magnitude (speed) as needed.
                    magicCreatedSound.Play();

                }
                mouseHeld = 0;
            }



            previousMouseState = mouseState;
       
            foreach (var magic1 in magics) magic1.Update(gameTime);
            slimeGhost.Update(gameTime);
            foreach (var bat in bats) bat.Update(gameTime);
            zoom = MathHelper.Clamp(zoom, .02f, 5.0f);
            transformMatrix = Matrix.CreateScale(new Vector3(zoom, zoom, 1));
        }



        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">the measured game time</param>
        protected override void Update(GameTime gameTime)
        {

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(gameTime);
                    break;
                case GameState.Playing:
                    UpdatePlaying(gameTime);
                    break;
            }
          
            base.Update(gameTime);
        }


        private void DrawPlaying(GameTime gameTime)
        {
            TimeSpan position = MediaPlayer.PlayPosition;
            if (position.Minutes > 0)
            {
                GraphicsDevice.Clear(Color.Black);

            }
            else
            {
                GraphicsDevice.Clear(new Color(200,200,200));
            }
           

            bangers.MeasureString("This is a string to measure");
            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix);
            if (position.Minutes > 0)
            {

                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        spriteBatch.Draw(backgroundSky, new Rectangle(i * GraphicsDevice.Viewport.Width, j * GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(33,33,33, 255));
                    }
                }
            }
            else
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        spriteBatch.Draw(backgroundSky, new Rectangle(i * GraphicsDevice.Viewport.Width, j * GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0, 0, 255, 255));
                    }
                }
            }
            //spriteBatch.Draw(atlas, new Vector2(50, 50), new Rectangle(96, 16, 16, 16), Color.White);
            foreach (var bat in bats) bat.Draw(gameTime, spriteBatch, new Color(255,255,255,128));
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            slimeGhost.Draw(gameTime, spriteBatch, mousePosition);
            foreach (var magic in magics) magic.Draw(gameTime, spriteBatch);
            
            string positionText = "Recharge";
            if (position.Minutes < 1)
            {
                positionText = (60 - position.Seconds).ToString();
            }
            if (position.Minutes > 0)
            {
                
                for (int i = 0; i < 50; i++)
                {
                    spriteBatch.Draw(mainMenuBackground, new Rectangle(i*GraphicsDevice.Viewport.Width, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(255, 0, 0, 32));
                    for (int j = 0; j < 50; j++)
                    {
                        spriteBatch.Draw(backgroundSky, new Rectangle(i * GraphicsDevice.Viewport.Width, (j+1) * GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(255, 0, 0, 32));
                    }
                }
            }
            else
            {
                for (int i = 0; i < 50; i++)
                {
                    spriteBatch.Draw(mainMenuBackground, new Rectangle(i*GraphicsDevice.Viewport.Width, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(200, 200, 200, 32));
                    for (int j = 0; j < 50; j++)
                    {
                        spriteBatch.Draw(backgroundSky, new Rectangle(i * GraphicsDevice.Viewport.Width, (j+1) * GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(200, 200, 200, 32));
                    }
                }
            }

            spriteBatch.End();
            
        



            spriteBatch.Begin();
            spriteBatch.DrawString(bangers, "Score: " + kills, new Vector2(2, 2), Color.Gold);
            spriteBatch.DrawString(bangers, "Death: " + positionText, new Vector2(550, 2), Color.White);
            spriteBatch.DrawString(bangers, "Zoom: " + zoom, new Vector2(200, 2), Color.White);
            spriteBatch.End();
        }

        private void DrawMainMenu(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mainMenuBackground, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.DrawString(bangers, "Press Enter to Start", new Vector2(200, 200), Color.Black); 
            spriteBatch.DrawString(bangers, "Press Esc to Terminate", new Vector2(200, 300), Color.Black);
            spriteBatch.DrawString(bangers, "The Magician's Adventure", new Vector2(200, 0), Color.Gold);
            spriteBatch.DrawString(bangers, "Tip: Scroll to zoom out", new Vector2(0, 400), Color.Red);

            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    DrawMainMenu(gameTime);
                    break;
                case GameState.Playing:
                    DrawPlaying(gameTime);
                    break;
            }

            base.Draw(gameTime);
        }

  
       
    }
}
