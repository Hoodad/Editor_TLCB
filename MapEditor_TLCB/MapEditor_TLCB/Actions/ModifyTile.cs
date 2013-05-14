using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using MapEditor_TLCB.Actions.Interface;
using MapEditor_TLCB.Systems.Interface;
using MapEditor_TLCB.Systems;
using MapEditor_TLCB.Components;
using System.Runtime.Serialization;

namespace MapEditor_TLCB.Actions
{
	[Serializable()]
	class ModifyTile : ActionInterface
	{
		public Tilemap.TilemapType affectedTilemap;
		public int row;
		public int col;
		public int state;
		public List<ActionSystemInterface> affectedSystems;
		
		public ModifyTile(SystemManager p_systemManager)
		{
			AddAffectedSystems(p_systemManager);
		}
		public ModifyTile(SerializationInfo info, StreamingContext ctxt)
		{
			affectedTilemap = (Tilemap.TilemapType)info.GetValue("AffectedTilemap", typeof(Tilemap.TilemapType));
			row = (int)info.GetValue("Row", typeof(int));
			col = (int)info.GetValue("Col", typeof(int));
			state = (int)info.GetValue("State", typeof(int));
		}

		public void PerformAction()
		{
			foreach (ActionSystemInterface system in affectedSystems)
			{
				system.ReceiveAction(this);
			}
		}

		public void AddAffectedSystems(SystemManager p_systemManager)
		{
			
			if (affectedSystems == null)
			{
				affectedSystems = new List<ActionSystemInterface>();
			}

			//Add the systems that will be affected by this change
			affectedSystems.Add(((RoadToolSystem)p_systemManager.GetSystem<RoadToolSystem>()[0]));
		}

		public void GetObjectData(SerializationInfo info,StreamingContext context)
		{
			info.AddValue("AffectedTilemap", affectedTilemap);
			info.AddValue("Row", row);
			info.AddValue("Col", col);
			info.AddValue("State", state);
		}

        public string GetInfo()
        {
            return "Tile edit (" + row+","+col + ")"; // could state be used here as well? what is it?
        }
	}
}
