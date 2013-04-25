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
    class CurrentToolContainer : Container
    {
        private Window m_parentWindow;

        Texture2D m_roadToolIcon;

        public CurrentToolContainer(Manager p_manager, Window p_parent, ContentManager p_content)
			: base(p_manager)
		{
            m_parentWindow = p_parent;

            m_roadToolIcon = p_content.Load<Texture2D>("RoadToolIcon");
		}
        
		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
            renderer.Draw(m_roadToolIcon, rect, Color.White);
		}

		protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			Refresh();
		}
        public void Update(float p_dt)
        {
        }
    }
}
