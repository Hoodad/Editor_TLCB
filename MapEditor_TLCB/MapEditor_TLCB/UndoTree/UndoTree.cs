using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapEditor_TLCB.Actions.Interface;
using Microsoft.Xna.Framework.Content;

namespace MapEditor_TLCB.Actions
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

        public Rectangle m_renderArea;

        private float m_nodeOrigWidth = 90.0f;
        private float m_nodeOrigHeight = 30.0f;
        private float m_nodeMiniWidth = 10.0f;
        private float m_nodeMiniHeight = 10.0f;

        private float m_nodeWidth;
        private float m_nodeHeight;
        private Vector2 m_nodeMargin;
        private Vector2 scrollOffset = Vector2.Zero;
        private Vector2 scrollInputBuffer=Vector2.Zero;
        private Vector2 m_renderOffset = new Vector2(30, 100);
        private Vector2 m_lineRenderOffset;

        Color m_activeBranchCol = new Color(58, 174, 163);
        Color m_currentNodeCol = new Color(58, 255, 163);
        Color m_inactiveNodeCol = Color.White;

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
            m_lineRenderer = new LineRenderer(m_gd,p_content);
            m_currentNodeId = addAction(null);
            m_startNodeId = m_currentNodeId;

            // modes
            m_mode = Mode.TREE;
            m_zoom = Zoom.NORMAL;
            m_nodeWidth=m_nodeOrigWidth;
            m_nodeHeight=m_nodeOrigHeight;

            // create resources
            m_nodeBoxTex = p_content.Load<Texture2D>("node");
            m_font = p_content.Load<SpriteFont>("Arcadepix");
        }

        public void update(float p_dt)
        {
            // update sub sizes dependant on any changes
            m_lineRenderOffset = new Vector2(m_nodeWidth / 2, m_nodeHeight / 2);
            m_nodeMargin = new Vector2(m_nodeWidth * 1.1f, m_nodeHeight * 1.5f);
            // position
            Queue<int> batch = new Queue<int>();
            m_renderBatch.Clear();
            List<int> columnPerRowCounter = new List<int>(); // "global siblings"
            batch.Enqueue(m_startNodeId);
            maxSiblings = 0; maxLevel = 0; // reset for count
            do
            {
                int currentId = batch.Dequeue();
                ActionNode currentProcNode = m_nodes[currentId];
                int level = currentProcNode.m_level;
                if (level > maxLevel) maxLevel = level;


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
                                               currentProcNode.m_level * m_nodeMargin.Y);
                }
                currentProcNode.m_renderPos = Vector2.Lerp(currentProcNode.m_renderPos,
                                                         goal,
                                               10.0f * p_dt); //<-dt




                // process children and add to batch
                if (columnPerRowCounter.Count <= level) columnPerRowCounter.Insert(level, 0);
                int localSibling = 0;
                bool noactivecildren = true;
                foreach (int childId in currentProcNode.m_children)
                {
                    ActionNode childNode = m_nodes[childId];
                    // set as active branch if a child is activebranch or currentaction
                    if (childNode.m_activeBranch>0 || m_currentNodeId == childId)
                    {
                        currentProcNode.m_activeBranch = 2;
                        noactivecildren = false;
                    }
                    //
                    batch.Enqueue(childId);
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
                if (noactivecildren && currentProcNode.m_activeBranch>0) currentProcNode.m_activeBranch -= 1;


                // render cull
                Vector2 renderpos = currentProcNode.m_renderPos + scrollOffset + m_renderOffset;
                if (renderpos.X < m_renderArea.X + m_renderArea.Width + m_nodeWidth && renderpos.X > m_renderArea.X - m_nodeWidth &&
                    renderpos.Y < m_renderArea.Y + m_renderArea.Height + m_nodeHeight && renderpos.Y > m_renderArea.Y - m_nodeHeight)
                {
                    if (currentProcNode.m_activeBranch>0 || m_mode == Mode.TREE)
                        m_renderBatch.Add(currentId);
                }

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
                        Color tint = m_inactiveNodeCol;
                        if (currentRender.m_activeBranch > 0) tint = m_activeBranchCol;
                        if (i == m_currentNodeId) tint = m_currentNodeCol;
                        drawLine(p_spriteBatch, m_nodes[parentId].m_renderPos, currentRender.m_renderPos, tint);
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
                    Color tint = m_inactiveNodeCol;
                    if (currentRender.m_activeBranch>0) tint = m_activeBranchCol;
                    if (i == m_currentNodeId) tint = m_currentNodeCol;
                    drawNode(p_spriteBatch, scrollOffset + m_renderOffset + currentRender.m_renderPos, tint);
                    // draw action info as well
                    if (m_zoom != Zoom.MINI)
                    {
                        if (currentRender.m_actionIds[0] != -1) action = m_actions[currentRender.m_actionIds[0]];
                        if (action != null)
                        {
                            string info = "";
                            if (currentRender.m_actionIds.Count > 1)
                                info = currentRender.m_info; // action group info
                            else
                                info = action.GetInfo();    // single action info
                            Vector2 strSz = m_font.MeasureString(info);
                            float scale = Math.Min(1.0f, m_nodeWidth / strSz.X);
                            strSz *= scale;
                            Vector2 intpos = scrollOffset + m_renderOffset + currentRender.m_renderPos;
                            intpos.X = (int)intpos.X; intpos.Y = (int)intpos.Y;
                            //
                            p_spriteBatch.DrawString(m_font, info, intpos, Color.White,
                                0, new Vector2((int)((-m_nodeWidth + strSz.X) * 0.5f), (int)((-m_nodeHeight + strSz.Y) * 0.5f) ),
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

        void drawLine(SpriteBatch p_spriteBatch,Vector2 p_end, Vector2 p_start, Color p_color)
        {
            Vector2[] pos = new Vector2[10];
            pos[0]=p_start;
            // start curve
            Vector2 yoffset = new Vector2(0.0f, 1.0f);

            pos[1] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 0.02f), MathHelper.Lerp(p_start.Y, p_end.Y, 0.2f));
            pos[2] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 0.1f), MathHelper.Lerp(p_start.Y, p_end.Y, 0.35f));
            pos[3] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 0.2f), MathHelper.Lerp(p_start.Y, p_end.Y, 0.43f));
            // middles
            pos[4]=new Vector2(MathHelper.Lerp(p_start.X, p_end.X,0.35f),MathHelper.Lerp(p_start.Y, p_end.Y,0.48f));
            pos[5] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 0.65f), MathHelper.Lerp(p_start.Y, p_end.Y, 0.52f));
            // end curve
            pos[6] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 1.0f-0.2f), MathHelper.Lerp(p_start.Y, p_end.Y, 1.0f-0.43f));
            pos[7] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 1.0f-0.1f), MathHelper.Lerp(p_start.Y, p_end.Y, 1.0f-0.35f));
            pos[8] = new Vector2(MathHelper.Lerp(p_start.X, p_end.X, 1.0f-0.02f), MathHelper.Lerp(p_start.Y, p_end.Y, 1.0f-0.2f));


            pos[9]=p_end;
            for (int i = 1; i < 10; i++)
            {
                m_lineRenderer.Draw(p_spriteBatch,
                    scrollOffset + m_lineRenderOffset + m_renderOffset + pos[i-1],
                    scrollOffset + m_lineRenderOffset + m_renderOffset + pos[i],
                    p_color, 1.0f, true);
            }
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

        // add a single action
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

        // add a group of actions
        public int addActionGroup(string p_groupName,List<ActionInterface> p_actions)
        {
            int nodeId = -1;
            // store actions in list
            List<int> actionIds = new List<int>();
            foreach (ActionInterface n in p_actions)
            {
                int actionId = m_actions.add(n);
                actionIds.Add(actionId);
            }
            //
            if (m_currentNodeId >= 0)
            {
                ActionNode currentNodeRef = m_nodes[m_currentNodeId];
                // and add its id to tree list
                ActionNode groupNode = new ActionNode(actionIds, m_currentNodeId, currentNodeRef.m_level + 1);
                groupNode.m_info = p_groupName;
                nodeId = m_nodes.add(groupNode);
                // then add new node index as child to parent
                currentNodeRef.m_children.Add(nodeId);
                // set starting position(render) for node
                groupNode.m_renderPos = currentNodeRef.m_renderPos;
            }
            else // first node
            {
                ActionNode groupNode = new ActionNode(actionIds, m_currentNodeId, 0);
                groupNode.m_info = p_groupName;
                nodeId = m_nodes.add(groupNode);
            }
            m_currentNodeId = nodeId;
            return nodeId;
        }

        // Step down to child
        public List<ActionInterface> redo()
        {
            // step
            List<ActionInterface> actions = null;
            ActionNode currentNodeRef = m_nodes[m_currentNodeId];
            if (currentNodeRef.m_children.Count > 0)
            {
                m_currentNodeId = currentNodeRef.m_children[0];           
                // change current node
                currentNodeRef = m_nodes[m_currentNodeId];
                // build a list from the indices for returning
                actions = new List<ActionInterface>();
                foreach (int n in currentNodeRef.m_actionIds)
                    actions.Add(m_actions[n]);
            }
            // return
            return actions;
        }

        // step up to parent
        public List<ActionInterface> undo()
        {
            // step
            List<ActionInterface> actions = null;
            ActionNode currentNodeRef = m_nodes[m_currentNodeId];
            if (currentNodeRef.m_parentId>=0)
            {
                m_currentNodeId = currentNodeRef.m_parentId;            
                // build a list from the indices for returning
                actions = new List<ActionInterface>();
                foreach (int n in currentNodeRef.m_actionIds)
                    actions.Add(m_actions[n]);
                actions.Reverse(); // actions must be reversed when executed for undo (executed from start to end)
                // change current node
                currentNodeRef = m_nodes[m_currentNodeId];
            }
            // return
            return actions;
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
