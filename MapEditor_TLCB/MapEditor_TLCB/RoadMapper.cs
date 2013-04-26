using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Components;

namespace MapEditor_TLCB
{
	class RoadMapper
	{
		public RoadMapper()
		{
			m_mapping = new Tuple<ContactMap, int>[16];
			m_mapping[0] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, false), 121);

			m_mapping[1] = new Tuple<ContactMap, int>(new ContactMap(false, true, false, false), 93);
			m_mapping[2] = new Tuple<ContactMap, int>(new ContactMap(false, false, true, false), 123);
			m_mapping[3] = new Tuple<ContactMap, int>(new ContactMap(false, false, false, true), 94);
			m_mapping[4] = new Tuple<ContactMap, int>(new ContactMap(true, false, false, false), 153);

			m_mapping[5] = new Tuple<ContactMap, int>(new ContactMap(false, true, false, true), 91);
			m_mapping[6] = new Tuple<ContactMap, int>(new ContactMap(true, false, true, false), 122);

			m_mapping[7] = new Tuple<ContactMap, int>(new ContactMap(true, true, false, false), 150);
			m_mapping[8] = new Tuple<ContactMap, int>(new ContactMap(false, true, true, false), 90);
			m_mapping[9] = new Tuple<ContactMap, int>(new ContactMap(false, false, true, true), 92);
			m_mapping[10] = new Tuple<ContactMap, int>(new ContactMap(true, false, false, true), 152);

			m_mapping[11] = new Tuple<ContactMap, int>(new ContactMap(true, true, false, true), 155);
			m_mapping[12] = new Tuple<ContactMap, int>(new ContactMap(false, true, true, true), 95);
			m_mapping[13] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, false), 124);
			m_mapping[14] = new Tuple<ContactMap, int>(new ContactMap(true, false, true, true), 125);

			m_mapping[15] = new Tuple<ContactMap, int>(new ContactMap(true, true, true, true), 154);
		}

		public int getContactType(int p_x, int p_y, Tilemap p_roadMap)
		{
			ContactMap currentContact = getContactMap(p_x, p_y, p_roadMap);
			for (int i = 0; i < 16; i++)
			{
				if (m_mapping[i].Item1.compare(currentContact))
				{
					return m_mapping[i].Item2;
				}
			}
			return 0;
		}

		private ContactMap getContactMap(int p_x, int p_y, Tilemap p_roadMap)
		{
			ContactMap contact = new ContactMap();
			if (p_roadMap.getState(p_x, p_y - 1) >= 0)
				contact.m_map[0] = true;
			if (p_roadMap.getState(p_x + 1, p_y) >= 0)
				contact.m_map[1] = true;
			if (p_roadMap.getState(p_x, p_y + 1) >= 0)
				contact.m_map[2] = true;
			if (p_roadMap.getState(p_x - 1, p_y) >= 0)
				contact.m_map[3] = true;
			return contact;
		}

		private Tuple<ContactMap, int>[] m_mapping;
	}
}
