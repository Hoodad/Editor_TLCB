using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MapEditor_TLCB.CustomControls
{
	class TilemapContainer : Container
	{
		public Texture2D tilemapImage;
		public Texture2D tileSelectorImage;
		private Rectangle selectorRect;
		private Window windowParent;

		private Point tileSize;

        private Point curr;

        private int currentIndex;

		public TilemapContainer(Manager p_manager, Texture2D p_tilemapImage, Texture2D p_tileSelector, Window p_parent, Point p_windowSize)
			: base(p_manager)
		{
			tilemapImage = p_tilemapImage;
			tileSelectorImage = p_tileSelector;
			windowParent = p_parent;
	

			// Calculate the tile size and see if the maximum window size is not able to contain the whole 
			// tilemap.
			float aspect = (float)(p_windowSize.X) / (float)(tilemapImage.Width);
			if (aspect > 1.0f)
				aspect = 1.0f;
			tileSize.X = (int)(32 * aspect);

			aspect = (float)(p_windowSize.Y) / (float)(tilemapImage.Height);
			if (aspect > 1.0f)
				aspect = 1.0f;
			tileSize.Y = (int)(32 * aspect);

			selectorRect = new Rectangle();
			selectorRect.X = 6;
			selectorRect.Y = 26;
			selectorRect.Width = tileSize.X;
			selectorRect.Height = tileSize.Y;

            currentIndex = 0;
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
			renderer.Draw(tilemapImage, rect, Color.White);
			renderer.Draw(tileSelectorImage, selectorRect, Color.White);
		}

		protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			//Debug.Print("Mouse " + e.Position.ToString());
			//Debug.Print( "Scrollvalue {X "+windowParent.ScrollBarValue.Horizontal + ", Y "+ windowParent.ScrollBarValue.Vertical+"}");


            curr.X = (e.Position.X + 0) / tileSize.X;
            curr.Y = (e.Position.Y + 0) / tileSize.Y;
            currentIndex = curr.Y * 30 + curr.X;


			selectorRect.X = ((e.Position.X+0) / tileSize.X);
			selectorRect.Y = ((e.Position.Y+0) / tileSize.Y);
            //curr.X = selectorRect.X;
            //curr.Y = selectorRect.Y;
			//Debug.Print( "Resulting Tile {X: "+selectorRect.X +" Y: "+ selectorRect.Y+"}");
			selectorRect.X *= tileSize.X;
			selectorRect.Y *= tileSize.Y;
			selectorRect.X += 6;	//Window thickness
			selectorRect.Y += 28;	// -||-
			selectorRect.X -= windowParent.ScrollBarValue.Horizontal;	//
			selectorRect.Y -= windowParent.ScrollBarValue.Vertical;		//

			Refresh();
		}
        public Texture2D GetTilemapTexture()
        {
            return tilemapImage;
        }
        public Rectangle GetTilemapSourceRectangle()
        {
            Rectangle rect;
            rect.X = 32 * curr.X;
            rect.Width = 32;
            rect.Y = 32 * curr.Y;
            rect.Height = 32;
            return rect;
        }
        public int GetCurrentIndex()
        {
            return currentIndex;
        }
	}
}
