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

        public float m_nodeOrigWidth = 90.0f;
        public float m_nodeOrigHeight = 30.0f;
        public float m_nodeMiniWidth = 30.0f;
        public float m_nodeMiniHeight = 30.0f;

        public float m_nodeWidth;
        public float m_nodeHeight;
        private Vector2 m_nodeMargin;
        public Vector2 scrollOffset = Vector2.Zero;
        private Vector2 scrollInputBuffer=Vector2.Zero;
        public Vector2 m_renderOffset;
        private Vector2 m_lineRenderOffset;

        Color m_activeBranchCol = new Color(58, 174, 163);
        Color m_currentNodeCol = new Color(58, 255, 163);
        Color m_inactiveNodeCol = Color.White;

        public float m_zoomValue = 0.8f;

        public int m_currentMousePosX = 0;
        public int m_currentMousePosY = 0;

        //
        bool newNodeDirty = false; // has checked if there is new node

        // stats
        int maxSiblings = 0;
        int maxLevel = 0;
        public Vector2 m_totalSize = Vector2.Zero;

        // resources
        Texture2D m_nodeBoxTex;
        SpriteFont m_font;

        List<int> m_renderBatch = new List<int>();

        public UndoTree(GraphicsDevice p_gd, ContentManager p_content)
        {
            m_gd = p_gd;
            m_lineRenderer = new LineRenderer(m_gd,p_content);

			ResetData();
            
			// modes
            m_mode = Mode.TREE;
            m_zoom = Zoom.NORMAL;
            m_nodeWidth=m_nodeOrigWidth;
            m_nodeHeight=m_nodeOrigHeight;

            // create resources
            m_nodeBoxTex = p_content.Load<Texture2D>("node");
            m_font = p_content.Load<SpriteFont>("Arcadepix");
        }

		public void ResetData()
		{
            m_currentNodeId=-1;
            m_startNodeId=-1;
            scrollOffset = Vector2.Zero;
            scrollInputBuffer=Vector2.Zero;
            m_renderOffset = new Vector2(30, 50);
			m_nodes = new InvariableIndexList<ActionNode>();
			m_actions = new InvariableIndexList<ActionInterface>();
			m_currentNodeId = addAction(null);
			m_startNodeId = m_currentNodeId;
		}

        public void Clear(int p_startId=-1)
        {
            scrollOffset = Vector2.Zero;
            scrollInputBuffer = Vector2.Zero;
            m_renderOffset = new Vector2(30, 50);
            m_currentNodeId = p_startId;
            m_startNodeId = p_startId;
            m_nodes = new InvariableIndexList<ActionNode>();
            m_actions = new InvariableIndexList<ActionInterface>(); 
        }

        public Tuple<InvariableIndexList<ActionNode>, InvariableIndexList<ActionInterface>> GetData()
        {
            return new Tuple<InvariableIndexList<ActionNode>, InvariableIndexList<ActionInterface>>(
                m_nodes, m_actions);
        }

        public void SetData(InvariableIndexList<ActionNode> p_nodes, InvariableIndexList<ActionInterface> p_actions)
        {
            m_nodes = p_nodes;
            m_actions = p_actions;
        }

        public void RefreshZoom(float p_dt=-1.0f)
        {
            if (m_zoomValue <= 0.5f)
            {
                if (m_zoom != Zoom.MINI)
                    setZoom(Zoom.MINI);

                if (m_zoomValue <= 0.1f)
                    m_zoomValue = 0.1f;
            }
            else
            {
                if (m_zoomValue > 1.0f)
                    m_zoomValue = 1.0f;

                if (m_zoom != Zoom.NORMAL)
                    setZoom(Zoom.NORMAL);
            }
            //
            if (p_dt > 0.0f)
            {
                if (m_zoom == Zoom.NORMAL)
                {
                    m_nodeWidth = MathHelper.Lerp(m_nodeWidth, m_nodeOrigWidth, 10.0f * p_dt);
                    m_nodeHeight = MathHelper.Lerp(m_nodeHeight, m_nodeOrigHeight, 10.0f * p_dt);
                    m_nodeMargin = new Vector2(m_nodeWidth * 1.1f, m_nodeHeight * 1.5f);
                }
                else
                {
                    m_nodeWidth = MathHelper.Lerp(m_nodeWidth, m_nodeMiniWidth, 10.0f * p_dt);
                    m_nodeHeight = MathHelper.Lerp(m_nodeHeight, m_nodeMiniHeight, 10.0f * p_dt);
                    m_nodeMargin = new Vector2(m_nodeWidth * 3.0f, m_nodeHeight * 1.5f);
                }
            }
        }

        public void update(float p_dt)
        {
            //
            RefreshZoom(p_dt);
            //

            // update sub sizes dependant on any changes
            m_lineRenderOffset = new Vector2(m_nodeWidth / 2, m_nodeHeight / 2);
            
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
                //currentProcNode.traversedflash = MathHelper.Lerp(currentProcNode.traversedflash, 0.0f, p_dt);



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
                Vector2 renderpos = currentProcNode.m_renderPos + scrollOffset;
                if (renderpos.X * m_zoomValue + m_renderOffset.X < (m_renderArea.X + m_renderArea.Width + m_nodeWidth * m_zoomValue) &&
                    renderpos.X * m_zoomValue + m_renderOffset.X > (m_renderArea.X - m_nodeWidth * m_zoomValue) &&
                    renderpos.Y * m_zoomValue + m_renderOffset.Y < (m_renderArea.Y + m_renderArea.Height + m_nodeHeight * m_zoomValue) &&
                    renderpos.Y * m_zoomValue + m_renderOffset.Y > (m_renderArea.Y - m_nodeHeight * m_zoomValue))
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
        public void draw(SpriteBatch p_spriteBatch, Vector2 p_offset)
        {
            Vector2 offset = p_offset;
            int mousex = m_currentMousePosX - (int)((scrollOffset.X) * m_zoomValue + m_renderOffset.X);
            int mousey = m_currentMousePosY - (int)((scrollOffset.Y) * m_zoomValue + m_renderOffset.Y);

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
                    if (isHit(i,mousex,mousey)) tint=Color.LightSkyBlue;
                    //

                    //tint = Color.Lerp(tint,Color.Red,currentRender.traversedflash);
                    drawNode(p_spriteBatch, scrollOffset + currentRender.m_renderPos, tint);
                    // draw action info as well
                    if (m_zoom != Zoom.MINI)
                    {
                        if (currentRender.m_actionIds[0] != -1) action = m_actions[currentRender.m_actionIds[0]];
                        if (action != null)
                        {
                            string info = /*i.ToString()+"| "*/"";
                            if (currentRender.m_actionIds.Count > 1)
                                info += currentRender.GetInfo(); // action group info
                            else
                                info += action.GetInfo();    // single action info
                            Vector2 strSz = m_font.MeasureString(info);
                            float scale = Math.Min(1.0f, (m_nodeWidth / strSz.X));
                            strSz *= scale;
                            Vector2 intpos = (scrollOffset + currentRender.m_renderPos) * m_zoomValue + m_renderOffset;
                            intpos.X = (int)intpos.X; intpos.Y = (int)intpos.Y;
                            //
                            p_spriteBatch.DrawString(m_font, info, intpos, Color.White,
                                0, new Vector2((int)((-m_nodeWidth * m_zoomValue + strSz.X) * 0.5f), (int)((-m_nodeHeight * m_zoomValue + strSz.Y) * 0.5f)),
                                scale, SpriteEffects.None, 0);
                        }
                    }
                }
            }

           

        }

        private void drawNode(SpriteBatch p_spriteBatch,Vector2 p_pos,Color p_tint)
        {
            float scale = scale = m_zoomValue;
            p_spriteBatch.Draw(m_nodeBoxTex,
                new Rectangle((int)(p_pos.X * scale + m_renderOffset.X), (int)(p_pos.Y * scale + m_renderOffset.Y), (int)(m_nodeWidth * scale), (int)(m_nodeHeight * scale)),
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
                    (scrollOffset + m_lineRenderOffset + pos[i-1])*m_zoomValue + m_renderOffset,
                    (scrollOffset + m_lineRenderOffset+ pos[i]) * m_zoomValue + m_renderOffset ,
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
        }

        public Zoom getZoom()
        {
            return m_zoom;
        }

        public bool isThereANewNode()
        {
            if (newNodeDirty)
            {
                newNodeDirty=false;
                return true;
            }
            return false;
        }

        public Vector2 getCurrentNodeContextPosition()
        {
            return (m_nodes[m_currentNodeId].m_renderPos);
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
            newNodeDirty = true;
            return nodeId;
        }

        // add a group of actions
        public int addActionGroup(ActionNode.NodeType p_groupType,List<ActionInterface> p_actions)
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
                groupNode.m_type = p_groupType;
                nodeId = m_nodes.add(groupNode);
                // then add new node index as child to parent
                currentNodeRef.m_children.Add(nodeId);
                // set starting position(render) for node
                groupNode.m_renderPos = currentNodeRef.m_renderPos;
            }
            else // first node
            {
                ActionNode groupNode = new ActionNode(actionIds, m_currentNodeId, 0);
                groupNode.m_type = p_groupType;
                nodeId = m_nodes.add(groupNode);
            }
            m_currentNodeId = nodeId;
            newNodeDirty = true;
            return nodeId;
        }

        // Step down to child
        public List<ActionInterface> redo()
        {
            // step
            List<ActionInterface> actions = null;
            ActionNode currentNodeRef = m_nodes[m_currentNodeId];
            //if 
            if (currentNodeRef.m_redoId > -1)
            {
                actions = setCurrent(currentNodeRef.m_redoId);
            }
            else if (currentNodeRef.m_children.Count > 0)
            {
                m_currentNodeId = currentNodeRef.m_children[0];           
                // change current node
                currentNodeRef = m_nodes[m_currentNodeId];
                // build a list from the indices for returning
                actions = new List<ActionInterface>();
                foreach (int n in currentNodeRef.m_actionIds)
                    actions.Add(m_actions[n]);
                newNodeDirty = true;
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
                int old = m_currentNodeId;
                m_currentNodeId = currentNodeRef.m_parentId;
                // build a list from the indices for returning
                actions = new List<ActionInterface>();
                foreach (int n in currentNodeRef.m_actionIds)
                    actions.Add(m_actions[n]);
                actions.Reverse(); // actions must be reversed when executed for undo (executed from start to end)
                // change current node
                currentNodeRef = m_nodes[m_currentNodeId];
                // change return index
                currentNodeRef.m_redoId = old;
                //
                newNodeDirty = true;
            }
            // return
            return actions;
        }

        // traverse from current to specified id
        // super crappy method
        public List<ActionInterface> traverse(int p_newId)
        {
            ActionNode currentNode = m_nodes[m_currentNodeId];
            ActionNode newNode = m_nodes[p_newId];
            
            List<ActionInterface> upStepActions = new List<ActionInterface>(); // actions encountered when stepping through parents (undo actions)
            List<ActionInterface> downStepActions = new List<ActionInterface>(); // actions encountered when stepping through children (redo actions)

            List<ActionNode> upNodes = null;
            List<ActionNode> downNodes = getStepsFromParentToChild(m_currentNodeId,p_newId);
            if (downNodes==null) // Try find in children
            {
                // if fail, try find in parent
                upNodes = getStepsFromChildToParent(m_currentNodeId,p_newId);
                if (upNodes[upNodes.Count - 1] != m_nodes[p_newId]) // if fail, use parent path to root and append root search from child
                {
                    downNodes = getStepsFromChildToParent(p_newId, 0);
                    downNodes.Reverse();
                        // getStepsFromParentToChild(0, p_newId);
                }

            }
            // add all undos in reverse
            if (upNodes!=null) // if null then we only have redos
            {
                //upNodes[upNodes.Count-1].traversedflash = 1.0f;
                upNodes.RemoveAt(upNodes.Count-1); // never undo bottom(if existant it's always the newId; the to-be-undoed)
                for (int i = 0; i<upNodes.Count; i++) // already in reverse due to search being child-to-parent
                {
                    ActionNode node = upNodes[i];
                    //node.traversedflash = 1.0f;
                    for (int n = node.m_actionIds.Count - 1; n >= 0; n--) // actions internally must be added to list in reverse though
                    {
                        int aid = node.m_actionIds[n];
                        if (aid > -1)
                        {
                            ActionInterface action = m_actions[aid];
                            upStepActions.Add(action);
                        }
                    }
                }
            }
            // then append all redos in normal order
            if (downNodes != null) // if null then we only have undos
            {
                // downNodes[0].traversedflash = 1.0f;
                downNodes.RemoveAt(0); // never redo top(it's either the start or root)
                for (int i = 0;i<downNodes.Count;i++)
                {
                    ActionNode node = downNodes[i];
                    //node.traversedflash = 1.0f;
                    for (int n = 0; n < node.m_actionIds.Count; n++)
                    {
                        int aid = node.m_actionIds[n];
                        if (aid > -1)
                        {
                            ActionInterface action = m_actions[aid];
                            downStepActions.Add(action);
                        }
                    }
                }
            }

            upStepActions.AddRange(downStepActions); // append redos at end of undos

            return upStepActions; // return finalized action list
        }

        public bool findChild(int p_current,int p_childId)
        {
            ActionNode p = m_nodes[p_current];
            Queue<ActionNode> workQueue = new Queue<ActionNode>();
            workQueue.Enqueue(p);
            while(workQueue.Count>0)
            {
                p = workQueue.Dequeue();
                foreach (int childId in p.m_children)
                {
                    //
                    if (childId == p_childId) return true;
                    //
                    ActionNode childNode = m_nodes[childId];
                    workQueue.Enqueue(childNode);
                }
            }
            return false;
        }

        // adds all parents from(and including) child to the specified parent, or the root if not found
        public List<ActionNode> getStepsFromChildToParent(int p_child, int p_parent)
        {
            List<ActionNode> list = new List<ActionNode>();
            int current = p_child;
            list.Add(m_nodes[current]);
            while (current > 0 && current!=p_parent)
            {
                int parent = m_nodes[current].m_parentId;
                list.Add(m_nodes[parent]);
                current = parent;
            }

            return list;
        }

        // adds all parents from(and including) child to the specified parent, or the root if not found
        public List<ActionNode> getStepsFromParentToChild(int p_parent, int p_child)
        {
            List<ActionNode> resultPath = null;
            if (findChild(p_parent, p_child))
            {
                resultPath = getStepsFromChildToParent(p_child, p_parent);
                resultPath.Reverse();
            }
            
            return resultPath;
        }


        public List<ActionInterface> setCurrent(int p_id)
        {
            List<ActionInterface> returnPath = null;
            if (m_currentNodeId != p_id)
            {
                returnPath = traverse(p_id);
                m_currentNodeId = p_id;
                newNodeDirty = true;
            }
            return returnPath;
        }

        public List<ActionInterface> setCurrentByPosition()
        {
            List<ActionInterface> returnPath = null;
            int mousex = m_currentMousePosX - (int)((scrollOffset.X)*m_zoomValue + m_renderOffset.X);
            int mousey = m_currentMousePosY - (int)((scrollOffset.Y)*m_zoomValue + m_renderOffset.Y);
            // only check the visible for collision
            foreach (int i in m_renderBatch)
            {
                if (i != -1 && i < m_nodes.getSize() && m_nodes[i] != null)
                {
                    if (isHit(i,mousex,mousey))
                    {
                        // if passed, click hit
                        // int old = m_currentNodeId;
                        returnPath = setCurrent(i);
                        // m_nodes[m_currentNodeId].m_redoId = old;   Test without for now
                        break;
                    }
                }
            }
            return returnPath;
        }

        public List<ActionInterface> directSelectiveUndoByPosition()
        {
            List<ActionInterface> undoObj = null;
            int mousex = m_currentMousePosX - (int)((scrollOffset.X) * m_zoomValue + m_renderOffset.X);
            int mousey = m_currentMousePosY - (int)((scrollOffset.Y) * m_zoomValue + m_renderOffset.Y);
            // only check the visible for collision
            foreach (int i in m_renderBatch)
            {
                if (i != -1 && i < m_nodes.getSize() && m_nodes[i] != null)
                {
                    if (isHit(i, mousex, mousey))
                    {
                        // if passed, click hit
                        // int old = m_currentNodeId;
                        undoObj = new List<ActionInterface>();
                        foreach (int n in m_nodes[i].m_actionIds)
                            undoObj.Add(m_actions[n]);
                        undoObj.Reverse(); // actions must be reversed when executed for undo (executed from start to end)
                        m_currentNodeId = addActionGroup(m_nodes[i].m_type, undoObj);
                        // m_nodes[m_currentNodeId].m_redoId = old;   Test without for now
                        break;
                    }
                }
            }
            return undoObj;
        }

        public bool isHit(int p_nodeID, int p_x, int p_y)
        {
            ActionNode currentRender = m_nodes[p_nodeID];
            // ye olde box collision
            if (p_x > (currentRender.m_renderPos.X + m_nodeWidth) * m_zoomValue) return false;
            if (p_x < currentRender.m_renderPos.X * m_zoomValue) return false;
            if (p_y > (currentRender.m_renderPos.Y + m_nodeHeight) * m_zoomValue) return false;
            if (p_y < currentRender.m_renderPos.Y * m_zoomValue) return false;
            // if passed
            return true;
        }


        public void ScrollX(float p_dx, float p_max)
        {
            scrollInputBuffer.X = p_dx / p_max;
            scrollOffset.X = -m_totalSize.X * scrollInputBuffer.X;
        }


        public void ScrollY(float p_dy, float p_max)
        {
            scrollInputBuffer.Y = p_dy / p_max;
            scrollOffset.Y = -m_totalSize.Y * scrollInputBuffer.Y;
        }

        public void ScrollRefresh(float p_dt)
        {
            /*scrollOffset = Vector2.Lerp(scrollOffset, -m_totalSize * scrollInputBuffer,
                                               10.0f * p_dt);*/
        }
    }
}
