using System.Drawing;

namespace BejeweledBot
{
    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public class Move
    {
        public Point Pt { get; set; }
        public Direction Direction { get; set; }

        public Move(Point pt, Direction dir)
        {
            Pt = pt;
            Direction = dir;
        }
    }
}
