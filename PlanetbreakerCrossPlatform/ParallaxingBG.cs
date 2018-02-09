using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Planetbreaker
{
    internal class ParallaxingBG
    {
        private Texture2D texture;
        // Ticks between movement
        private readonly int rate;
        // Ticks until next movement
        private int countdown;
        // Current scroll position
        private int pos;
        private int screenHeight, screenWidth;

        internal ParallaxingBG(Texture2D texture, int screenHeight, int rate)
        {
            this.texture = texture;
            this.screenHeight = screenHeight;
            screenWidth = texture.Width;
            this.rate = rate;
            countdown = rate;
        }

        internal void Draw(SpriteBatch batch)
        {
            Rectangle dest  = new Rectangle(0, pos, screenWidth, screenHeight - pos);
            Rectangle dwrap = new Rectangle(0, 0, screenWidth, pos);
            Rectangle src   = new Rectangle(0, 0, screenWidth, screenHeight - pos);
            Rectangle swrap = new Rectangle(0, screenHeight - pos, screenWidth, pos);

            batch.Draw(texture, dest, src, Color.White);
            batch.Draw(texture, dwrap, swrap, Color.White);
        }

        internal void Update()
        {
            --countdown;
            if (countdown == 0)
            {
                ++pos;
                if (pos == screenHeight) pos = 0;
                countdown = rate;
            }
        }
    }
}
