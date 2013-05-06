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

        public enum Zoom
        {
            NORMAL, MINI
        };

        //
        public int m_currentNodeId=-1;
        private int m_startNodeId=-1;
        private LineRenderer m_lineRenderer;
        private GraphicsDevice m_gd;
        InvariableIndexList<ActionInterface> m_actions;
        InvariableIndexList<ActionNode> m_nodes;
        Mode m_mode;
        Zoom m_zoom;

        private float m_nodeOrigWidth = 120.0f;
        private float m_nodeOrigHeight = 25.0f;
        private float m_nodeMiniWidth = 10.0f;
        private float m_nodeMiniHeight = 10.0f;

        private float m_nodeWidth;
        private float m_nodeHeight;
        private Vector2 m_nodeMargin;
        private Vector2 scrollOffset = Vector2.Zero;
        private Vector2 scrollInputBuffer=Vector2.Zero;
        private Vector2 m_renderOffset = new Vector2(30, 100);
        private Vector2 m_lineRenderOffset;

        // stats
        int maxSiblings = 0;
        int maxLevel = 0;
        private Vector2 m_totalSize = Vector2.Zero;

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

            // modes
            m_mode = Mode.TREE;
            m_zoom = Zoom.NORMAL;
            m_nodeWidth=m_nodeOrigWidth;
            m_nodeHeight=m_nodeOrigHeight;

            // TEST RUN:
            addAction(new ChangeColor(Color.Red,null));
            addAction(null);
            for (int i = 0; i < 40; i++)
            {
                undo();
                addAction(null);
                addAction(null);
                addAction(null);
                if (i == 5)
                {
                    for (int x=0;x<100;x++)
                        addAction(null);
                }
                undo();
                undo();
                addAction(null);
                addAction(null);
            }
            // TEST RUN ^

            // create resources
            m_nodeBoxTex = p_content.Load<Texture2D>("textbox");
            m_font = p_content.Load<SpriteFont>("Arcadepix");
        }

        public void update(float p_dt)
        {
            // update sub sizes dependant on any changes
            m_lineRenderOffset = new Vector2(m_nodeWidth / 2, m_nodeHeight / 2);
            m_nodeMargin = new Vector2(m_nodeWidth * 1.4f, m_nodeHeight * 1.6f);
            // position
            Queue<int> batch = new Queue<int>();
            m_renderBatch.Clear();
            List<int> columnPerRowCounter = new List<int>(); // "global siblings"
            batch.Enqueue(m_startNodeId);
            m_renderBatch.Add(m_startNodeId);
            maxSiblings = 0; maxLevel = 0; // reset for count
            do
            {
                ActionNode currentProcNode = m_nodes[batch.Dequeue()];
                int level = currentProcNode.m_level;
                if (level > maxLevel) maxLevel = level;
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
                    goal = new Vector2(currentProcNode.m_siblingId * m_nodeMargin.X,
                                                currentProcNode.m_level * m_nodeMargin.Y);
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
                    // store maximum
                    if (localSibling > maxSiblings) maxSiblings = localSibling;
                    if (columnPerRowCounter[level] > maxSiblings) maxSiblings = columnPerRowCounter[level];
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
            // calculate width and height of whole tree in pixels
            m_totalSize = new Vector2((float)maxSiblings, (float)maxLevel) * m_nodeMargin;
            ScrollRefresh(p_dt);
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
                        m_lineRenderer.Draw(p_spriteBatch, scrollOffset+m_lineRenderOffset+m_renderOffset + currentRender.m_renderPos,
                            scrollOffset+m_lineRenderOffset+m_renderOffset + m_nodes[parentId].m_renderPos, 
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
                    ActionInterface action=null;
                    drawNode(p_spriteBatch, scrollOffset+m_renderOffset+currentRender.m_renderPos,Color.LightGreen );
                    // draw action info as well
                    if (m_zoom != Zoom.MINI)
                    {
                        if (currentRender.m_actionId != -1) action = m_actions[currentRender.m_actionId];
                        if (action != null)
                        {
                            string info = action.GetInfo();
                            Vector2 strSz = m_font.MeasureString(info);
                            float scale = Math.Min(1.0f, m_nodeWidth / strSz.X);
                            strSz *= scale;
                            p_spriteBatch.DrawString(m_font, info, scrollOffset + m_renderOffset + currentRender.m_renderPos, Color.White,
                                0, new Vector2(-m_nodeWidth + strSz.X, -m_nodeHeight + strSz.Y) * 0.5f,
                                scale, SpriteEffects.None, 0);
                        }
                    }
                }
            }

           

        }

        private void drawNode(SpriteBatch p_spriteBatch,Vector2 p_pos,Color p_tint)
        {
            p_spriteBatch.Draw(m_nodeBoxTex, 
                new Rectangle((int)p_pos.X, (int)p_pos.Y, (int)m_nodeWidth, (int)m_nodeHeight), 
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

        // set render zoom mode
        public void setZoom(Zoom p_zoom)
        {
            m_zoom = p_zoom;
            if (m_zoom == Zoom.NORMAL)
            {
                m_nodeWidth = m_nodeOrigWidth;
                m_nodeHeight = m_nodeOrigHeight;
            }
            else
            {
                m_nodeWidth = m_nodeMiniWidth;
                m_nodeHeight = m_nodeMiniHeight;
            }
        }

        public Zoom getZoom()
        {
            return m_zoom;
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

        public void ScrollX(int p_dx, int p_max)
        {
            scrollInputBuffer.X = (float)p_dx / (float)p_max;
            scrollOffset.X = -m_totalSize.X * scrollInputBuffer.X;
        }

        public void ScrollY(int p_dy, int p_max)
        {
            scrollInputBuffer.Y = (float)p_dy / (float)p_max;
            scrollOffset.Y = -m_totalSize.Y * scrollInputBuffer.Y;
        }

        public void ScrollRefresh(float p_dt)
        {
            scrollOffset = Vector2.Lerp(scrollOffset, -m_totalSize * scrollInputBuffer,
                                               10.0f * p_dt); ;
        }
    }
}
