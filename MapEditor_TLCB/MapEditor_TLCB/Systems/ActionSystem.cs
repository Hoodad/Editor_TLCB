using System.Collections.Generic;
using Artemis;
using MapEditor_TLCB.Actions.Interface;
using MapEditor_TLCB.Actions;
using MapEditor_TLCB.Systems;
using System.Diagnostics;

namespace MapEditor_TLCB
{
	class ActionSystem : EntitySystem
	{
		private struct EditorAction
		{
			public ActionInterface action;
			public int groupID;

			public EditorAction(ActionInterface p_action)
			{
				action = p_action;
				groupID = -1;
			}
			public EditorAction(ActionInterface p_action, int p_ID)
			{
				action = p_action;
				groupID = p_ID;
			}
		}
        private List<ActionInterface> queuedActions;
		// private List<EditorAction> queuedActions;
		// private List<EditorAction> performedActions;
		// private List<EditorAction> redoActions;              Changed to undo tree structure
        private UndoTree actionTree;


		private bool grouping;
        private ActionNode.NodeType currentGroupType;

		static int groupCount = 0;
        private bool faultyStopCalled = false;

		public ActionSystem()
			: base()
		{
			//queuedActions = new List<EditorAction>();
			//performedActions = new List<EditorAction>();
			//redoActions = new List<EditorAction>();
            queuedActions = new List<ActionInterface>();
			grouping = false;
		}

        public override void Initialize()
        {
            UndoTreeSystem undotreesystem = ((UndoTreeSystem)world.SystemManager.GetSystem<UndoTreeSystem>()[0]);
            actionTree = undotreesystem.undoTreeContainer.m_undoTree; // <-- "super ugly, maybe fix, or not, whatever" -Jarl
        }

		public override void Process()
		{
            /* grouping disabled for now
			if (queuedActions.Count > 0)
			{
				foreach (EditorAction editorAction in queuedActions)
				{
					editorAction.action.PerformAction();
					performedActions.Add(editorAction);
				}
				queuedActions.Clear();
				redoActions.Clear();
			}
             * */ 
            
            // if a faulty stop was encountered
            // do a second check to make sure there are no
            // buffered actions remaining
            if (faultyStopCalled)
            {
                StopGroupingActions();
                faultyStopCalled = false;
            }
		}

		public void EnqueueAction(ActionInterface p_action)
		{
            /*
			int groupID = -1;
			if (grouping)
			{
				groupID = groupCount;
			}
			queuedActions.Add(new EditorAction(p_action, groupID));
             * */

            p_action.PerformAction();
            // if (grouping)                        Does not work properly right now due to derpy events
                queuedActions.Add(p_action);
            /*else
                actionTree.addAction(p_action);*/
		}

		public void UndoLastPerformedAction()
		{
			// PerformAction(performedActions, redoActions);
            List<ActionInterface> actions = actionTree.undo();
            if (actions != null)
            {
                foreach (ActionInterface action in actions)
                    if (action != null) action.PerformAction();
            }
		}

		public void RedoLastAction()
		{
			// PerformAction(redoActions, performedActions);
            List<ActionInterface> actions = actionTree.redo();
            if (actions != null)
            {
                foreach (ActionInterface action in actions)
                    if (action != null) action.PerformAction();
            }
		}

        public void PerformActionList(List<ActionInterface> p_actions)
        {
            // PerformAction(performedActions, redoActions);
            if (p_actions != null)
            {
                foreach (ActionInterface action in p_actions)
                    if (action != null) action.PerformAction();
            }
        }

        /*
		private void PerformAction(List<EditorAction> p_originalActionOwner, List <EditorAction> p_newActionOwner)
		{
			bool hasPerformedAllAssociatedActions = false;

			while (hasPerformedAllAssociatedActions == false)
			{
				if (p_originalActionOwner.Count > 0)
				{
					EditorAction lastAction = p_originalActionOwner[p_originalActionOwner.Count - 1];
					lastAction.action.PerformAction();

					if (p_originalActionOwner.Count > 1)
					{
						EditorAction secondLastAction = p_originalActionOwner[p_originalActionOwner.Count - 2];

						if (!AreActionsInSameGroup(lastAction, secondLastAction))
						{
							hasPerformedAllAssociatedActions = true;
						}
					}
					else
					{
						hasPerformedAllAssociatedActions = true;
					}

					p_newActionOwner.Add(lastAction);
					p_originalActionOwner.RemoveAt(p_originalActionOwner.Count - 1);
				}
				else
				{
					hasPerformedAllAssociatedActions = true;
				}
			}
		}
         * */

		public void StartGroupingActions(ActionNode.NodeType p_nodeType)
		{
            if (grouping)
            {
                Debug.Print("Warning, Action System was asked to START grouping actions while it was already grouping actions");
            }
            else
            {
                currentGroupType = p_nodeType;
            }
			grouping = true;
		}
		public void StopGroupingActions()
		{
            if (queuedActions.Count > 0)
            {
                if (!grouping)
                {
                    Debug.Print("Warning, Action System was asked to STOP grouping actions while it was already STOPPED");
                }
                else
                {
                    groupCount++;
                    grouping = false;
                    actionTree.addActionGroup(currentGroupType, queuedActions);
                    currentGroupType = ActionNode.NodeType.NONE;
                    queuedActions.Clear();
                    faultyStopCalled = false;
                }
            }
            else
            {
                faultyStopCalled = true;
                Debug.Print("Warning, Action System was asked to STOP grouping actions with no actions in group");
            }
		}

		public void LoadSerialiazedActions()
		{
			/*
			ActionsSerialized obj = new ActionsSerialized();
			Serializer seri = new Serializer();
			obj = seri.DeSerializeObject("SerializeObjects.txt");

			performedActions = obj.performedActions;
			queuedActions = obj.queuedActions;
			redoActions = obj.redoActions;

			foreach (ActionInterface action in performedActions)
			{
				action.AddAffectedSystems(world.SystemManager);
			}

			foreach (ActionInterface action in queuedActions)
			{
				action.AddAffectedSystems(world.SystemManager);
			}

			foreach (ActionInterface action in redoActions)
			{
				action.AddAffectedSystems(world.SystemManager);

			}
			while (redoActions.Count > 0)
			{
				RedoLastAction();
			}
			*/
		}
		public void SaveSerialiazedActions(string p_completePath)
		{
			ActionsSerialized obj = new ActionsSerialized();
			obj.queuedActions = queuedActions;

			Serializer seri = new Serializer();
			seri.SerializeObject(p_completePath, obj);

			Notification alreadySaving = new Notification("Successfully saved the map! Its located " + p_completePath, NotificationType.SUCCESS);
			((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(alreadySaving);
		}
		public void ClearAllActions()
		{
			//performedActions = new List<EditorAction>();
			//queuedActions = new List<EditorAction>();
			//redoActions = new List<EditorAction>();
		}
		private bool AreActionsInSameGroup(EditorAction p_actionRed, EditorAction p_actionBlue)
		{
			if (p_actionRed.groupID != -1 && p_actionBlue.groupID != -1)
			{
				if (p_actionRed.groupID == p_actionBlue.groupID)
				{
					return true;
				}
			}
			return false;
		}
	}
}