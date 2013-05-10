using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.CustomControls
{
    class RadialWindow : Control
    {
        public RadialWindow(Manager p_manager)
            : base(p_manager)
        {
        }

        protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer,
            Rectangle rect, GameTime gameTime)
        {
            //Insert render of radial menu here.
        }
        public RadialMenuContext RadialContext { get; set; }
    }
}
