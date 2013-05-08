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
    struct IntPair
    {
        public int i1;
        public int i2;
    }

	class TilemapContainer : Container
	{
		public Texture2D tilemapImage = null;
		public Texture2D tileSelectorImage;
		public Texture2D gridImage;
		public Color gridColor;
		private Rectangle selectorRect;
        private Rectangle highlightRect;
		public Window windowParent;

		private Point tileSize;

        private Point curr;
        private Point currSize;
        private Point marked;
        private Point markedSize;

        private int currentIndex;

        Vector2 downPos;
        Vector2 currentPos;

		public TilemapContainer(Manager p_manager)
			: base(p_manager)
		{
			gridColor = Color.White;

            currentIndex = 0;
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
            Rectangle selectRect = adjustToScroll();
			if (tilemapImage != null)
			{
				renderer.Draw(tilemapImage, rect, Color.Gray);
                Rectangle source;
                source.X = 32 * curr.X;
                source.Width = currSize.X;
                source.Y = 32 * curr.Y;
                source.Height = currSize.Y;

                Rectangle highlight = adjustToScroll2();
                renderer.Draw(tilemapImage, highlight, source, Color.White);
			}
			renderer.Draw(gridImage, rect, gridColor);
            renderer.Draw(tileSelectorImage, selectRect, Color.White);
		}

		/*protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
            curr.X = (e.Position.X + 0) / tileSize.X;
            curr.Y = (e.Position.Y + 0) / tileSize.Y;
            currentIndex = curr.Y * 30 + curr.X;

			//Debug.Print("Mouse " + e.Position.ToString());
			//Debug.Print( "Scrollvalue {X "+windowParent.ScrollBarValue.Horizontal + ", Y "+ windowParent.ScrollBarValue.Vertical+"}");
            if (e.State.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                selectorRect.X = (((int)downPos.X + 0) / tileSize.X);
                selectorRect.Y = (((int)downPos.Y + 0) / tileSize.Y);
                currentPos = new Vector2(e.Position.X, e.Position.Y);

                int sizeX = (int)((currentPos.X - downPos.X) / tileSize.X) + 1;
                int sizeY = (int)((currentPos.Y - downPos.Y) / tileSize.Y) + 1;
                selectorRect.Width = sizeX * tileSize.X;
                selectorRect.Height = sizeY * tileSize.Y;
            }
            else
            {
                downPos = currentPos;
                selectorRect.X = ((e.Position.X + 0) / tileSize.X);
                selectorRect.Y = ((e.Position.Y + 0) / tileSize.Y);
                selectorRect.Width = tileSize.X;
                selectorRect.Height = tileSize.Y;
            }

			//selectorRect.X = ((e.Position.X+0) / tileSize.X);
			//selectorRect.Y = ((e.Position.Y+0) / tileSize.Y);


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
		}*/
        protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            currentPos = new Vector2(e.Position.X, e.Position.Y);
            if (e.State.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                downPos = currentPos;
                curr.X = (e.Position.X + 0) / tileSize.X;
                curr.Y = (e.Position.Y + 0) / tileSize.Y;
                currentIndex = curr.Y * 30 + curr.X;
            }
            currSize = new Point(32 * (int)((currentPos.X - downPos.X) / tileSize.X + 1), 32 * (int)((currentPos.Y - downPos.Y) / tileSize.Y + 1));

            highlightRect.X = ((int)(downPos.X + 0) / tileSize.X);
            highlightRect.Y = ((int)(downPos.Y + 0) / tileSize.Y);
            highlightRect.Width = tileSize.X * (int)((currentPos.X - downPos.X) / tileSize.X + 1);
            highlightRect.Height = tileSize.Y * (int)((currentPos.Y - downPos.Y) / tileSize.Y + 1);
            highlightRect.X *= tileSize.X;
            highlightRect.Y *= tileSize.Y;
            highlightRect.X += 6;
            highlightRect.Y += 28;
            //highlightRect.X -= windowParent.ScrollBarValue.Horizontal;
            //highlightRect.Y -= windowParent.ScrollBarValue.Vertical;

            Refresh();
        }
        public Texture2D GetTilemapTexture()
        {
            return tilemapImage;
        }
        public Rectangle GetTilemapSourceRectangle()
        {
            Rectangle rect;
            rect.X = 32 * marked.X;
            rect.Width = markedSize.X;
            rect.Y = 32 * marked.Y;
            rect.Height = markedSize.Y;
            return rect;
        }
        public IntPair GetCurrentIndex()
        {
            IntPair ip;
            ip.i1 = currentIndex;
            ip.i2 = currentIndex + (int)(currSize.Y / 32 - 1) * 30 + (int)(currSize.X / 32 - 1);
            return ip;
        }

		public void Init(Point p_windowSize)
		{
			base.Init();
			// Calculate the tile size and see if the maximum window size is not able to contain the whole 
			// tilemap.
			float aspect = (float)(p_windowSize.X) / (float)(960);
			if (aspect > 1.0f)
				aspect = 1.0f;
			tileSize.X = (int)(32 * aspect);

			aspect = (float)(p_windowSize.Y) / (float)(960);
			if (aspect > 1.0f)
				aspect = 1.0f;
			tileSize.Y = (int)(32 * aspect);

			selectorRect = new Rectangle();
			selectorRect.X = 6;
			selectorRect.Y = 26;
			selectorRect.Width = tileSize.X;
			selectorRect.Height = tileSize.Y;
		}
        public void setDownPos(Vector2 p_downPos)
        {
            downPos = new Vector2(tileSize.X * (int)(p_downPos.X / tileSize.X), tileSize.Y * (int)(p_downPos.Y / tileSize.Y));

            curr.X = (int)((p_downPos.X + 0) / tileSize.X);
            curr.Y = (int)((p_downPos.Y + 0) / tileSize.Y);
            currentIndex = curr.Y * 30 + curr.X;
        }
        public void setSelectorRect()
        {
            selectorRect.X = ((int)(downPos.X + 0) / tileSize.X);
            selectorRect.Y = ((int)(downPos.Y + 0) / tileSize.Y);
            selectorRect.Width = tileSize.X * (int)((currentPos.X - downPos.X) / tileSize.X + 1);
            selectorRect.Height = tileSize.Y * (int)((currentPos.Y - downPos.Y) / tileSize.Y + 1);
            selectorRect.X *= tileSize.X;
            selectorRect.Y *= tileSize.Y;
            selectorRect.X += 6;
            selectorRect.Y += 28;
            downPos = currentPos;
            marked = curr;
            markedSize = currSize;
        }
        public Rectangle adjustToScroll()
        {
            Rectangle rect = selectorRect;
            rect.X -= windowParent.ScrollBarValue.Horizontal;
            rect.Y -= windowParent.ScrollBarValue.Vertical;
            return rect;
        }
        public Rectangle adjustToScroll2()
        {
            Rectangle rect = highlightRect;
            rect.X -= windowParent.ScrollBarValue.Horizontal;
            rect.Y -= windowParent.ScrollBarValue.Vertical;
            return rect;
        }
	}
}
