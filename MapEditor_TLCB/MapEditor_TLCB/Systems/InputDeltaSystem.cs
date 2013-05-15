using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;
using Microsoft.Xna.Framework.Input;

namespace MapEditor_TLCB.Systems
{
	class InputDeltaSystem: EntitySystem
	{
		public InputDeltaSystem()
			: base(typeof(InputDelta))
		{
		}

		protected override void ProcessEntities(Dictionary<int, Entity> p_entities)
		{
			foreach (Entity e in p_entities.Values)
			{
				InputDelta input = m_inputMapper.Get(e);
				input.previousKeyboard = input.currentKeyboard;
				input.currentKeyboard = Keyboard.GetState();
			}
		}

		public override void Initialize()
		{
			m_inputMapper = new ComponentMapper<InputDelta>(world);
		}

		private ComponentMapper<InputDelta> m_inputMapper;
	}
}
