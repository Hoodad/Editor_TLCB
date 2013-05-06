﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class StateSystem : EntitySystem
	{
		bool requestToShutdown;
		bool shouldShutdown;
		Manager manager;
		Window confirmWindow;
		Button acceptButton;
		Button cancelButton;


		public StateSystem(Manager p_manager) : base()
		{
			requestToShutdown = false;
			shouldShutdown = false;
			manager = p_manager;
		}

		public override void Initialize()
		{
			Point screenSize = ((ContentSystem)world.SystemManager.GetSystem<ContentSystem>()[0]).GetViewportSize();

			confirmWindow = new Window(manager);
			confirmWindow.Init();
			confirmWindow.Text = "Would you like to exit?";
			confirmWindow.Width = 248;
			confirmWindow.Height = 48;
			confirmWindow.Center();
			confirmWindow.Visible = false;
			confirmWindow.Resizable = false;
			manager.Add(confirmWindow);

			acceptButton = new Button(manager);
			acceptButton.Init();
			acceptButton.Parent = confirmWindow;
			acceptButton.Width = 100;
			acceptButton.Height = 24;
			acceptButton.Click +=new TomShane.Neoforce.Controls.EventHandler(ConfirmedExit);
			acceptButton.Left = 12;
			acceptButton.Top = 8;
			acceptButton.Text = "Yes";

			cancelButton = new Button(manager);
			cancelButton.Init();
			cancelButton.Parent = confirmWindow;
			cancelButton.Width = 100;
			cancelButton.Height = 24;
			cancelButton.Click += new TomShane.Neoforce.Controls.EventHandler(CancelExit);
			cancelButton.Left = 124;
			cancelButton.Top = 8;
			cancelButton.Text = "No thanks";

		}

		public void RequestToShutdown()
		{
			requestToShutdown = true;
			confirmWindow.Visible = true;
			confirmWindow.ShowModal();
			cancelButton.Focused = true;
		}
		public bool ShouldShutDown()
		{

			return shouldShutdown;
		}

		void ConfirmedExit (object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			shouldShutdown = true;
			confirmWindow.Visible = false;
		}
		void CancelExit(object sender, TomShane.Neoforce.Controls.EventArgs e)
		{
			requestToShutdown = false;
			confirmWindow.Close(ModalResult.Cancel);
		}

	}
}
