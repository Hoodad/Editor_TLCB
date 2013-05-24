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

        public IntPair(int p_i1, int p_i2)
        {
            i1 = p_i1;
            i2 = p_i2;
        }
	}

	class TilemapContainer : Container
	{
		private enum RectanglePoint{TOPLEFT, BOTTOMRIGHT};
		
		public Texture2D tilemapImage = null;
		public Texture2D tileSelectorImage = null;
		public Texture2D gridImage;
		public Color gridColor;
		private Rectangle selectorRect;
		private Rectangle highlightRect;
		public Window windowParent;

		private Point tileSize;
		private bool showAddToRadial = false;

		Vector2 startingPos;
		Vector2 currentPos;

		SpriteFont m_font;

		Vector2 addToRadialTextPos;

		Texture2D whiteDot;

		public TilemapContainer(Manager p_manager, ContentManager p_content)
			: base(p_manager)
		{
			gridColor = Color.White;

			m_font = p_content.Load<SpriteFont>("Arial10");
			whiteDot = p_content.Load<Texture2D>("white_dot");
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
			selectorRect.X = 0;
			selectorRect.Y = 0;
			selectorRect.Width = 0;
			selectorRect.Height = 0;
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
			if (tilemapImage != null)
			{
				Color c = new Color(255, 255, 255, 255);
				renderer.Draw(tilemapImage, rect, c);
				renderer.Draw(tilemapImage, rect, Color.White);

				Rectangle source;
				source.X = 0;
				source.Y = 0;
				source.Width = 1;
				source.Height = 1;

				Rectangle highlightOffset = adjustToScroll(highlightRect);

				if (!showAddToRadial)
				{
					c = new Color(150, 150, 255, 255);
					renderer.Draw(tileSelectorImage, highlightOffset, source, Color.Green);
				}
			}
			renderer.Draw(gridImage, rect, gridColor);

			Rectangle selectRect = adjustToScroll(selectorRect);
			Rectangle markedRect;
			markedRect.X = 0;
			markedRect.Y = 0;
			markedRect.Width = 1;
			markedRect.Height = 1;
			renderer.Draw(tileSelectorImage, selectRect, markedRect, Color.CornflowerBlue);


			if (showAddToRadial)
			{
				string s = "Add Selection to Radial Menu";
				Rectangle dest;
				dest.X = (int)addToRadialTextPos.X - 2;
				dest.Y = (int)addToRadialTextPos.Y - 2;
				dest.Width = (int)m_font.MeasureString(s).X + 4;
				dest.Height = (int)m_font.MeasureString(s).Y + 4;

				float mouseX = Mouse.GetState().X - windowParent.AbsoluteLeft;
				float mouseY = Mouse.GetState().Y - windowParent.AbsoluteTop;

				Microsoft.Xna.Framework.Color drawColor = Color.White;

				if (mouseX > dest.X && mouseY > dest.Y)
					if (mouseX < dest.X + dest.Width && mouseY < dest.Y + dest.Height)
						drawColor = Color.Green;

				renderer.Draw(whiteDot, dest, Color.Black);
				dest.X = (int)addToRadialTextPos.X;
				dest.Y = (int)addToRadialTextPos.Y;
				dest.Width = (int)m_font.MeasureString(s).X;
				dest.Height = (int)m_font.MeasureString(s).Y;
				renderer.Draw(whiteDot, dest, drawColor);
				renderer.DrawString(m_font, s, (int)addToRadialTextPos.X, (int)addToRadialTextPos.Y, Color.Black);
			}
		}

		public Texture2D GetTilemapTexture()
		{
			return tilemapImage;
		}

		public void setDownPos(Vector2 p_downPos)
		{
			startingPos.X = tileSize.X * (int)((p_downPos.X) / tileSize.X);
			startingPos.Y = tileSize.Y * (int)((p_downPos.Y) / tileSize.Y);
		}
		public IntPair GetCurrentIndex()
		{
			int tileIndex = (selectorRect.Y-28)/tileSize.Y * 30 + (selectorRect.X-6)/tileSize.X;
			IntPair pair;
			pair.i1 = tileIndex;
			pair.i2 = tileIndex + (int)(selectorRect.Height / tileSize.Y - 1) * 30 + (int)(selectorRect.Width / tileSize.X - 1);

			return pair;
		}
		public void setSelectorRect()
		{
			Point topLeft; // Get the top left position from current position and startingPosition
			topLeft = GetDesiredPoint(currentPos, startingPos, RectanglePoint.TOPLEFT);
			//topLeft.Y = Math.Min(currentPos.Y, startingPos.Y);

			Point bottomRight; // Get the bottom right position from current position and startingPosition
			bottomRight = GetDesiredPoint(currentPos, startingPos, RectanglePoint.BOTTOMRIGHT);

			//This needs to be checked if the starting tile is to be included
			if (bottomRight.X == startingPos.X)
			{
				bottomRight.X += tileSize.X;
			}
			if (bottomRight.Y == startingPos.Y)
			{
				bottomRight.Y += tileSize.Y;
			}

			selectorRect = CalculateRect(topLeft, bottomRight);
			selectorRect.X += 6;
			selectorRect.Y += 28;
			startingPos = currentPos;
		}

		private Rectangle CalculateRect(Point p_topLeft, Point p_bottomRight)
		{
			Rectangle newRect = new Rectangle();

			newRect.X = GetIntPair(p_topLeft).i1 * tileSize.X;
			newRect.Y = GetIntPair(p_topLeft).i2 * tileSize.Y;
			newRect.Width = (int)(tileSize.X * ((int)(p_bottomRight.X - p_topLeft.X) / tileSize.X + 1));
			newRect.Height = (int)(tileSize.Y * ((int)(p_bottomRight.Y - p_topLeft.Y) / tileSize.Y + 1));

			return newRect;
		}

		protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
            base.OnMouseMove(e);
			currentPos = new Vector2(e.Position.X, e.Position.Y);
			if (e.State.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
			{
				startingPos = currentPos;
			}

			/*HACK: UGLY AS FUCK HACK TO REMOVE GLITCHING STUPID STUFF*/
			if (currentPos.X % tileSize.X == 0)
				currentPos.X += 1;

			if (currentPos.Y % tileSize.Y == 0)
				currentPos.Y += 1;
			/*-END-*/

			Point topLeft = GetDesiredPoint(currentPos, startingPos, RectanglePoint.TOPLEFT);
			Point bottomRight = GetDesiredPoint(currentPos, startingPos, RectanglePoint.BOTTOMRIGHT);

			//This needs to be checked if the starting tile is to be included
			if (e.State.LeftButton == ButtonState.Pressed)
			{
				if (bottomRight.X == startingPos.X)
				{
					bottomRight.X += tileSize.X;
				}
				if (bottomRight.Y == startingPos.Y)
				{
					bottomRight.Y += tileSize.Y;
				}
			}

			highlightRect = CalculateRect(topLeft, bottomRight);

			String debugText = "";
			debugText += "High (" + highlightRect.X + ", " + highlightRect.Y + ")";
			debugText += "(" + (highlightRect.Width) + ", " + (highlightRect.Height) + ")";

			//Debug.Print(debugText);

			highlightRect.X += 6;
			highlightRect.Y += 28;

			Refresh(); //Request to render it again
		}

		private IntPair GetIntPair(Point p_position)
		{
			IntPair pair;
			pair.i1 = (int)(p_position.X / tileSize.X);
			pair.i2 = (int)(p_position.Y / tileSize.Y);

			return pair;
		}
		public Rectangle adjustToScroll(Rectangle p_rect)
		{
			Rectangle rect = p_rect;
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
			dest.X = (int)addToRadialTextPos.X - 2;
			dest.Y = (int)addToRadialTextPos.Y - 2;
			dest.Width = (int)m_font.MeasureString(s).X + 4;
			dest.Height = (int)m_font.MeasureString(s).Y + 4;

			if (p_pos.X > dest.X && p_pos.Y > dest.Y)
				if (p_pos.X < dest.X + dest.Width && p_pos.Y < dest.Y + dest.Height)
					return true;
			return false;
		}

		protected override void OnMouseOut(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			highlightRect.X = 0;
			highlightRect.Y = 0;
			highlightRect.Width = 0;
			highlightRect.Height = 0;
		}

		private Point GetDesiredPoint(Vector2 p_redPosition, Vector2 p_bluePosition, RectanglePoint p_type)
		{
			Point newPoint = new Point();

			if (p_type == RectanglePoint.BOTTOMRIGHT)
			{
				newPoint.X = (int)Math.Max(p_redPosition.X, p_bluePosition.X);
				newPoint.Y = (int)Math.Max(p_redPosition.Y, p_bluePosition.Y);
			}
			else if (p_type == RectanglePoint.TOPLEFT)
			{
				newPoint.X = (int)Math.Min(p_redPosition.X, p_bluePosition.X);
				newPoint.Y = (int)Math.Min(p_redPosition.Y, p_bluePosition.Y);
			}

			return newPoint;
		}
	}
}
