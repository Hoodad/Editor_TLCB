using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB
{
	class WallMapper
	{
		public WallMapper()
		{
			m_mapping = new Tuple<ContactMap, int>[23];
			// Inner wall corners (7)
			m_mapping[0] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, true, true, false, true, true), 4);
			m_mapping[1] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, true, true, true, true, false), 34);
			m_mapping[2] = new Tuple<ContactMap, int>(new ContactMap(true, false, true, true, true, true, true, true), 33);
			m_mapping[3] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, false, true, true, true, true), 3);
			// Outer walls (5)
			m_mapping[4]= new Tuple<ContactMap, int>(new ContactMap(false, false, true, true, true, true, true, false), 1);
			m_mapping[5]= new Tuple<ContactMap, int>(new ContactMap(true, false, false, false, true, true, true, true), 32);
			m_mapping[6] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, false, false, false, true, true), 61);
			m_mapping[7] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, true, true, false, false, false), 30);
			// Outer wall corners (3)
			m_mapping[8] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, false, true, true, true, false), 2);
			m_mapping[9] = new Tuple<ContactMap, int>(new ContactMap(true, false, false, false, false, false, true, true), 62);
			m_mapping[10] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, false, false, false, false, false), 60);
			m_mapping[11] = new Tuple<ContactMap, int>(new ContactMap(false, false, true, true, true, false, false, false), 0);
			// Small walls (2)
			m_mapping[12] = new Tuple<ContactMap, int>(new ContactMap(false, false, true, false, false, false, true, false), 6);
			m_mapping[13] = new Tuple<ContactMap, int>(new ContactMap(true, false, false, false, true, false, false, false), 37);
			// Small wall corners (2)
			m_mapping[14] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, false, true, false, true, false), 7);
			m_mapping[15] = new Tuple<ContactMap, int>(new ContactMap(true, false, false, false, false, false, true, false), 67);
			m_mapping[16] = new Tuple<ContactMap, int>(new ContactMap(true, false, true, false, false, false, false, false), 65);
			m_mapping[17] = new Tuple<ContactMap, int>(new ContactMap(false, false, true, false, true, false, false, false), 5);
			// Small wall endings (1)
			m_mapping[18] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, false, true, false, false, false), 10);
			m_mapping[19] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, false, false, false, true, false), 39);
			m_mapping[20] = new Tuple<ContactMap, int>(new ContactMap(true, false, false, false, false, false, false, false), 40);
			m_mapping[21] = new Tuple<ContactMap, int>(new ContactMap(false, false, true, false, false, false, false, false), 38);
			// Single well (0)
			m_mapping[22] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, false, false, false, false, false), 36);

		}

		public int getContactType(int p_x, int p_y, Tilemap p_roadMap)
		{
			ContactMap currentContact = getContactMap(p_x, p_y, p_roadMap);
			for (int i = 0; i < 23; i++)
			{
				if (m_mapping[i].Item1.intersects(currentContact))
				{
					return m_mapping[i].Item2;
				}
			}
			return 240;
		}

		private ContactMap getContactMap(int p_x, int p_y, Tilemap p_roadMap)
		{
			ContactMap contact = new ContactMap();
			if (p_roadMap.getState(p_x, p_y - 1) == 1)
				contact.m_map[0] = true;
			if (p_roadMap.getState(p_x + 1, p_y) == 1)
				contact.m_map[1] = true;
			if (p_roadMap.getState(p_x, p_y + 1) == 1)
				contact.m_map[2] = true;
			if (p_roadMap.getState(p_x - 1, p_y) == 1)
				contact.m_map[3] = true;

			if (p_roadMap.getState(p_x + 1, p_y - 1) == 1)
				contact.m_map[4] = true;
			if (p_roadMap.getState(p_x + 1, p_y + 1) == 1)
				contact.m_map[5] = true;
			if (p_roadMap.getState(p_x - 1, p_y + 1) == 1)
				contact.m_map[6] = true;
			if (p_roadMap.getState(p_x - 1, p_y - 1) == 1)
				contact.m_map[7] = true;

			return contact;
		}

		private Tuple<ContactMap, int>[] m_mapping;
	}
}
