using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MapEditor_TLCB.CustomControls;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
    class CurrentToolSystem : EntitySystem
    {
        Manager manager;
		Window currentToolWindow;

        GraphicsDevice m_device;
        ContentManager m_content;

        CurrentToolContainer m_container;

        public CurrentToolSystem(Manager p_manager, GraphicsDevice p_device, ContentManager p_content)
		{
			manager = p_manager;
            m_device = p_device;
            m_content = p_content;
		}
		public override void Initialize()
		{
			Viewport viewport = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewport();
			
			//Notification Bar
            currentToolWindow = new Window(manager);
            currentToolWindow.Init();
            currentToolWindow.Text = "Current Tool";
            currentToolWindow.Height = 100;
            currentToolWindow.Width = 100;
            currentToolWindow.Visible = true;
            currentToolWindow.Top = 10;
            currentToolWindow.Left = viewport.Width-currentToolWindow.Width;
            currentToolWindow.CloseButtonVisible = false;
            currentToolWindow.AutoScroll = false;
			currentToolWindow.Resizable = false;
            currentToolWindow.BorderVisible = false;
			currentToolWindow.Movable = false;
			currentToolWindow.Visible = false;
            manager.Add(currentToolWindow);

            m_container = new CurrentToolContainer(manager, currentToolWindow, m_content);
            m_container.Init();
            m_container.Width = currentToolWindow.Width;// tilemap.tilemapImage.Width;
            m_container.Height = currentToolWindow.Height;// tilemap.tilemapImage.Height;
            m_container.Parent = currentToolWindow;
            m_container.CanFocus = false;
            m_container.Click += new TomShane.Neoforce.Controls.EventHandler(OnWindowClickBehavior);

            //Initialize some hotkeys
            EventSystem ev = (EventSystem)world.SystemManager.GetSystem<EventSystem>()[0];
            EventData ev1 = new EventData(SetCurrentToolCB, Tool.ROAD_TOOL);
            EventData ev2 = new EventData(SetCurrentToolCB, Tool.PAINT_TOOL);
            EventData ev3 = new EventData(SetCurrentToolCB, Tool.ERASE_TOOL);
            ev.addEvent(ev1);
            ev.addEvent(ev2);
            ev.addEvent(ev3);
            ev.setHotKey(ev1, Microsoft.Xna.Framework.Input.Keys.D1);
            ev.setHotKey(ev2, Microsoft.Xna.Framework.Input.Keys.D2);
            ev.setHotKey(ev3, Microsoft.Xna.Framework.Input.Keys.D3);
		}
		public override void Process()
		{
		}
        public void SetCurrentToolCB(object p_tool)
        {
            SetCurrentTool((Tool)p_tool);
        }
        public void SetCurrentTool(Tool p_tool)
        {
            m_container.SetCurrentTool(p_tool);
            if (p_tool == Tool.PAINT_TOOL)
            {
                TilemapBarSystem tbs = (TilemapBarSystem)world.SystemManager.GetSystem<TilemapBarSystem>()[0];
                Texture2D tilemapTex = tbs.GetTilemapContainer().GetTilemapTexture();
                m_container.SetTilemapTexture(tilemapTex);
                //m_container.SetTilemapRectangle(tbs.GetTilemapContainer().GetTilemapSourceRectangle());
                m_container.SetCurrentDrawTileIndex(tbs.GetTilemapContainer().GetCurrentIndex());

                RadialMenuSystem rms = (RadialMenuSystem)(world.SystemManager.GetSystem<RadialMenuSystem>()[0]);
                rms.currentToolChanged(tbs.GetTilemapContainer().GetCurrentIndex());
            }
        }
        public void SetCurrentDrawToolIndex(IntPair p_index)
        {
            //Does it ever enter this? Yes through Radial menu
            m_container.SetCurrentDrawTileIndex(p_index);

            RadialMenuSystem rms = (RadialMenuSystem)(world.SystemManager.GetSystem<RadialMenuSystem>()[0]);
            rms.currentToolChanged(p_index);

        }
		public Tool GetCurrentTool()
		{
			return m_container.GetCurrentTool();
		}
        public IntPair GetCurrentDrawTileIndex()
        {
            return m_container.GetCurrentDrawTileIndex();
        }
		public Rectangle getTilemapIconRectangle()
		{
			return m_container.getTilemapIconRectangle();
		}
        public void OnWindowClickBehavior(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            NotificationBarSystem noteSys = (NotificationBarSystem)world.SystemManager.GetSystem<NotificationBarSystem>()[0];
            Notification n = new Notification("This window shows the current tool. Draw by left clicking on the canvas.", NotificationType.INFO);
            noteSys.AddNotification(n);
        }
    }
}
