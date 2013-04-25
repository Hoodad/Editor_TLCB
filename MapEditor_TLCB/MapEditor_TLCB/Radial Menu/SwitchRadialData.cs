using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor_TLCB
{
    public class SwitchRadialData
    {
        public RadialMenuContext context;
        public int index;

        public SwitchRadialData(RadialMenuContext p_context, int p_index)
        {
            context = p_context;
            index = p_index;
        }
    }
}
