using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.Actions.Interface;
using Microsoft.Xna.Framework.Content;

namespace MapEditor_TLCB.UndoTree
{
    class UndoTree
    {
        public enum Mode
        {
            TREE, LIST
        };

        //
        public int m_currentNodeId=-1;
        private int m_startNodeId=-1;
        private LineRenderer m_lineRenderer;
        private GraphicsDevice m_gd;
        InvariableIndexList<ActionInterface> m_actions;
        InvariableIndexList<ActionNode> m_nodes;
        Mode m_mode;

        private float m_nodeWidth = 32.0f;
        private float m_nodeHeight = 25.0f;
        private Vector2 m_renderOffset = new Vector2(30, 100);
        private Vector2 m_lineRenderOffset;

        // resources
        Texture2D m_nodeBoxTex;
        SpriteFont m_font;

        List<int> m_renderBatch = new List<int>();

        public UndoTree(GraphicsDevice p_gd, ContentManager p_content)
        {
            m_gd = p_gd;
            m_nodes = new InvariableIndexList<ActionNode>();
            m_actions = new InvariableIndexList<ActionInterface>();
            m_lineRenderer = new LineRenderer(m_gd);
            m_currentNodeId = addAction(null);
            m_startNodeId = m_currentNodeId;

            // TEST RUN:
            addAction(null);
            addAction(null);
            undo();
            addAction(null);
            addAction(null);
            addAction(null);
            undo();
            undo();
            addAction(null);
            addAction(null);
            // TEST RUN ^

            // create resources
            m_nodeBoxTex = p_content.Load<Texture2D>("textbox");
            m_font = p_content.Load<SpriteFont>("Arial10");
            m_lineRenderOffset = new Vector2(m_nodeWidth / 2, m_nodeHeight / 2);
        }

        public void update(float p_dt)
        {
            // position
            Queue<int> batch = new Queue<int>();
            m_renderBatch.Clear();
            List<int> columnPerRowCounter = new List<int>(); // "global siblings"
            batch.Enqueue(m_startNodeId);
            m_renderBatch.Add(m_startNodeId);
            do
            {
                ActionNode currentProcNode = m_nodes[batch.Dequeue()];
                int level = currentProcNode.m_level;
                // retrieve current object
                //          >>>>>>>>>>>>>>>>>>>>>   Transform obj = currentRender.m_visualRepr;
                //          >>>>>>>>>>>>>>>>>>>>>   Material objMaterial = obj.GetChild(0).renderer.material;
                //          >>>>>>>>>>>>>>>>>>>>>   currentRender.m_visualRepr.renderer.enabled = true;
                //          >>>>>>>>>>>>>>>>>>>>>   currentRender.m_visualRepr.GetChild(0).renderer.enabled = true;
                //          >>>>>>>>>>>>>>>>>>>>>   currentRender.m_visualRepr.GetChild(1).renderer.enabled = true;
                // if click
                // if (obj == mouseHoverObj && mouseClick)
                //     m_currentNode = currentRender;

                // Move nodes to correct position
                Vector2 goal;
                if (m_mode == Mode.TREE)
                {
                    goal = new Vector2(currentProcNode.m_siblingId * m_nodeWidth * 2,
                                                currentProcNode.m_level * m_nodeHeight * 2);
                }
                else
                {
                    goal = new Vector2(m_nodes[m_startNodeId].m_renderPos.X,
                                               currentProcNode.m_level * m_nodeHeight * 2);
                }
                currentProcNode.m_renderPos = Vector2.Lerp(currentProcNode.m_renderPos,
                                                         goal,
                                               10.0f * p_dt); //<-dt



                // objMaterial.color = m_normalNodeColor;


                // process children and add to batch
                if (columnPerRowCounter.Count <= level) columnPerRowCounter.Insert(level, 0);
                int localSibling = 0;
                currentProcNode.m_activeBranch = false;
                foreach (int childId in currentProcNode.m_children)
                {
                    ActionNode childNode = m_nodes[childId];
                    // set as active branch if a child is activebranch or currentaction
                    if (childNode.m_activeBranch || m_currentNodeId == childId)
                    {
                        currentProcNode.m_activeBranch = true;
                    }
                    //
                    batch.Enqueue(childId);
                    m_renderBatch.Add(childId);
                    if (childNode.m_parentId != -1)
                        childNode.m_siblingId = Math.Max(m_nodes[childNode.m_parentId].m_siblingId + localSibling,
                                                         columnPerRowCounter[level]);  // for alignment purposes   
                    else
                        childNode.m_siblingId = columnPerRowCounter[level];
                    columnPerRowCounter[level] += childNode.m_siblingId - columnPerRowCounter[level] + 1;
                    localSibling++;
                }


                // visualize active branch
                /*
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
                }*/
            } while (batch.Count > 0);

        }

        // draw the tree
        public void draw(SpriteBatch p_spriteBatch,Vector2 p_offset)
        {
            Vector2 offset = p_offset;

            // render all lines(render first so they're behind the boxes)
            foreach (int i in m_renderBatch)
            {
                if (i != -1 && i < m_nodes.getSize() && m_nodes[i] != null)
                {
                    ActionNode currentRender = m_nodes[i];
                    // draw line to parent
                    int parentId = currentRender.m_parentId;
                    if (parentId != -1 && parentId < m_nodes.getSize() && m_nodes[parentId] != null)
                    {
                        m_lineRenderer.Draw(p_spriteBatch, m_lineRenderOffset+m_renderOffset + currentRender.m_renderPos,
                            m_lineRenderOffset+m_renderOffset + m_nodes[parentId].m_renderPos, 
                            Color.CornflowerBlue, 2.0f);
                    }
                }
            }

            // render all boxes
            foreach(int i in m_renderBatch)
            {
                if (i != -1 && i < m_nodes.getSize() && m_nodes[i] != null)
                {
                    ActionNode currentRender = m_nodes[i];
                    drawNode(p_spriteBatch, currentRender.m_renderPos,new Color(0.0f,currentRender.m_level,0.0f) );
                }
            }

           

        }

        private void drawNode(SpriteBatch p_spriteBatch,Vector2 p_pos,Color p_tint)
        {
            Vector2 drwPos = p_pos + m_renderOffset;
            p_spriteBatch.Draw(m_nodeBoxTex, 
                new Rectangle((int)drwPos.X, (int)drwPos.Y, (int)m_nodeWidth, (int)m_nodeHeight), 
                p_tint);
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
            m_currentNodeId = nodeId;
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
