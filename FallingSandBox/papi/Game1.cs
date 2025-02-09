using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace papi
{
    public class Game1 : Game
    {
        public const int size_x = 120;
        public const int size_y = 60;
        public int timer = 0;
        public int timerpix = 0;
        public int btype = 1;
        public bool isPaused = false;
        public int brushSize = 1;
        bool alreadypressed = false;
        Random r = new Random();

        public Game1()
        {
            Storage.GDM = new GraphicsDeviceManager(this);
            Storage.GDM.GraphicsProfile = GraphicsProfile.Reach;
            Storage.CM = Content;
            Storage.game = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            Storage.GDM.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(11.1113333333333333333333333333333333333333333333333333); //60 frames/sec = 16.667ms pre frame
        }

        protected override void Initialize()
        {
            Storage.GDM.PreferredBackBufferWidth = 1920;
            Storage.GDM.PreferredBackBufferHeight = 1080;
            Storage.GDM.ApplyChanges();
            Storage.player = new Player[size_x, size_y];
            Storage.target = new RenderTarget2D(GraphicsDevice, size_x, size_y);
            Storage.targetBatch = new SpriteBatch(GraphicsDevice);
            Storage.particles = new Particle[size_x, size_y];
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    Storage.particles[x, y] = null;
                }
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            Storage.SB = new SpriteBatch(GraphicsDevice);
            if (Storage.texture == null)
            {
                // Texture used for drawing, will be scaled later.
                Storage.texture = new Texture2D(Storage.GDM.GraphicsDevice, 1, 1);
                Storage.texture.SetData<Microsoft.Xna.Framework.Color>(new Microsoft.Xna.Framework.Color[] { Microsoft.Xna.Framework.Color.SandyBrown });
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                for (int x = 0; x < size_x; x++)
                {
                    for (int y = 0; y < size_y; y++)
                    {
                        if (Storage.particles[x, y] != null)
                        {
                            Storage.particles[x, y] = null;
                        }
                        if (Storage.player[x, y] != null)
                        {
                            Storage.player[x, y] = null;
                        }
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                btype = 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
            {
                btype = 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
            {
                btype = 3;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                btype = 4;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.NumPad5))
            {
                btype = 5;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.Space) && timer > 500)
            {
                timer = 0;
                if (isPaused == false)
                {
                    isPaused = true;
                }
                else
                {
                    isPaused = false;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F) && alreadypressed == false)
            {
                alreadypressed = true;
                //if (Storage.GDM.IsFullScreen == false)
                //{
                    Storage.GDM.IsFullScreen = true;
                //}
                //else
                //{
                   // Storage.GDM.IsFullScreen = false;
                //}
            }
            if (Keyboard.GetState().IsKeyUp(Keys.F))
            {
                alreadypressed = false;
            }
            int timerphys = 0;
            timerphys += gameTime.ElapsedGameTime.Milliseconds;
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    var particle = Storage.particles[x, y];
                    if (particle != null && isPaused == false && find_afk(x, y))
                    {
                        particle.y_velocity += 1;
                        if (MathF.Abs(particle.y_velocity) > 1)
                        {
                            particle.y_velocity = 1 * Math.Sign(particle.y_velocity);
                        }

                        particle.x_velocity += 0;
                        if (MathF.Abs(particle.x_velocity) > 1)
                        {
                            particle.x_velocity = 1 * Math.Sign(particle.x_velocity);
                        }

                        int next_x = x + particle.x_velocity;
                        int next_y = y + particle.y_velocity;
                        if (!find_collision(next_x, next_y))
                        {
                            // Go there.
                        }
                        else if (!find_collision(next_x - 1, next_y))
                        {
                            next_x -= 1;
                        }
                        else if (!find_collision(next_x + 1, next_y))
                        {
                            next_x += 1;
                        }

                        else
                        {
                            // Can't move.
                            next_x = x;
                            particle.x_velocity = 0;

                            next_y = y;
                            particle.y_velocity = 0;
                        }
                        // Simulate water. Move side to side if you can.
                        if (particle.type == 2)
                        {
                            particle.y_velocity += 1;
                            if (MathF.Abs(particle.y_velocity) > 1)
                            {
                                particle.y_velocity = 1 * Math.Sign(particle.y_velocity);
                            }

                            particle.x_velocity += r.Next(0, 1);
                            if (MathF.Abs(particle.x_velocity) > 1)
                            {
                                particle.x_velocity = 1 * Math.Sign(particle.x_velocity);
                            }
                            if (!find_collision(next_x, next_y))
                            {
                                // Go there.
                            }
                            else if (!find_collision(next_x - 1, next_y))
                            {
                                next_x -= 1;
                            }
                            else if (!find_collision(next_x + 1, next_y))
                            {
                                next_x += 1;
                            }

                            else
                            {
                                // Can't move.
                                next_x = x;
                                particle.x_velocity = 0;

                                next_y = y;
                                particle.y_velocity = 0;
                            }
                        }
                        else if (particle.type == 3)
                        {
                            // Can't move.
                            next_x = x;
                            particle.x_velocity = 0;

                            next_y = y;
                            particle.y_velocity = 0;
                        }
                        else if (particle.type == 4 && timerphys > 75)
                        {
                            timerphys = 0;
                            if(!find_collision(next_x, next_y))
                            {
                                next_x = r.Next(-1, 1);
                            }
                        }
                        else if (particle.type == 5)
                        {
                            // Can't move.
                            next_x = x;
                            particle.x_velocity = 0;
                        }
                        Storage.particles[x, y] = null;
                        x = next_x;
                        y = next_y;
                        Storage.particles[x, y] = particle;
                    }
                }
            }
            timer += gameTime.ElapsedGameTime.Milliseconds;
            timerpix += gameTime.ElapsedGameTime.Milliseconds;
            for (int xp = 0; xp < size_x; xp++)
            {
                for (int yp = 0; yp < size_y; yp++)
                {
                    var player = Storage.player[xp, yp];
                    if (player != null && isPaused == false)
                    {
                        player.vy += 1;
                        if (MathF.Abs(player.vy) > 1)
                        {
                            player.vy = 1 * Math.Sign(player.vy);
                        }

                        player.vx += 0;
                        if (MathF.Abs(player.vx) > 1)
                        {
                            player.vx = 1 * Math.Sign(player.vx);
                        }

                        int next_xp = xp + player.vx;
                        int next_yp = yp + player.vy;
                        if (!find_collision(next_xp, next_yp))
                        {
                            // Go there.
                        }
                        //else if (!find_collision(next_xp - 1, next_yp))
                        //{
                         //   next_xp -= 1;
                       // }
                        //else if (!find_collision(next_xp + 1, next_yp))
                        //{
                         //   next_xp += 1;
                        //}

                        else
                        {
                            // Can't move.
                            next_xp = xp;
                            player.vx = 0;

                            next_yp = yp;
                            player.vy = 0;
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.D) && !find_collision(xp + 1, yp) && timerpix > 50)
                        {
                            timerpix = 0;
                            next_xp += 1;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.A) && !find_collision(xp - 1, yp) && timerpix > 50)
                        {
                            next_xp -= 1;
                            timerpix = 0;
                        }
                        Storage.player[xp, yp] = null;
                        xp = next_xp;
                        yp = next_yp;
                        Storage.player[xp, yp] = player;
                    }
                }
            }
            //player physics

            // Delay how fast you can create particles.
            var mouse_state = Mouse.GetState();
            if (mouse_state.LeftButton == ButtonState.Pressed)
            {
                var x = mouse_state.Position.X * size_x / Storage.GDM.PreferredBackBufferWidth;
                var y = mouse_state.Position.Y * size_y / Storage.GDM.PreferredBackBufferHeight;

                // Only create a particle in this grid location if nothing is currently there.
                if (x >= 0 && x < size_x && y >= 0 && y < size_y && Storage.particles[x, y] == null)
                {
                    Particle particle = new Particle();
                    particle.type = btype;
                    if (particle.type == 1)
                    {
                        particle.color = new Microsoft.Xna.Framework.Color(r.Next(207, 227), r.Next(204, 224), r.Next(110, 130));
                    }
                    else if (particle.type == 2)
                    {
                        particle.color = new Microsoft.Xna.Framework.Color(36, 54, r.Next(195, 245));
                    }
                    else if (particle.type == 3)
                    {
                        int kakab = r.Next(240, 255);
                        particle.color = new Microsoft.Xna.Framework.Color(kakab, kakab, kakab);
                    }
                    else if (particle.type == 4)
                    {
                        particle.color = new Microsoft.Xna.Framework.Color(r.Next(100, 255), r.Next(100, 255), r.Next(100, 255));
                    }
                    else if (particle.type == 5)
                    {
                        int kakab = r.Next(170, 190);
                        particle.color = new Microsoft.Xna.Framework.Color(r.Next(135, 150), kakab, kakab);
                    }
                    Storage.particles[x, y] = particle;
                }
            }
            if(mouse_state.RightButton == ButtonState.Pressed)
            {
                var x = mouse_state.Position.X * size_x / Storage.GDM.PreferredBackBufferWidth;
                var y = mouse_state.Position.Y * size_y / Storage.GDM.PreferredBackBufferHeight;
                if (x >= 0 && x < size_x && y >= 0 && y < size_y && Storage.particles[x, y] != null)
                {
                    Storage.particles[x, y] = null;
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.G)&& timer > 500)
            {
                var x = mouse_state.Position.X * size_x / Storage.GDM.PreferredBackBufferWidth;
                var y = mouse_state.Position.Y * size_y / Storage.GDM.PreferredBackBufferHeight;
                timer = 0;
                Player player = new Player();
                Storage.player[x, y] = new Player();
            }

            base.Update(gameTime);
        }

        bool find_collision(int x, int y)
        {
            if (y >= size_y)
            {
                return true;
            }
            if (x <= -1 || x >= size_x)
            {
                return true;
            }
            return Storage.particles[x, y] != null;
        }
        bool is_liquid(int x, int y)
        {
            return Storage.particles[x, y].type == 2;
        }
        bool find_afk(int x, int y)
        {
            if (Storage.particles[x, y] != null)
            {
                //if(find_collision(x + 1, y) && find_collision(x - 1, y) && find_collision(x, y + 1) && find_collision(x, y - 1))
                //{
                    return true;
                //}
            }
            return Storage.particles[x, y].y_velocity == 100;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(Storage.target);

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            Storage.SB.Begin();
            //Storage.SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            //int depth = 0;

            //var player = Storage.player;
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    //depth += 1;
                    Particle particle = Storage.particles[x, y];
                    if (particle != null)
                    {
                        Vector2 pos = new Vector2(x, y);
                        //Draw each particle as a sprite.
                        Storage.SB.Draw(Storage.texture,
                            pos,
                            Storage.drawRec,
                            particle.color,
                            0,
                            Vector2.Zero,
                            1.0f, // Scale
                            SpriteEffects.None,
                           0.000f);
                    }
                }
            }
            for (int xp = 0; xp < size_x; xp++)
            {
                for (int yp = 0; yp < size_y; yp++)
                {
                    //depth += 1;
                    var player = Storage.player[xp, yp];
                    if (player != null)
                    {
                        Vector2 pos = new Vector2(xp, yp);
                        //Draw each particle as a sprite.
                        Storage.SB.Draw(Storage.texture,
                            pos,
                            Storage.drawRec,
                            player.color,
                            0,
                            Vector2.Zero,
                            1.0f, // Scale
                            SpriteEffects.None,
                           0.000f);
                    }
                }
            }
            Storage.SB.End();

            // Set rendering back to the back buffer.
            GraphicsDevice.SetRenderTarget(null);

            // Render target to back buffer.
            Storage.targetBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            // Scale target from 64x64 to 512x512.
            Storage.targetBatch.Draw(Storage.target, new Microsoft.Xna.Framework.Rectangle(0, 0, Storage.GDM.PreferredBackBufferWidth, Storage.GDM.PreferredBackBufferHeight), Microsoft.Xna.Framework.Color.White);
            Storage.targetBatch.End();

            base.Draw(gameTime);
        }
    }
}
