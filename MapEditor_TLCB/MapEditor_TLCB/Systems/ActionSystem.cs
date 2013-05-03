using System.Collections.Generic;
using Artemis;
using MapEditor_TLCB.Actions.Interface;
using MapEditor_TLCB.Actions;
using MapEditor_TLCB.Systems;

namespace MapEditor_TLCB
{
	internal class ActionSystem : EntitySystem
	{
		private List<ActionInterface> queuedActions;
		private List<ActionInterface> performedActions;
		private List<ActionInterface> redoActions;

		public ActionSystem()
			: base()
		{
			queuedActions = new List<ActionInterface>();
			performedActions = new List<ActionInterface>();
			redoActions = new List<ActionInterface>();
		}

		public override void Process()
		{
			if (queuedActions.Count > 0)
			{
				foreach (ActionInterface action in queuedActions)
				{
					action.PerformAction();
					performedActions.Add(action);
				}
				queuedActions.Clear();
				redoActions.Clear();
			}
		}

		public void QueAction(ActionInterface p_action)
		{
			queuedActions.Add(p_action);
		}

		public void UndoLastPerformedAction()
		{
			if (performedActions.Count > 0)
			{
				performedActions[performedActions.Count - 1].PerformAction();
				redoActions.Add(performedActions[performedActions.Count - 1]);
				performedActions.RemoveAt(performedActions.Count - 1);
			}
		}

		public void RedoLastAction()
		{
			if (redoActions.Count > 0)
			{
				redoActions[redoActions.Count - 1].PerformAction();
				performedActions.Add(redoActions[redoActions.Count - 1]);
				redoActions.RemoveAt(redoActions.Count - 1);
			}
		}

		public void LoadSerialiazedActions()
		{
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
		}
		public void SaveSerialiazedActions()
		{
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
		}
		public void ClearAllActions()
		{
			performedActions = new List<ActionInterface>();
			queuedActions = new List<ActionInterface>();
			redoActions = new List<ActionInterface>();
		}
	}
}