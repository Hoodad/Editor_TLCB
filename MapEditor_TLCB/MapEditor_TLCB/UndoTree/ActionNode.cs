using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace MapEditor_TLCB.Actions
{
    [Serializable()]
    class ActionNode : ISerializable
    {
        public enum NodeType
        {
            NONE=0,
            PAINT,
            ROAD,
            CLEAR,
            ERASE,
        }

        private static string[] typeName =
        { "*",
          "Paint",
          "Road",
          "Clear",
          "Erase",
        };

        public ActionNode(List<int> p_actionIds, int p_parentId, int p_level)
        {
            m_actionIds = p_actionIds;
            m_level = p_level;
            m_parentId = p_parentId;
            m_children = new List<int>();
            m_renderPos = Vector2.Zero;
        }

        public ActionNode(int p_actionId, int p_parentId, int p_level)
        {
            m_actionIds = new List<int>();
            m_actionIds.Add(p_actionId);
            m_level = p_level;
            m_parentId = p_parentId;
            m_children = new List<int>();
            m_renderPos = Vector2.Zero;
        }


        public ActionNode(SerializationInfo info, StreamingContext ctxt)
		{
            m_actionIds = (List<int>)info.GetValue("ActionIds", typeof(List<int>));
            m_children = (List<int>)info.GetValue("ChildrenIds", typeof(List<int>));
            m_parentId = (int)info.GetValue("ParentId", typeof(int));
            m_level = (int)info.GetValue("Level", typeof(int));
            m_type = (NodeType)info.GetValue("Type", typeof(int));

            m_renderPos = Vector2.Zero;
		}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ActionIds", m_actionIds);
            info.AddValue("ChildrenIds", m_children);
            info.AddValue("ParentId", m_parentId);
            info.AddValue("Level", m_level);
            info.AddValue("Type", (int)m_type);
        }

        public string GetInfo()
        {
            return typeName[(int)m_type];
        }


        public List<int> m_actionIds; ///< action reference
        public List<int> m_children;
        public int m_parentId;
        public int m_level; ///< level this node has in tree
        public int m_siblingId = 0; ///< id amongst siblings
        //public bool m_activeBranch = false; ///< whether this node is part of the active action branch
        public int m_activeBranch = 2; // using int for now. 0=false, 1=evaluation, 2=true. make tick down each time not having active branch child
        // for representation
        public Vector2 m_renderPos;
        public NodeType m_type=NodeType.NONE; ///< fix this into indexed names
        //public float traversedflash = 0.0f;
    }
}
