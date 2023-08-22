using Microsoft.Xna.Framework;
using System;

namespace SoftShaderTest
{
    public class LightBall
    {
        public Vector3 colorOffset; //RGB offset from white
        public Vector2 position;
        public float velocity;

        private bool _moveRight = true;
        private bool _moveDown = true;

        private Random rand = new Random();

        private Vector3 screenDimm;//Width, height, margin

        private int maxVelocity = 10;

        public LightBall(float x, float y, float velocity, Vector3 colorOffset, Vector3 screenDimm)
        {
            this.position = new Vector2(x, y);
            this.velocity = velocity;
            this.colorOffset = colorOffset;
            this.screenDimm = screenDimm;

            int roll = rand.Next(1, 3);
            if (roll == 1) _moveRight = false;
            roll = rand.Next(1, 3);
            if (roll == 1) _moveDown = false;
        }

        public void Update()
        {
            if (position.X > screenDimm.X - screenDimm.Z)
            {
                _moveRight = false;
                velocity = rand.Next(0 - maxVelocity, maxVelocity) + 2;
            }
            else if (position.X <= 0)
            {
                _moveRight = true;
                velocity = rand.Next(0 - maxVelocity, maxVelocity) + 2;
            }
            if (position.Y <= 0)
            {
                _moveDown = true;
                velocity = rand.Next(0 - maxVelocity, maxVelocity) + 2;
            }
            else if (position.Y >= screenDimm.Y)
            {
                _moveDown = false;
                velocity = rand.Next(0 - maxVelocity, maxVelocity) + 2;
            }

            if (_moveRight)
            {
                position.X = position.X + velocity;
            }
            if (!_moveRight)
            {
                position.X = position.X - velocity;
            }
            if (_moveDown)
            {
                position.Y = position.Y + velocity;
            }
            if (!_moveDown)
            {
                position.Y = position.Y - velocity;
            }
        }
    }
}
