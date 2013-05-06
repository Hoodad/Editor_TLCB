using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using MapEditor_TLCB.Actions.Interface;
using MapEditor_TLCB.Systems.Interface;


namespace MapEditor_TLCB
{
	[Serializable()]
	class ChangeColor : ActionInterface
	{
		public Color color;
		public List<ActionSystemInterface> affectedSystems;
		public ChangeColor(Color p_color, SystemManager p_systemManager)
		{
			color = p_color;
			affectedSystems = new List<ActionSystemInterface>();
			AddAffectedSystems(p_systemManager);
			
		}
		public ChangeColor(SerializationInfo info, StreamingContext ctxt)
		{
			color = (Color)info.GetValue("Color", typeof(Color));
			//Don't fetch any affected System from the info
		}

		public void PerformAction()
		{
			foreach (ActionSystemInterface system in affectedSystems)
			{
				system.ReceiveAction(this);
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Color", color);
			//Don't store affected system in the file as they will change between executions
		}

		public void AddAffectedSystems(SystemManager p_systemManager)
		{
			if (affectedSystems == null)
			{
				affectedSystems = new List<ActionSystemInterface>();
			}
			//Add the system needing this information to affectedsystems
			/*m_affectedSystems.Add(((RenderingSystem)
				p_systemManager.GetSystem<RenderingSystem>()[0]));*/
		}

        public string GetInfo()
        {
            return "Color change";
        }
	}
}
