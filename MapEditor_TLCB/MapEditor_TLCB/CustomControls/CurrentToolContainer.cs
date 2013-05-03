using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MapEditor_TLCB.CustomControls
{
    enum Tool
    {
        ROAD_TOOL, PAINT_TOOL, ERASE_TOOL, NO_TOOL
    }

    class CurrentToolContainer : Container
    {
        private Window m_parentWindow;

        Tool m_currentTool;

        Texture2D m_roadToolIcon;
        Texture2D m_eraserToolIcon;
        Texture2D m_paintToolIcon;

        Texture2D m_tileMapIcon;
        Rectangle m_tileMapIconRectangle;

        int m_currentDrawTileIndex;

        public CurrentToolContainer(Manager p_manager, Window p_parent, ContentManager p_content)
			: base(p_manager)
		{
            m_parentWindow = p_parent;

            m_roadToolIcon = p_content.Load<Texture2D>("RoadToolIcon");
            m_eraserToolIcon = p_content.Load<Texture2D>("Eraser");
            m_paintToolIcon = p_content.Load<Texture2D>("paintTool");

            m_currentTool = Tool.ROAD_TOOL;

            m_currentDrawTileIndex = 0;
		}
        
		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
            if (m_currentTool == Tool.ROAD_TOOL)
                renderer.Draw(m_roadToolIcon, rect, Color.White);
            else if (m_currentTool == Tool.ERASE_TOOL)
            {
                renderer.Draw(m_eraserToolIcon, rect, Color.White);
            }
            else if (m_currentTool == Tool.PAINT_TOOL)
            {
                renderer.Draw(m_paintToolIcon, rect, Color.White);

                Rectangle rect2 = rect;
                rect2.Width = (int)(rect2.Width*0.5f);
                rect2.Height = (int)(rect2.Height * 0.5f);

                renderer.SpriteBatch.Draw(m_tileMapIcon, rect2, m_tileMapIconRectangle, Color.White);
            }
            else
            {
            }
		}

		protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			Refresh();
		}
        public void Update(float p_dt)
        {
        }
        public void SetCurrentTool(Tool p_tool)
        {
            m_currentTool = p_tool;
            Refresh();
        }
		public Tool GetCurrentTool()
		{
			return m_currentTool;
		}
        public void SetTilemapTexture(Texture2D p_texture)
        {
            m_tileMapIcon = p_texture;
        }
        /*public void SetTilemapRectangle(Rectangle p_rectangle)
        {
            m_tileMapIconRectangle = p_rectangle;
        }*/
        public void SetCurrentDrawTileIndex(int p_index)
        {
            Vector2 curr = new Vector2(p_index - 30 * (p_index / 30), p_index / 30);
            m_currentDrawTileIndex = p_index;
            m_tileMapIconRectangle.X = (int)(32 * curr.X);
            m_tileMapIconRectangle.Width = 32;
            m_tileMapIconRectangle.Y = (int)(32 * curr.Y);
            m_tileMapIconRectangle.Height = 32;
        }
        public int GetCurrentDrawTileIndex()
        {
            return m_currentDrawTileIndex;
        }
    }
}
