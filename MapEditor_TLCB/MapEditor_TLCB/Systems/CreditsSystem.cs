using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Artemis;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class CreditsSystem : EntitySystem
	{
		Window creditWindow;
		TextBox textBox;
		Manager manager;
		public CreditsSystem(Manager p_manager)
		{
			manager = p_manager;
		}
		public override void Initialize()
		{
			creditWindow = new Window(manager);
			creditWindow.Init();
			creditWindow.Width = 300;
			creditWindow.Height = 200;
			creditWindow.Center();
			creditWindow.Text = "Credits";
			creditWindow.Visible = false;
			creditWindow.IconVisible = false;
			manager.Add(creditWindow);

			textBox = new TextBox(manager);
			textBox.Init();
			textBox.Parent = creditWindow;
			textBox.Left = 8;
			textBox.Top = 8;
			textBox.Width = creditWindow.ClientArea.Width - 16;
			textBox.Height = creditWindow.Height - 48;
			textBox.Anchor = Anchors.All;
			textBox.Mode = TextBoxMode.Multiline;
			textBox.ReadOnly = true;

			string text = "Makers of The Little Cheese Boy Editor:\n"+ GetCreators();

			text += "\n\nSpecial Thanks to Alexander Brodén and \nMattias Liljeson for their great work \non The Little Cheese Boy!";

			textBox.Text = text;

			textBox.ScrollBars = ScrollBars.None;
			textBox.CanFocus = false;
			textBox.TextColor = Color.White;

		}

		public override void Process()
		{
			if (Keyboard.GetState(0).IsKeyDown(Keys.F1))
			{
				StateSystem stateSys = (StateSystem)(world.SystemManager.GetSystem<StateSystem>()[0]);
				if(stateSys.CanCanvasBeReached())
					creditWindow.Show();
			}
		}
		private string GetCreators()
		{
			List<string> names = new List<string>();
			names.Add("Anton Andersson");
			names.Add("Johan Carlberg");
			names.Add("Jarl Larsson");
			names.Add("Robin Thunström");

			string creators = "";
			System.Random rand = new System.Random();

			for (int i = 0; i < 4;i++ )
			{
				int index = rand.Next(names.Count);
				creators += names[index]+"\n";

				names.RemoveAt(index);
			}

			return creators;
		}
	}
}
