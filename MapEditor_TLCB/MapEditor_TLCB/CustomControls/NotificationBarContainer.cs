using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.CustomControls
{
    class NotificationBarContainer : Container
    {
		private Window m_parentWindow;

        private NotificationBar m_bar;

        public NotificationBarContainer(Manager p_manager, Window p_parent, NotificationBar p_bar)
			: base(p_manager)
		{
            m_bar = p_bar;
            m_parentWindow = p_parent;
            int top1 = m_parentWindow.ClientArea.AbsoluteTop;
            int top2 = m_parentWindow.ClientArea.OriginTop;
            int top3 = m_parentWindow.ClientArea.Top;
            int top4 = this.Top;
            int top5 = this.AbsoluteTop;
		}
        
		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
            m_bar.draw(renderer.SpriteBatch);
		}

		protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
		{
			Refresh();
		}
        public void Update(float p_dt)
        {
            m_bar.update(p_dt);
        }
    }
}
