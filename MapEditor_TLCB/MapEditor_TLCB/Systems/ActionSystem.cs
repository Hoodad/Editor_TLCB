﻿using System.Collections.Generic;
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
		private List<EditorAction> queuedActions;
		private List<EditorAction> performedActions;
		private List<EditorAction> redoActions;
		private bool grouping;

		static int groupCount = 0;

		public ActionSystem()
			: base()
		{
			queuedActions = new List<EditorAction>();
			performedActions = new List<EditorAction>();
			redoActions = new List<EditorAction>();
			grouping = false;
		}

		public override void Process()
		{
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
		}

		public void QueAction(ActionInterface p_action)
		{
			int groupID = -1;
			if (grouping)
			{
				groupID = groupCount;
			}
			queuedActions.Add(new EditorAction(p_action, groupID));
		}

		public void UndoLastPerformedAction()
		{
			PerformAction(performedActions, redoActions);
		}

		public void RedoLastAction()
		{
			PerformAction(redoActions, performedActions);
		}
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

		public void StartGroupingActions()
		{
			if (grouping)
			{
				Debug.Print("Warning, Action System was asked to START grouping actions while it was already grouping actions");
			}
			grouping = true;
		}
		public void StopGroupingActions()
		{
			if (!grouping)
			{
				Debug.Print("Warning, Action System was asked to STOP grouping actions while it was already STOPPED");
			}
			else
			{
				groupCount++;
				grouping = false;
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
		public void SaveSerialiazedActions()
		{
			/*
			while (performedActions.Count > 0)
			{
				UndoLastPerformedAction();
			}

			ActionsSerialized obj = new ActionsSerialized();
			obj.queuedActions = queuedActions;
			obj.performedActions = performedActions;
			obj.redoActions = redoActions;

			Serializer seri = new Serializer();
			seri.SerializeObject("SerializeObjects.txt", obj);

			while (redoActions.Count > 0)
			{
				RedoLastAction();
			}
			 * */
		}
		public void ClearAllActions()
		{
			performedActions = new List<EditorAction>();
			queuedActions = new List<EditorAction>();
			redoActions = new List<EditorAction>();
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