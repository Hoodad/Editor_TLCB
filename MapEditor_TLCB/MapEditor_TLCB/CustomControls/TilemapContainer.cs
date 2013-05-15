using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB.CustomControls
{
    public struct IntPair
    {
        public int i1;
        public int i2;
    }

	class TilemapContainer : Container
	{
		public Texture2D tilemapImage = null;
		public Texture2D tileSelectorImage = null;
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
        private int markedIndex;

        private bool showAddToRadial = false;

        Vector2 downPos;
        Vector2 currentPos;

        SpriteFont m_font;

        Vector2 addToRadialTextPos;

        Texture2D whiteDot;

		public TilemapContainer(Manager p_manager, ContentManager p_content)
			: base(p_manager)
		{
			gridColor = Color.White;

            currentIndex = 0;

            m_font = p_content.Load<SpriteFont>("Arial10");
            whiteDot = p_content.Load<Texture2D>("white_dot");
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
            Rectangle selectRect = adjustToScroll();
			if (tilemapImage != null)
			{
                Color c = new Color(255, 255, 255, 255);
				renderer.Draw(tilemapImage, rect, c);
				renderer.Draw(tilemapImage, rect, Color.White);

                Rectangle source;
                source.X = 32 * curr.X;
                source.Width = currSize.X;
                source.Y = 32 * curr.Y;
                source.Height = currSize.Y;

                Rectangle highlight = adjustToScroll2();

                if (!showAddToRadial)
                {
                    c = new Color(150, 150, 255, 255);
                    renderer.Draw(tileSelectorImage, highlight, source, Color.Green);
                }
			}
            Rectangle markedRect;
            markedRect.X = 32 * marked.X;
            markedRect.Width = markedSize.X;
            markedRect.Y = 32 * marked.Y;
            markedRect.Height = markedSize.Y;
			renderer.Draw(gridImage, rect, gridColor);
			renderer.Draw(tileSelectorImage, selectRect, markedRect, Color.CornflowerBlue);


            if (showAddToRadial)
            {
                string s = "Add Selection to Radial Menu";
                Rectangle dest;
                dest.X = (int)addToRadialTextPos.X-2;
                dest.Y = (int)addToRadialTextPos.Y-2;
                dest.Width = (int)m_font.MeasureString(s).X+4;
                dest.Height = (int)m_font.MeasureString(s).Y+4;

                float mouseX = Mouse.GetState().X - windowParent.AbsoluteLeft;
                float mouseY = Mouse.GetState().Y - windowParent.AbsoluteTop;

                Microsoft.Xna.Framework.Color drawColor = Color.White;

                if (mouseX > dest.X && mouseY > dest.Y)
                    if (mouseX < dest.X + dest.Width && mouseY < dest.Y + dest.Height)
                        drawColor = Color.Green;

                renderer.Draw(whiteDot, dest, Color.Black);
                dest.X = (int)addToRadialTextPos.X ;
                dest.Y = (int)addToRadialTextPos.Y;
                dest.Width = (int)m_font.MeasureString(s).X;
                dest.Height = (int)m_font.MeasureString(s).Y;
                renderer.Draw(whiteDot, dest, drawColor);
                renderer.DrawString(m_font, s, (int)addToRadialTextPos.X, (int)addToRadialTextPos.Y, Color.Black);
            }
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
            ip.i1 = markedIndex;
            ip.i2 = markedIndex + (int)(markedSize.Y / 32 - 1) * 30 + (int)(markedSize.X / 32 - 1);
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
			selectorRect.X = 1;
			selectorRect.Y = 1;
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
            markedIndex = currentIndex;
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
        public void toggleAddToRadialText(Vector2 p_pos)
        {
            showAddToRadial = !showAddToRadial;
            addToRadialTextPos = p_pos;
        }
        public bool addToRadialTextVisible()
        {
            return showAddToRadial;
        }
        public bool pointInAddToRadialText(Vector2 p_pos)
        {
            string s = "Add Selection to Radial Menu";
            Rectangle dest;
            dest.X = (int)addToRadialTextPos.X-2;
            dest.Y = (int)addToRadialTextPos.Y-2;
            dest.Width = (int)m_font.MeasureString(s).X+4;
            dest.Height = (int)m_font.MeasureString(s).Y+4;

            if (p_pos.X > dest.X && p_pos.Y > dest.Y)
                if (p_pos.X < dest.X + dest.Width && p_pos.Y < dest.Y + dest.Height)
                    return true;
            return false;
        }

		protected override void OnMouseOut(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			highlightRect.X = 1;
			highlightRect.Y = 1;
		}
	}
}
