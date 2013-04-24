using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.CustomControls
{
	class TilemapContainer : Container
	{
		public Texture2D tilemapImage;
		public TilemapContainer(Manager p_manager)
			: base(p_manager)
		{
			
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
			renderer.Draw(tilemapImage, rect, Color.White);
		}
	}
}
