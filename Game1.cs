using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace SoftShaderTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int pixelRes = 17;
        float pixelSize;
        float screenWidth;
        float screenHeight;

        float margin;

        float dimming = 0.5f;

        Random rand;

        Vector2[][] positionMap;
        Vector3[][] colorMap;

        LightBall[] lights = new LightBall[3];

        Color background = new Color(41,37,74);

        int frameCounter = 0;
        int framesUntilRandomPixelResPicked = 60; //-1 for off      

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            rand = new Random();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1.25);
            _graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.25);
            _graphics.ApplyChanges();

            base.Initialize();

            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;          

            BakePixels(false);

            lights[0] = new LightBall(0, 0, 4, new Vector3(0.3f, 0.3f, 1.5f), new Vector3(screenWidth, screenHeight, margin));
            lights[1] = new LightBall(screenWidth, screenHeight / 3, 4, new Vector3(1.5f, 0.2f, 0.2f), new Vector3(screenWidth, screenHeight, margin));
            lights[2] = new LightBall(screenWidth/2, screenHeight/8, 4, new Vector3(0.3f, 1.5f, 0.3f), new Vector3(screenWidth, screenHeight, margin));

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
         
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(framesUntilRandomPixelResPicked > 0 && frameCounter >= framesUntilRandomPixelResPicked) // >= makes me feel safe
            {
                frameCounter = 0;
                BakePixels(true);
            }
            frameCounter++;

            for (int i = 0; i < lights.Length; i++) 
            {
                lights[i].Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();         

            for (int z = 0; z < lights.Length; z++)
            {
                LightBall ball = lights[z];
                for (int i = 0; i < pixelRes; i++)
                {
                    for (int j = 0; j < pixelRes; j++)
                    {
                        //float distanceToLight = Vector2.Distance(positionMap[i][j], ball.position);
                        float distanceToLight = FastIntDistance((int)positionMap[i][j].X, (int)positionMap[i][j].Y, (int)ball.position.X, (int)ball.position.Y);

                        float lighting = Math.Max(1 - (distanceToLight / screenWidth) - dimming, 0); //Must be positive or zero, no negative values. 

                        float r = colorMap[i][j].X + (lighting * ball.colorOffset.X);
                        float g = colorMap[i][j].Y + (lighting * ball.colorOffset.Y);
                        float b = colorMap[i][j].Z + (lighting * ball.colorOffset.Z);

                        colorMap[i][j] = new Vector3(r,g,b);

                        _spriteBatch.FillRectangle(new RectangleF((float)(positionMap[i][j].X + margin), (float)(positionMap[i][j].Y), pixelSize, pixelSize), new Color(r, g, b));
                    }
                }
            }

            //Reset color map
            for (int i = 0; i < pixelRes; i++)
            {
                colorMap[i] = new Vector3[pixelRes];
                for (int j = 0; j < pixelRes; j++)
                {
                    colorMap[i][j] = new Vector3(0f, 0f, 0f);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void BakePixels(bool shakeThingsUp)
        {
            pixelSize = calcPixelSize();
            margin = (screenWidth - (pixelRes * pixelSize)) / 2;

            if (shakeThingsUp)
            {
                pixelRes = rand.Next(2, 201);
                pixelSize = calcPixelSize();
                margin = (screenWidth - (pixelRes * pixelSize)) / 2;
            }

            //Pre-Bake pixel positions
            positionMap = new Vector2[pixelRes][];
            colorMap = new Vector3[pixelRes][];
            for (int i = 0; i < pixelRes; i++)
            {
                positionMap[i] = new Vector2[pixelRes];
                colorMap[i] = new Vector3[pixelRes];
                for (int j = 0; j < pixelRes; j++)
                {
                    positionMap[i][j] = new Vector2(i * pixelSize, j * pixelSize);
                    colorMap[i][j] = new Vector3(0f, 0f, 0f);
                }
            }
        }

        public float calcPixelSize()
        {
            return (float)_graphics.PreferredBackBufferHeight / (float)pixelRes;
        }

        public static int FastIntDistance(int x1, int y1, int x2, int y2)
        {
            int num = x1 - x2;
            int num2 = y1 - y2;
            return FastIntSqrt(num * num + num2 * num2);
        }

        /// <summary>
        /// Finds the integer square root of a positive number  
        /// https://stackoverflow.com/questions/5345552/fast-method-of-calculating-square-root-and-power
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int FastIntSqrt(int num)
        {
            if (0 == num) { return 0; }  // Avoid zero divide  
            int n = (num / 2) + 1;       // Initial estimate, never low  
            int n1 = (n + (num / n)) / 2;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (num / n)) / 2;
            } // end while  
            return n;
        } // end Isqrt()  
    }
}