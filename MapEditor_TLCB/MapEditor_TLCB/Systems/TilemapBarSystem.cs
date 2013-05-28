using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MapEditor_TLCB.CustomControls;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace MapEditor_TLCB.Systems
{
	class TilemapBarSystem : EntitySystem
	{
		Manager manager;
		Window tilemapWindow;

		TilemapContainer tilemap;
        Point startingPosition;
        Point startingScrollLocation;
        Point currentPosition;
        bool panningEnable = false;
        bool spacePressed = false;
        bool m_haveShownTilemapMessage = false;
		bool haveShownTilemapBar = false;
		bool windowIsLarge = false;
		bool enableWindowAutoSize = false;

        ContentManager m_content;

		public TilemapBarSystem(Manager p_manager, ContentManager p_content, bool p_enableWindowAutoSize = false)
		{
			manager = p_manager;
            m_content = p_content;
			enableWindowAutoSize = p_enableWindowAutoSize;
		}

		public override void Initialize()
		{
			ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();

			tilemapWindow = new Window(manager);
			tilemapWindow.Init();
			tilemapWindow.Text = "Tilemap";
			if (enableWindowAutoSize)
			{
				tilemapWindow.Resizable = false;
				tilemapWindow.Movable = false;
				tilemapWindow.Height = 160;
				tilemapWindow.MouseOver += new TomShane.Neoforce.Controls.MouseEventHandler(OnMouseOverBehavior);
				tilemapWindow.MouseOut += new TomShane.Neoforce.Controls.MouseEventHandler(OnMouseOutBehavior);
			}
			else
			{
				tilemapWindow.Height = 196;
			}
			tilemapWindow.Width = 576;
			tilemapWindow.Visible = true;
			tilemapWindow.Top = viewport.Height - tilemapWindow.Height;
			tilemapWindow.Left = (int)((float)viewport.Width -tilemapWindow.Width);
			tilemapWindow.CloseButtonVisible = false;
			tilemapWindow.MaximumHeight = 960 + 54;
			tilemapWindow.MaximumWidth = 960 + 32;
			tilemapWindow.IconVisible = false;
            tilemapWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			manager.Add(tilemapWindow);

            tilemap = new TilemapContainer(manager, m_content);
			tilemap.gridImage = contentSystem.LoadTexture("TileSheets/grid");
			tilemap.tileSelectorImage = contentSystem.LoadTexture("TileSheets/tileSelector");
			tilemap.Parent = tilemapWindow;
			tilemap.windowParent = tilemapWindow;
			tilemap.Width = 960;
			tilemap.Height = 960;
			tilemap.Parent = tilemapWindow;
			tilemap.gridColor = Color.Black;
			tilemap.CanFocus = false;
			tilemap.Click += new TomShane.Neoforce.Controls.EventHandler(OnClick);
            tilemap.DoubleClicks = false;
            tilemap.MouseDown += new TomShane.Neoforce.Controls.MouseEventHandler(OnMouseDown);
            tilemap.MouseDown += new MouseEventHandler(PanningMouseDownBehavior);
            tilemap.MouseMove += new MouseEventHandler(PanningMouseMoveBehavior);
            tilemap.MouseUp += new MouseEventHandler(PanningMouseUpBehavior);
			if (enableWindowAutoSize)
			{
				tilemap.MouseOver += new TomShane.Neoforce.Controls.MouseEventHandler(OnMouseOverBehavior);
				tilemap.MouseOut += new TomShane.Neoforce.Controls.MouseEventHandler(OnMouseOutBehavior);
			}
			tilemap.Init(contentSystem.GetViewportSize());

		}

		public override void Process()
		{
			StartupDialogSystem dialogSystem = (StartupDialogSystem)(world.SystemManager.GetSystem<StartupDialogSystem>()[0]);

			if (dialogSystem.HasChangedTileMap())
			{
				tilemap.tilemapImage = dialogSystem.tilemap;
				tilemap.Refresh();
			}

            if (Keyboard.GetState(0).IsKeyDown(Keys.Space) && spacePressed == false)
            {
                spacePressed = true;
                startingPosition.X = -1;
                startingPosition.Y = -1;

                spacePressed = true;
                startingScrollLocation = new Point();
                startingScrollLocation.X = tilemapWindow.ScrollBarValue.Horizontal;
                startingScrollLocation.Y = tilemapWindow.ScrollBarValue.Vertical;
            }
            else if (Keyboard.GetState(0).IsKeyUp(Keys.Space) && spacePressed == true)
            {
                spacePressed = false;
                panningEnable = false;
            }

			
		}
        public void PanningMouseDownBehavior(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            if (e.State.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && panningEnable == false)
            {
                startingPosition = e.Position;

                startingScrollLocation = new Point();
                startingScrollLocation.X = tilemapWindow.ScrollBarValue.Horizontal;
                startingScrollLocation.Y = tilemapWindow.ScrollBarValue.Vertical;

                panningEnable = true;
            }
        }
        public void PanningKeyDownBehavior(object sender, TomShane.Neoforce.Controls.KeyEventArgs e)
        {
            if (e.Key == Keys.Space && spacePressed == false && panningEnable == false)
            {
                startingPosition.X = -1;
                startingPosition.Y = -1;

                spacePressed = true;
                startingScrollLocation = new Point();
                startingScrollLocation.X = tilemapWindow.ScrollBarValue.Horizontal;
                startingScrollLocation.Y = tilemapWindow.ScrollBarValue.Vertical;

            }
        }
        void PanningKeyUpBehavior(object sender, TomShane.Neoforce.Controls.KeyEventArgs e){
            if (e.Key == Keys.Space)
            {
                spacePressed = false;
                panningEnable = false;
            }
        }
        public void PanningMouseMoveBehavior(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            if (spacePressed && panningEnable == false)
            {
                startingPosition.X = e.Position.X;
                startingPosition.Y = e.Position.Y;

                panningEnable = true;
            }
            if ( panningEnable )
            {
                currentPosition = e.Position;

                Point difference = new Point();
                difference.X = startingPosition.X - currentPosition.X;
                difference.Y = startingPosition.Y - currentPosition.Y;

                startingScrollLocation.X += difference.X;
                startingScrollLocation.Y += difference.Y;

				if (startingScrollLocation.X < 0)
					startingScrollLocation.X = 0;
				else if (startingScrollLocation.X > (tilemap.Width-tilemapWindow.Width + 32))
					startingScrollLocation.X = tilemap.Width - tilemapWindow.Width + 32;

				if (startingScrollLocation.Y < 0)
					startingScrollLocation.Y = 0;
				else if (startingScrollLocation.Y > (tilemap.Height-tilemapWindow.Height + 54))
					startingScrollLocation.Y = tilemap.Height - tilemapWindow.Height + 54;

				Debug.Print(startingScrollLocation.ToString());

                tilemapWindow.ScrollTo(startingScrollLocation.X, startingScrollLocation.Y);
            }
        }

        public void PanningMouseUpBehavior(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            panningEnable = false;
        }
        public void OnMouseDown(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (!tilemap.addToRadialTextVisible())
                {
                    CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
                    tilemap.setDownPos(new Vector2(e.Position.X, e.Position.Y));
                }
            }
        }
        public void OnScroll(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            tilemap.setDownPos(new Vector2(e.Position.X, e.Position.Y));
        }
        public void OnMouseMove(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
            toolSys.SetCurrentTool(CustomControls.Tool.PAINT_TOOL);
        }


		public void OnClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
            TomShane.Neoforce.Controls.MouseEventArgs ev = (TomShane.Neoforce.Controls.MouseEventArgs)(e);
            if (ev.Button == MouseButton.Left)
            {
                if (!tilemap.addToRadialTextVisible())
                {
                    tilemap.setSelectorRect();
                    CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
                    toolSys.SetCurrentTool(CustomControls.Tool.PAINT_TOOL);

                    if (!m_haveShownTilemapMessage)
                    {
                        NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];

                        List<Paragraph> paragraphs = new List<Paragraph>();
                        paragraphs.Add(new Paragraph("You use the tilemap to select tiles and place them on the canvas in the center of the screen."));
                        paragraphs.Add(new Paragraph("By clicking and dragging you may select multiple tiles at the same time."));
                        paragraphs.Add(new Paragraph("Some tiles may also be selected using the Radial Menu. You open the Radial Menu by pressing the right mouse button anywhere on the canvas in the middle of the screen. You can close it again by clicking the right mouse button or by clicking the close button in the middle of the menu. Try it out!"));
                        paragraphs.Add(new Paragraph("You can add tile selections to the Radial Menu. Select some tiles in the tilemap, right click and a pop-up text appears prompting you to add the selection to the Radial Menu."));

                        Notification note = new Notification("You just selected your first tile. Press more for more information.", NotificationType.INFO, paragraphs, "Tilemap");
                        noteSys.AddNotification(note);
                        m_haveShownTilemapMessage = true;
                    }
                }
                else
                {
                    if (tilemap.pointInAddToRadialText(new Vector2(ev.Position.X - tilemapWindow.AbsoluteLeft, ev.Position.Y - tilemapWindow.AbsoluteTop)))
                    {
                        RadialMenuSystem radial = (RadialMenuSystem)world.SystemManager.GetSystem<RadialMenuSystem>()[0];
                        radial.addCustomSelection(tilemap.GetCurrentIndex());
                    }
                    tilemap.toggleAddToRadialText(Vector2.Zero);
                }
            }
            else if (ev.Button == MouseButton.Right)
            {
                tilemap.toggleAddToRadialText(new Vector2(ev.Position.X-tilemapWindow.AbsoluteLeft, ev.Position.Y-tilemapWindow.AbsoluteTop));
            }
		}
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
			if (!haveShownTilemapBar)
			{
				NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
				Notification n = new Notification("Select a tile from the tilemap to start drawing.", NotificationType.INFO);
				noteSys.AddNotification(n);

				haveShownTilemapBar = true;
			}
        }
        public TilemapContainer GetTilemapContainer()
        {
            return tilemap;
        }

		private void OnMouseOverBehavior(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			Point position = e.Position;

			if (windowIsLarge == false)
			{
				if (position.X >= tilemapWindow.Left && position.X <= tilemapWindow.Left + tilemapWindow.Width)
				{
					if (position.Y >= tilemapWindow.Top && position.Y <= tilemapWindow.Top + tilemapWindow.Height)
					{
						tilemapWindow.Width = 960+32;
						tilemapWindow.Height = tilemapWindow.Height * 2;
						tilemapWindow.Left = manager.ScreenWidth - tilemapWindow.Width;
						tilemapWindow.Top = manager.ScreenHeight - tilemapWindow.Height;
						windowIsLarge = true;
					}
				}
			}
		}
		private void OnMouseOutBehavior(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			Point position = e.Position;
			Debug.Print(position.ToString());
			if (windowIsLarge == true)
			{
				if (position.X <= tilemapWindow.Left || position.X >= tilemapWindow.Left + tilemapWindow.Width 
					|| position.Y <= tilemapWindow.Top || position.Y >= tilemapWindow.Top + tilemapWindow.Height)
				{
					tilemapWindow.Width = 576;
					tilemapWindow.Height = tilemapWindow.Height / 2;
					tilemapWindow.Left = manager.ScreenWidth - tilemapWindow.Width;
					tilemapWindow.Top = manager.ScreenHeight - tilemapWindow.Height;
					windowIsLarge = false;
				}
			}
		}
	}
}
