using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor_TLCB.UndoTree
{
    class UndoTree
    {
        public enum Mode
        {
            TREE, LIST
        };

        public int m_currentNode;
        private LineRenderer m_lineRenderer;
        private GraphicsDevice m_gd;
        InvariableIndexList<ActionNode> m_nodes;
        Mode m_mode;

        public UndoTree(GraphicsDevice p_gd)
        {
            m_gd = p_gd;
            m_lineRenderer = new LineRenderer(m_gd);
        }

        public void update(float p_dt)
        {

        }

        public void draw(SpriteBatch p_spriteBatch,Vector2 p_offset)
        {
            Vector2 offset = p_offset;
            m_lineRenderer.Draw(p_spriteBatch, Vector2.Zero,-Vector2.UnitY, Color.CornflowerBlue, 2.0f);
            
        }

        public void setMode(Mode p_mode)
        {
            m_mode = p_mode;
        }

    }
}
