using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.CustomControls
{
	class HalfTransparentOverlay: Control
	{
		public HalfTransparentOverlay(Manager p_manager)
			: base(p_manager)
		{
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer,
			Rectangle rect, GameTime gameTime)
		{
			renderer.Draw(BackGroundTexture, rect, new Color(0.5f, 0.5f, 0.5f, 0.6f));
		}

		public Texture2D BackGroundTexture;
	}
}
