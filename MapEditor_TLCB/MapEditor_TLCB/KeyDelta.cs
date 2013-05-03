using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB
{
    class KeyDelta
    {
        static List<float> DeltaList;
        static List<bool> DownList;
        public static void initialize()
        {
            DeltaList = new List<float>();
            DownList = new List<bool>();
            for (int i = 0; i < Enum.GetNames(typeof(Keys)).Length; i++)
            {
                DeltaList.Add(0.0f);
                DownList.Add(Keyboard.GetState().IsKeyDown((Keys)i));
            }
        }
        public static void update()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Keys)).Length; i++)
            {
                bool down = Keyboard.GetState().IsKeyDown((Keys)i);
                if (down && !DownList[i])
                    DeltaList[i] = 1.0f;
                else if (!down && DownList[i])
                    DeltaList[i] = -1.0f;
                else
                    DeltaList[i] = 0.0f;
                DownList[i] = down;
            }
        }
        public static float getDelta(Keys p_key)
        {
            return DeltaList[(int)p_key];
        }
    }
}
