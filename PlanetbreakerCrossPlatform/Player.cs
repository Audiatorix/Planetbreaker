using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Planetbreaker.Utilities;
using Planetbreaker.Attacks;


namespace Planetbreaker
{
    internal class Player : LivingGameEntity
    {
        private RectHitbox rectArea;

        private int cannonAmmo = cannonCap, torpedos = torpedoCap, missiles = missileCap;
        private const int cannonCap = 45, torpedoCap = 3, missileCap = 1;
        private const int cannonRL = 120, torpedoRL = 180, missileRL = 600;
        private int cannonRLT = cannonRL, torpedoRLT = torpedoRL, missileRLT = missileRL;
        private const int cannonROF = 5, torpedoROF = 15;
        private int cannonCD = 0, torpedoCD = 0;

        internal Player(IHitbox area, Texture2D texture, Texture2D armoredTexture, Texture2D shieldedTexture) 
            : base(area, texture, armoredTexture, shieldedTexture, 100, 100, 100, 3)
        {
            rectArea = (RectHitbox) area;
        }

        internal Point Center()
        {
            return new Point(rectArea.X1 + rectArea.Width / 2, rectArea.Y1 + rectArea.Height / 2);
        }

        internal void Update(KeyboardState ks, int maxX, ref List<Attack> attacks)
        {
            SetVel(0, 0);
            if (ks.IsKeyDown(Keys.Left))       SetVel(6, Math.PI);
            else if (ks.IsKeyDown(Keys.Right)) SetVel(6, 0);

            if (cannonCD > 0) --cannonCD;
            if (torpedoCD > 0) --torpedoCD;

            if (cannonAmmo == 0)
            {
                --cannonRLT;
                if (cannonRLT == 0)
                {
                    cannonAmmo = cannonCap;
                    cannonRLT = cannonRL;
                }
            }
            if (torpedos == 0)
            {
                --torpedoRLT;
                if (torpedoRLT == 0)
                {
                    torpedos = torpedoCap;
                    torpedoRLT = torpedoRL;
                }
            }
            if (missiles == 0)
            {
                --missileRLT;
                if (missileRLT == 0)
                {
                    missiles = missileCap;
                    missileRLT = missileRL;
                }
            }

            if (ks.IsKeyDown(Keys.Z) && cannonCD == 0 && cannonAmmo > 0)
            {
                attacks.Add(new Gunfire(
                    new Point(rectArea.X1 + rectArea.Width / 2 - Gunfire.Texture.Width / 2, rectArea.Y1 - 5),
                    (float) Math.PI / 2,
                    4));
                cannonCD = cannonROF;
                --cannonAmmo;
            }

            if (ks.IsKeyDown(Keys.X) && torpedoCD == 0 && torpedos > 0)
            {
                attacks.Add(new PhotonTorpedo(
                    new Point(rectArea.X1 + rectArea.Width / 2 - PhotonTorpedo.Texture.Width / 2, rectArea.Y1 - 5),
                    (float) Math.PI / 2,
                    4));
                torpedoCD = torpedoROF;
                --torpedos;
            }

            if (ks.IsKeyDown(Keys.C) && missiles > 0)
            {
                attacks.Add(new PhotonTorpedo(
                    new Point(rectArea.X1 + rectArea.Width / 2 - PhotonTorpedo.Texture.Width / 2, rectArea.Y1 - 5),
                    (float) Math.PI / 2,
                    4));
                --missiles;
            }

            base.Update();

            Area.MoveTo(MathHelper.Clamp(rectArea.X1, 0, maxX - rectArea.Width), rectArea.Y1);
        }
    }
}
