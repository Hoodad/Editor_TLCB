using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Common
{
    class LineRenderer
    {
        private RenderTarget2D m_lineTexture;

        public LineRenderer(GraphicsDevice p_gd)
        {
            m_lineTexture = new RenderTarget2D(p_gd, 1, 1);
            p_gd.SetRenderTarget(m_lineTexture);
            p_gd.Clear(Color.White);
            p_gd.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch p_sb, 
            Vector2 p_start, Vector2 p_end, 
            Color p_col, float p_lineThickness=1.0f)
        {
            Vector2 d = p_end - p_start;
            float len = d.Length();
            float rot = (float)Math.Atan2(d.Y,d.X);

            p_sb.Draw(m_lineTexture, p_start, null, p_col, rot,
                        Vector2.UnitY,
                        new Vector2(len, p_lineThickness),
                        SpriteEffects.None, 0);
        }

    }
}
