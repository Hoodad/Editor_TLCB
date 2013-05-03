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

            /*
            // position
            Queue<DataNode> batch = new Queue<DataNode>();
            List<int> columnPerRowCounter = new List<int>(); // "global siblings"
            batch.Enqueue(m_start);
            do
            {
                DataNode currentRender = batch.Dequeue();
                int level = currentRender.m_level;
                // retrieve current object
                Transform obj = currentRender.m_visualRepr;
                Material objMaterial = obj.GetChild(0).renderer.material;
                currentRender.m_visualRepr.renderer.enabled = true;
                currentRender.m_visualRepr.GetChild(0).renderer.enabled = true;
                currentRender.m_visualRepr.GetChild(1).renderer.enabled = true;
                // if click
                if (obj == mouseHoverObj && mouseClick)
                    m_currentNode = currentRender;
                // "render"
                if (m_treestate)
                {
                    obj.position = Vector3.Lerp(obj.position, new Vector3(currentRender.m_siblingId * m_nodeWidth,
                                               currentRender.m_level * m_nodeHeight,
                                               0.5f),
                                               10.0f * Time.deltaTime);
                }
                else
                {
                    obj.position = Vector3.Lerp(obj.position, new Vector3(m_start.m_visualRepr.position.x,
                                               currentRender.m_level * m_nodeHeight,
                                               0.5f),
                                               10.0f * Time.deltaTime);
                }

                objMaterial.color = m_normalNodeColor;
                // draw line to parent
                if (currentRender.m_parent != null)
                {
                    LineRenderer line = obj.GetComponent<LineRenderer>();
                    setLine(line, new Vector3(0.0f, 0.0f, 0.1f), new Vector3(0.0f, 0.0f, 0.1f) + currentRender.m_parent.m_visualRepr.position - obj.transform.position);
                }

                // process children and add to batch
                if (columnPerRowCounter.Count <= level) columnPerRowCounter.Insert(level, 0);
                int localSibling = 0;
                currentRender.m_activeBranch = false;
                foreach (DataNode d in currentRender.m_children)
                {
                    // set as active branch if a child is activebranch or currentaction
                    if (d.m_activeBranch || m_currentNode == d)
                    {
                        currentRender.m_activeBranch = true;
                    }
                    //
                    batch.Enqueue(d);
                    if (d.m_parent != null)
                        d.m_siblingId = Mathf.Max(d.m_parent.m_siblingId + localSibling,
                                                    columnPerRowCounter[level]);  // for alignment purposes   
                    else
                        d.m_siblingId = columnPerRowCounter[level];
                    columnPerRowCounter[level] += d.m_siblingId - columnPerRowCounter[level] + 1;
                    localSibling++;
                }


                // visualize active branch
                if (currentRender.m_activeBranch)
                {
                    objMaterial.color = m_activebranchNodeColor;
                }
                else if (!m_treestate && m_currentNode != currentRender)
                {
                    currentRender.m_visualRepr.renderer.enabled = false;
                    currentRender.m_visualRepr.GetChild(0).renderer.enabled = false;
                    currentRender.m_visualRepr.GetChild(1).renderer.enabled = false;
                    //currentRender.m_visualRepr.position = new Vector3(m_start.m_visualRepr.position.x, m_start.m_visualRepr.position.y, currentRender.m_visualRepr.position.z + 1);
                }

            } while (batch.Count > 0);
            */

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
                // set starting position(render) for node
                m_nodes[nodeId].m_renderPos = currentNodeRef.m_renderPos;
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
