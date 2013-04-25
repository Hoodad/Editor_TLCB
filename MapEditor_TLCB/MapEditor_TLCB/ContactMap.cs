using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapEditor_TLCB
{
	class ContactMap
	{
		public ContactMap()
		{
			m_map = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				m_map[i] = false;
			}
		}

		public ContactMap(bool p_up, bool p_right, bool p_down, bool p_left)
		{
			m_map = new bool[8];
			m_map[0] = p_up;
			m_map[1] = p_right;
			m_map[2] = p_down;
			m_map[3] = p_left;
			m_map[4] = false;
			m_map[5] = false;
			m_map[6] = false;
			m_map[7] = false;
		}

		public ContactMap(bool p_north, bool p_northEast, bool p_east, bool p_southEast,
			bool p_south, bool p_southWest, bool p_west, bool p_northWest)
		{
			m_map = new bool[8];
			m_map[0] = p_north;
			m_map[1] = p_east;
			m_map[2] = p_south;
			m_map[3] = p_west;
			m_map[4] = p_northEast;
			m_map[5] = p_southEast;
			m_map[6] = p_southWest;
			m_map[7] = p_northWest;
		}

		public bool compare(ContactMap p_other)
		{
			for (int i = 0; i < 8; i++)
			{
				if (m_map[i] != p_other.m_map[i])
					return false;
			}
			return true;
		}

		public bool intersects(ContactMap p_other)
		{
			for (int i = 0; i < 8; i++)
			{
				if (m_map[i] && p_other.m_map[i])
					return false;
			}
			return true;
		}

		public bool[] m_map;
	}
}
