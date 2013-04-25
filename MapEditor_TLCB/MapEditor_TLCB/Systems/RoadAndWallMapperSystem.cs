using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB.Systems
{
	class RoadAndWallMapperSystem: EntitySystem
	{
		public RoadAndWallMapperSystem(): base()
		{
			m_wallMapper = new WallMapper();
			m_roadMapper = new RoadMapper();
		}

		public WallMapper getWallMapper()
		{
			return m_wallMapper;
		}

		public RoadMapper getRoadMapper()
		{
			return m_roadMapper;
		}

		public override void Initialize()
		{
		}

		protected override void ProcessEntities(System.Collections.Generic.Dictionary<int, Artemis.Entity> entities)
		{
		}

		WallMapper m_wallMapper;
		RoadMapper m_roadMapper;
	}
}
