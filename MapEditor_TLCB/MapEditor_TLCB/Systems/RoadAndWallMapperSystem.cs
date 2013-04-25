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
		public RoadAndWallMapperSystem()
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

		WallMapper m_wallMapper;
		RoadMapper m_roadMapper;
	}
}
