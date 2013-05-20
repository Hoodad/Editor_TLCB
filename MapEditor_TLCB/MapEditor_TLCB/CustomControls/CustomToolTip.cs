using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MapEditor_TLCB.CustomControls
{
	class CustomToolTip: ToolTip
	{
		public CustomToolTip(Manager manager)
			:base(manager)
		{
		}

		protected override void DrawControl(Renderer renderer,
			Rectangle rect, GameTime gameTime)
		{
			this.Left = this.Left - this.Width; // Eeeeeeeeh?
			this.Top = this.Top - this.Height; // Eeeeeeeeh!
			// As with the panning in TilemapBar I have no idea of what happens in Neoforce...
			// ...but hey, at least I made it work!

			base.DrawControl(renderer, rect, gameTime);
		}
	}
}