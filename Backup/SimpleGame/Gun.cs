using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleGame
{
    class Gun
    {

        float bulletcooldown;
        int clipbulletsleft;
        int clipsize;
        int totalammo;

        public float bulletCooldown
        {
            get { return bulletcooldown; }
            set { bulletcooldown = value; }
        }

        public int clipBulletsLeft
        {
            get { return clipbulletsleft; }
            set { clipbulletsleft = value; }
        }

        public int clipSize
        {
            get { return clipsize; }
            set { clipsize = value; }
        }

        public int totalAmmo
        {
            get { return totalammo; }
            set { totalammo = value; }
        }

        public void Load()
        {
            bulletCooldown= 1.0f;
            clipBulletsLeft= 6;
            clipSize = 6;
            totalAmmo = 100;

        }
    }
}