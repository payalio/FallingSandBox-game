using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace papi
{
    public static class Storage
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 game;
        public static Texture2D texture;
        public static Rectangle drawRec = new Rectangle(0, 0, 1, 1);
        public static Particle[,] particles;
        public static Player[,] player;
        public static SpriteBatch targetBatch;
        public static RenderTarget2D target;
    }

    public class Player
    {
        public int x = 100, y = 0;
        public int sx = 4, sy = 8;
        public int vx = 0, vy = 0;
        public Color color = Color.Green;
    }

    public class Particle
    {
        public int x_velocity, y_velocity;
        public int type = 1;
        public Color color;
    }
}
