namespace SoftShaderTest
{
    public static class Utility
    {
        /// <summary>
        /// Calculates the distance using integer math only.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
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
        }

        /// <summary>
        /// Calculates the size each square needs to be in order to fill the screen.
        /// </summary>
        /// <returns></returns>
        public static float CalcPixelSize(float windowHeight, float pixelRes)
        {
            return windowHeight / pixelRes;
        }
    }
}
