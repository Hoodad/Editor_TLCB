using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor_TLCB.UndoTree
{
    class ActionNode
    {
        public ActionNode(int p_actionId, int p_parentId, int p_level)
        {
            m_actionId = p_actionId;
            m_level = p_level;
            m_parentId = p_parentId;
            m_children = new List<int>();
        }
        public int m_actionId; ///< action reference
        public List<int> m_children;
        public int m_parentId;
        public int m_level; ///< level this node has in tree
        public int m_siblingId = 0; ///< id amongst siblings
        public bool m_activeBranch = false; ///< whether this node is part of the active action branch
    }
}
