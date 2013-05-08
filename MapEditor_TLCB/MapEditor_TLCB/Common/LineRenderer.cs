using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MapEditor_TLCB.Common
{
    class LineRenderer
    {
        private RenderTarget2D m_lineTexture;
        private Texture2D m_aalineTexture;

        public LineRenderer(GraphicsDevice p_gd, ContentManager p_content)
        {
            m_lineTexture = new RenderTarget2D(p_gd, 1, 1);
            p_gd.SetRenderTarget(m_lineTexture);
            p_gd.Clear(Color.White);
            p_gd.SetRenderTarget(null);
            m_aalineTexture = p_content.Load<Texture2D>("aaline");
        }

        public void Draw(SpriteBatch p_sb, 
            Vector2 p_start, Vector2 p_end, 
            Color p_col, float p_lineThickness=1.0f,bool p_antialias=false)
        {
            Vector2 d = p_end - p_start;
            float len = d.Length();
            float rot = (float)Math.Atan2(d.Y,d.X);


            if (p_antialias)
            {
                Vector2 dn = Vector2.Normalize(d);
                p_sb.Draw(m_aalineTexture, p_start, null, p_col, rot,
                        Vector2.UnitY,
                        new Vector2(len+0.01f, p_lineThickness),
                        SpriteEffects.None, 0);
            }
            else
                p_sb.Draw(m_lineTexture, p_start, null, p_col, rot,
                        Vector2.UnitY,
                        new Vector2(len, p_lineThickness),
                        SpriteEffects.None, 0);
        }

    }
}
