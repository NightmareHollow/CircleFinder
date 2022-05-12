namespace CircleFinding
{
    public class Point
    {
        public float x { get; private set; }
        public float y { get; private set; }

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "{ " + x + ", " + y + " }";
        }
    }
}