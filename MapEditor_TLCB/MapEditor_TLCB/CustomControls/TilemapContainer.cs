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
		public Texture2D tileSelectorImage;
		private Rectangle selectorRect;
		private Window windowParent;

		private int horizontalNumberTiles;
		private int verticalNumberTiles;

		public TilemapContainer(Manager p_manager, Texture2D p_tilemapImage, Texture2D p_tileSelector, Window p_parent)
			: base(p_manager)
		{
			tilemapImage = p_tilemapImage;
			tileSelectorImage = p_tileSelector;

			selectorRect = new Rectangle();
			selectorRect.Width = tileSelectorImage.Width;
			selectorRect.Height = tileSelectorImage.Height;
			selectorRect.X = 6;
			selectorRect.Y = 26;
			windowParent = p_parent;

			horizontalNumberTiles = tilemapImage.Width / 32;
			verticalNumberTiles = tilemapImage.Height / 32;
			
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
			renderer.Draw(tilemapImage, rect, Color.White);
			renderer.Draw(tileSelectorImage, selectorRect, Color.White);
		}

		protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			Point mousePos = new Point();
			mousePos.X = e.Position.X - windowParent.ScrollBarValue.Horizontal;
			mousePos.Y = e.Position.Y - windowParent.ScrollBarValue.Vertical;

			selectorRect.X = (mousePos.X / 32) * 32 + 6;
			selectorRect.Y = (mousePos.Y / 32) * 32 + 28;

			Refresh();
		}
	}
}
