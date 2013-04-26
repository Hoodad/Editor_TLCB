﻿using System;
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

        public CurrentToolContainer(Manager p_manager, Window p_parent, ContentManager p_content)
			: base(p_manager)
		{
            m_parentWindow = p_parent;

            m_roadToolIcon = p_content.Load<Texture2D>("RoadToolIcon");
            m_eraserToolIcon = p_content.Load<Texture2D>("Eraser");
            m_paintToolIcon = p_content.Load<Texture2D>("paintTool");

            m_currentTool = Tool.ROAD_TOOL;
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
    }
}