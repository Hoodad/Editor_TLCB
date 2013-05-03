using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.Actions.Interface;

namespace MapEditor_TLCB.UndoTree
{
    class UndoTree
    {
        public enum Mode
        {
            TREE, LIST
        };

        public int m_currentNodeId=-1;
        private LineRenderer m_lineRenderer;
        private GraphicsDevice m_gd;
        InvariableIndexList<ActionInterface> m_actions;
        InvariableIndexList<ActionNode> m_nodes;
        Mode m_mode;

        public UndoTree(GraphicsDevice p_gd)
        {
            m_gd = p_gd;
            m_nodes = new InvariableIndexList<ActionNode>();
            m_actions = new InvariableIndexList<ActionInterface>();
            m_lineRenderer = new LineRenderer(m_gd);
            m_currentNodeId = addAction(null);
        }

        public void update(float p_dt)
        {

        }

        // draw the tree
        public void draw(SpriteBatch p_spriteBatch,Vector2 p_offset)
        {
            Vector2 offset = p_offset;
            m_lineRenderer.Draw(p_spriteBatch, Vector2.Zero,-Vector2.UnitY, Color.CornflowerBlue, 2.0f);
            
        }

        // set render mode
        public void setMode(Mode p_mode)
        {
            m_mode = p_mode;
        }

        public Mode getMode()
        {
            return m_mode;
        }

        // have internal storage of actions for now for simplicity
        // may want to have just one storage for them later
        public int addAction(ActionInterface p_action)
        {
            int nodeId = -1;                
            // store action in list
            int actionId = m_actions.add(p_action);
            //
            if (m_currentNodeId >= 0)
            {
                ActionNode currentNodeRef = m_nodes[m_currentNodeId];
                // and add its id to tree list
                nodeId = m_nodes.add(new ActionNode(actionId, m_currentNodeId, currentNodeRef.m_level + 1));
                // then add new node index as child to parent
                currentNodeRef.m_children.Add(nodeId);
            }
            else // first node
            {
                nodeId = m_nodes.add(new ActionNode(actionId, m_currentNodeId, 0));
            }
            return nodeId;
        }

        // Step down to child
        public void redo()
        {
            ActionNode currentNodeRef = m_nodes[m_currentNodeId];
            if (currentNodeRef.m_children.Count > 0)
            {
                m_currentNodeId = currentNodeRef.m_children[0];
            }
        }

        // step up to parent
        public void undo()
        {
            ActionNode currentNodeRef = m_nodes[m_currentNodeId];
            if (currentNodeRef.m_parentId>=0)
            {
                m_currentNodeId = currentNodeRef.m_parentId;
            }
        }

        public void setCurrent(int p_id)
        {
            m_currentNodeId = p_id;
        }


    }
}
