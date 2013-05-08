using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Actions
{
    class ActionNode
    {
        public ActionNode(int p_actionId, int p_parentId, int p_level)
        {
            m_actionId = p_actionId;
            m_level = p_level;
            m_parentId = p_parentId;
            m_children = new List<int>();
            m_renderPos = Vector2.Zero;
        }
        public int m_actionId; ///< action reference
        public List<int> m_children;
        public int m_parentId;
        public int m_level; ///< level this node has in tree
        public int m_siblingId = 0; ///< id amongst siblings
        //public bool m_activeBranch = false; ///< whether this node is part of the active action branch
        public int m_activeBranch = 2; // using int for now. 0=false, 1=evaluation, 2=true. make tick down each time not having active branch child
        // for representation
        public Vector2 m_renderPos;
    }
}
