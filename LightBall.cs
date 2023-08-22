using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool _firstIter = true;

        private Vector3 screenDimm;//Width, height, margin

        private int minVelocity = 3;
        private int maxVelocity = 7;

        public LightBall(float x, float y, float velocity, Vector3 colorOffset, Vector3 screenDimm) 
        { 
            this.position = new Vector2(x, y);
            this.velocity = velocity;
            this.colorOffset = colorOffset;
            this.screenDimm = screenDimm;

            int roll = rand.Next(1, 3);
            if(roll == 1) _moveRight = false;
            roll = rand.Next(1, 3);
            if (roll == 1) _moveDown = false;
        }

        public void Update()
        {
            if (position.X > screenDimm.X - screenDimm.Z)
            {
                _moveRight = false;
                velocity = rand.Next(0 - minVelocity, maxVelocity);
            }
            else if (position.X <= 0)
            {
                _moveRight = true;
                velocity = rand.Next(0 - minVelocity, maxVelocity);
            }
            if (position.Y <= 0)
            {
                _moveDown = true;
                velocity = rand.Next(0 - minVelocity, maxVelocity);
            }
            else if (position.Y >= screenDimm.Y)
            {
                _moveDown = false;
                velocity = rand.Next(0 - minVelocity, maxVelocity);
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

            if (_firstIter)
            {
                position.Y = position.Y - rand.Next(10, 26); ;
                _firstIter = false;
            }
        }
    }
}
