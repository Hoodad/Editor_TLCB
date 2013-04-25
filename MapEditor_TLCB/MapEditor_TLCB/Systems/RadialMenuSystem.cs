using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Artemis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MapEditor_TLCB.Systems
{
	class RadialMenuSystem : EntitySystem
	{
		RadialMenuContext m_context;

		GraphicsDevice m_device;
		ContentManager m_content;

		public RadialMenuSystem(GraphicsDevice p_device, ContentManager p_content)
		{
			m_device = p_device;
			m_content = p_content;
		}

		public override void Initialize()
		{
			m_context = new RadialMenuContext(m_device, m_content);

			//Edit menu
			Texture2D temp = m_content.Load<Texture2D>("radialTemp");
			Texture2D main = m_content.Load<Texture2D>("radialMain");


			EventData tempEvent = new EventData(null, null);

			List<RadialMenuItem> cowList = new List<RadialMenuItem>();
			cowList.Add(new RadialMenuItem("Bomb", temp, tempEvent));
			cowList.Add(new RadialMenuItem("Speedup", temp, tempEvent));
			cowList.Add(new RadialMenuItem("Super Drink", temp, tempEvent));
			RadialMenu cowMenu = new RadialMenu(m_device, m_content, cowList, temp, null);

			List<RadialMenuItem> horseList = new List<RadialMenuItem>();
			//horseList.Add(new RadialMenuItem("Go Back", star, ev0, false));
			horseList.Add(new RadialMenuItem("Cheese Boy", temp, tempEvent));
			horseList.Add(new RadialMenuItem("Rat", temp, tempEvent));
			horseList.Add(new RadialMenuItem("Infected Rat", temp, tempEvent));
			horseList.Add(new RadialMenuItem("Napoleon", temp, tempEvent));
			RadialMenu horseMenu = new RadialMenu(m_device, m_content, horseList, temp, null);

			List<RadialMenuItem> penList = new List<RadialMenuItem>();
			//penList.Add(new RadialMenuItem("Go Back", star, ev0, false));
			penList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			penList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			penList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			RadialMenu penMenu = new RadialMenu(m_device, m_content, penList, temp, null);

			List<RadialMenuItem> toolsList = new List<RadialMenuItem>();
			//toolsList.Add(new RadialMenuItem("Go Back", star, ev0, false));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			toolsList.Add(new RadialMenuItem("Dummy", temp, tempEvent));
			RadialMenu toolsMenu = new RadialMenu(m_device, m_content, toolsList, temp, null);

			List<RadialMenuItem> items = new List<RadialMenuItem>();
			items.Add(new RadialMenuItem("Power-Ups", temp, cowMenu));
			items.Add(new RadialMenuItem("Characters", temp, horseMenu));
			items.Add(new RadialMenuItem("Dummy", temp, penMenu));
			items.Add(new RadialMenuItem("Dummy", temp, toolsMenu));

			RadialMenu menu = new RadialMenu(m_device, m_content, items, main, null);

			cowMenu.setParent(menu);
			horseMenu.setParent(menu);
			penMenu.setParent(menu);
			toolsMenu.setParent(menu);

			m_context.addRadialMenu(menu);
			m_context.addRadialMenu(cowMenu);
			m_context.addRadialMenu(horseMenu);
			m_context.addRadialMenu(penMenu);
			m_context.addRadialMenu(toolsMenu);
		}

		public override void Process()
		{
			float dt = World.Delta / 1000.0f;
			m_context.update(dt);
		}
		public void Render(SpriteBatch p_spriteBatch)
		{
			m_context.draw5(p_spriteBatch);
		}
	}
}
