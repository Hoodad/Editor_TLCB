using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.CustomControls
{
	class CanvasWindow: Window
	{
		public CanvasWindow(Manager p_manager)
			: base(p_manager)
		{
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer,
			Rectangle rect, GameTime gameTime)
		{
			renderer.Draw(CanvasTexture, rect, Color.White);
		}

		public RenderTarget2D CanvasTexture {get;set;}

	}
}
