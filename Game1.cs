using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace SoftShaderTest
{
    public class Game1 : Game
    {
        //User Options:
        int pixelRes = 100;
        int minRandomPixelRes = 2;
        int maxRandomPixelRes = 200;
        float dimming = 0.4f;
        Color background = new Color(41, 37, 74);
        int framesUntilRandomPixelResPicked = 60; //-1 for static pixel resolution.

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        float pixelSize; 
        float screenWidth;
        float screenHeight;
        float margin;

        Random rand;

        Vector2[][] positionMap;
        Vector3[][] colorMap;

        LightBall[] lights = new LightBall[3];

        int frameCounter = 0;
        int pixelWidthCount;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            rand = new Random();
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1.25);
            _graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.25);
            _graphics.ApplyChanges();

            base.Initialize();

            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            BakePixels(false);

            lights[0] = new LightBall(0, 0, 4, new Vector3(0.2f, 0.2f, 1.5f), new Vector3(screenWidth, screenHeight, margin));
            lights[1] = new LightBall(screenWidth, screenHeight / 3, 4, new Vector3(1.5f, 0.2f, 0.2f), new Vector3(screenWidth, screenHeight, margin));
            lights[2] = new LightBall(screenWidth / 2, screenHeight / 8, 4, new Vector3(0.2f, 1.5f, 0.2f), new Vector3(screenWidth, screenHeight, margin));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (framesUntilRandomPixelResPicked > 0 && frameCounter >= framesUntilRandomPixelResPicked) // >= makes me feel safe
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

            _spriteBatch.Begin();

            for (int i = 0; i < pixelRes; i++)
            {
                colorMap[i] = new Vector3[pixelWidthCount];

                for (int j = 0; j < pixelWidthCount; j++)
                {
                    for (int k = 0; k < lights.Length; k++) //For each pixel i,j and for each light k.
                    {
                        LightBall ball = lights[k];

                        if (k == 0) //If this is the first light being calculated on this pixel, reset its color map entry.
                        {
                            colorMap[i][j] = new Vector3(0f, 0f, 0f);
                        }

                        //float distanceToLight = Vector2.Distance(positionMap[i][j], ball.position);
                        float distanceToLight = Utility.FastIntDistance((int)positionMap[i][j].X, (int)positionMap[i][j].Y, (int)ball.position.X, (int)ball.position.Y);

                        float lighting = Math.Max(1 - (distanceToLight / screenWidth) - dimming, 0); //Must be positive or zero, no negative values. 

                        float r = colorMap[i][j].X + (lighting * ball.colorOffset.X);
                        float g = colorMap[i][j].Y + (lighting * ball.colorOffset.Y);
                        float b = colorMap[i][j].Z + (lighting * ball.colorOffset.Z);

                        colorMap[i][j] = new Vector3(r, g, b);

                        _spriteBatch.FillRectangle(new RectangleF((float)(positionMap[i][j].X + margin), (float)(positionMap[i][j].Y), pixelSize, pixelSize), new Color(r, g, b));
                    }

                    //At end of frame, clear color map entries.
                    if (i == pixelRes - 1 && j == pixelWidthCount - 1)
                    {
                        Array.Clear(colorMap, 0, colorMap.Length);
                    }
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates and bakes the position of each square on the screen.
        /// </summary>
        /// <param name="shakeThingsUp">If true, set the density of squares on the screen to a random number</param>
        public void BakePixels(bool shakeThingsUp)
        {
            pixelSize = Utility.CalcPixelSize(_graphics.PreferredBackBufferHeight, pixelRes);
            pixelWidthCount = (int)(_graphics.PreferredBackBufferWidth / pixelSize);

            margin = (screenWidth - (pixelWidthCount * pixelSize)) / 2;

            if (shakeThingsUp)
            {
                pixelRes = rand.Next(minRandomPixelRes, maxRandomPixelRes + 1);
                pixelSize = Utility.CalcPixelSize(_graphics.PreferredBackBufferHeight, pixelRes);
                pixelWidthCount = (int)(_graphics.PreferredBackBufferWidth / pixelSize);
                margin = (screenWidth - (pixelWidthCount * pixelSize)) / 2;
            }

            //Pre-Bake pixel positions
            positionMap = new Vector2[pixelRes][];
            colorMap = new Vector3[pixelRes][];
            for (int i = 0; i < pixelRes; i++)
            {
                positionMap[i] = new Vector2[pixelWidthCount];
                colorMap[i] = new Vector3[pixelWidthCount];
                for (int j = 0; j < pixelWidthCount; j++)
                {
                    positionMap[i][j] = new Vector2(j * pixelSize, i * pixelSize);
                    colorMap[i][j] = new Vector3(0f, 0f, 0f);
                }
            }
        }
    }
}