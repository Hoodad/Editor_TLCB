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

namespace MapEditor_TLCB.Systems
{
	class TilemapBarSystem : EntitySystem
	{
		Manager manager;
		Window tilemapWindow;

		TilemapContainer tilemap;

        ContentManager m_content;

		public TilemapBarSystem(Manager p_manager, ContentManager p_content)
		{
			manager = p_manager;
            m_content = p_content;
		}

		public override void Initialize()
		{
			ContentSystem contentSystem = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Viewport viewport = contentSystem.GetViewport();

			tilemapWindow = new Window(manager);
			tilemapWindow.Init();
			tilemapWindow.Text = "Tilemap";
			tilemapWindow.Height = 224;
			tilemapWindow.Width = 576;
			tilemapWindow.Visible = true;
			tilemapWindow.Top = viewport.Height - tilemapWindow.Height;
			tilemapWindow.Left = (int)((float)viewport.Width -tilemapWindow.Width);
			tilemapWindow.CloseButtonVisible = false;
			tilemapWindow.MaximumHeight = 960 + 54;
			tilemapWindow.MaximumWidth = 960;
			tilemapWindow.IconVisible = false;
            tilemapWindow.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);
			manager.Add(tilemapWindow);

            tilemap = new TilemapContainer(manager, m_content);
			tilemap.gridImage = contentSystem.LoadTexture("TileSheets/grid");
			tilemap.tileSelectorImage = contentSystem.LoadTexture("TileSelector_v3");
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
            //tilemap.va += new TomShane.Neoforce.Controls.MouseEventHandler(OnScroll);
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
                    CurrentToolSystem toolSys = (CurrentToolSystem)(world.SystemManager.GetSystem<CurrentToolSystem>()[0]);
                    toolSys.SetCurrentTool(CustomControls.Tool.PAINT_TOOL);
                    tilemap.setSelectorRect();
                }
                else
                {
                    if (tilemap.pointInAddToRadialText(new Vector2(ev.Position.X-tilemapWindow.AbsoluteLeft, ev.Position.Y-tilemapWindow.AbsoluteTop)))
                        tilemap.addSelectionToRadial();
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
            NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
            Notification n = new Notification("Select a tile from the tilemap to start drawing.", NotificationType.INFO);
            noteSys.AddNotification(n);
        }
        public TilemapContainer GetTilemapContainer()
        {
            return tilemap;
        }
	}
}
