﻿using System.Collections.Generic;
using Artemis;
using MapEditor_TLCB.Actions.Interface;
using MapEditor_TLCB.Actions;
using MapEditor_TLCB.Systems;
using System.Diagnostics;
using MapEditor_TLCB.Common;
using System;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework.Graphics;

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
            p_action.PerformAction();
            queuedActions.Add(p_action);
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

            world.TagManager.GetEntity("mainTilemap").GetComponent<TilemapValidate>().validateThisTick = true;
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

            world.TagManager.GetEntity("mainTilemap").GetComponent<TilemapValidate>().validateThisTick = true;
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

		public void LoadSerialiazedActions(string p_completePath)
		{
			ActionsSerialized obj = new ActionsSerialized();
			Serializer seri = new Serializer();
			obj = seri.DeSerializeObject(p_completePath);

			//Avoid the action at place 0 since it's not a acttion
			for (int i = 1; i < obj.actions.getSize(); i++ )
			{
				obj.actions.at(i).AddAffectedSystems(world.SystemManager);
			}


            // Add some form of better clear here
            List<ActionInterface> actions = actionTree.undo();
            while (actions != null)
            {
                foreach (ActionInterface action in actions)
                    if (action != null) action.PerformAction();

                actions = actionTree.undo();
            }
            ////////

            actionTree.Clear(0); // zero: because we're adding data right away and we want to start from root
			actionTree.SetData(obj.nodes, obj.actions);
            PerformActionList(actionTree.setCurrent(obj.currentNode));

			List<Paragraph> info = new List<Paragraph>();
			info.Add(new Paragraph("Successfully loaded saved map from\n" + p_completePath));
			Notification alreadySaving = new Notification("Successfully loaded saved map!", NotificationType.SUCCESS, info, "Load");
			NotificationBarSystem notSys = (NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0]);
			notSys.AddNotification(alreadySaving);

			ContentSystem sys = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]);
			Dictionary<string, Texture2D> textures = sys.GetTextureDictionary();
			
			Texture2D tileMap;
			if (textures.ContainsKey(obj.preferredTileMap)){
				tileMap = textures[obj.preferredTileMap];
			}
			else{
				string tilemap_garden = "tilemap_garden";
				tileMap = textures["tilemap_garden"];

				List<Paragraph> missingTextureDetails = new List<Paragraph>();
				missingTextureDetails.Add(new Paragraph("Failed to find the preferred tileSheet\n" + obj.preferredTileMap + "\nReverting back to " + tilemap_garden));
				Notification missingTexture = new Notification("Failed to find tileSheet", NotificationType.WARNING, missingTextureDetails, "Missing Texture");
				notSys.AddNotification(missingTexture);
			}
			((StartupDialogSystem)world.SystemManager.GetSystem<StartupDialogSystem>()[0]).RequestToChangeTilemap(tileMap);
		}
		public void SaveSerialiazedActions(string p_completePath)
		{
			ActionsSerialized obj = new ActionsSerialized();

            int currentNode = actionTree.m_currentNodeId;

			List<ActionInterface> actions = actionTree.undo();
			while(actions != null)
			{
				foreach (ActionInterface action in actions)
					if (action != null) action.PerformAction();
				
				actions = actionTree.undo();
			}

			obj.nodes = actionTree.GetData().Item1;
			obj.actions = actionTree.GetData().Item2;
            obj.currentNode = currentNode;
			obj.preferredTileMap = ((StartupDialogSystem)(world.SystemManager.GetSystem<StartupDialogSystem>()[0])).tilemap.Name;


			Serializer seri = new Serializer();
			seri.SerializeObject(p_completePath, obj);

            List<Paragraph> info = new List<Paragraph>();
            info.Add(new Paragraph("Successfully saved the map to\n" + p_completePath));
			Notification alreadySaving = new Notification("Successfully saved the map!", NotificationType.SUCCESS, info, "Save");
			((NotificationBarSystem)(world.SystemManager.GetSystem<NotificationBarSystem>()[0])).AddNotification(alreadySaving);

            PerformActionList(actionTree.setCurrent(currentNode));
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