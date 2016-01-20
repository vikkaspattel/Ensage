using System;
using Ensage;
using Ensage.Common.Menu;
using System.Collections.Generic;
using System.Windows.Input;

namespace ZoomImproved
{
	class Program
	{
		
		private static readonly Menu Menu = new Menu("zOOm", "zOOm", true);
		private static readonly uint VK_CTRL = 0x11;
		private static readonly uint WM_MOUSEWHEEL = 0x020A;
		private static readonly ConVar ZoomVar = Game.GetConsoleVar("dota_camera_distance");
		private static readonly ConVar renderVar = Game.GetConsoleVar("r_farz");
		static void Main()
		{
			Game.OnWndProc += Game_OnWndProc;
			Game.OnUpdate += Game_OnUpdate;
		}
		private static bool loaded;
		private static void Game_OnUpdate(EventArgs args)
		{
			
			if (!Game.IsInGame)
			{
				loaded = false;
				return;
			}
			if (loaded)
			{
				return;
			}
			Game.GetConsoleVar("fog_enable").SetValue(0);
			var player = ObjectMgr.LocalPlayer;
			if ((player == null) || (player.Team == Team.Observer))
				return;
			Game.PrintMessage("<font color='#aaaaaa'>Zoom Improved </font><font color='#33ff66'>Loaded</font>", MessageType.LogMessage);
			var slider = new MenuItem("distance", "Camera Distance").SetValue(new Slider(1550, 1134, 2500));
			slider.ValueChanged += Slider_ValueChanged;
			Menu.AddItem(slider);
			Menu.AddToMainMenu();
			ZoomVar.RemoveFlags(ConVarFlags.Cheat);
			renderVar.RemoveFlags(ConVarFlags.Cheat);
			ZoomVar.SetValue(slider.GetValue<Slider>().Value);
			renderVar.SetValue(2*(slider.GetValue<Slider>().Value));
			loaded = true;
		}
		private static void Slider_ValueChanged(object sender, OnValueChangeEventArgs e)
		{
			var player = ObjectMgr.LocalPlayer;
			if ((player == null) || (player.Team == Team.Observer))
				return;
				ZoomVar.SetValue(e.GetNewValue<Slider>().Value);
				renderVar.SetValue(2*(e.GetNewValue<Slider>().Value));
		}
		private static void Game_OnWndProc(WndEventArgs args)
		{
			if (args.Msg == WM_MOUSEWHEEL && Game.IsInGame )
			{
				var player = ObjectMgr.LocalPlayer;
				if ((player == null) || (player.Team == Team.Observer))
					return;
				if (Game.IsKeyDown(VK_CTRL))
				{
					var delta = (short)((args.WParam >> 16) & 0xFFFF);
					var zoomValue = ZoomVar.GetInt();
					if (delta < 0)
						zoomValue += 50;
					if (delta > 0) 
						zoomValue -= 50;
					if (zoomValue < 1134)
						zoomValue = 1134;
					ZoomVar.SetValue(zoomValue);
					Menu.Item("distance").SetValue(new Slider(zoomValue, 1134, 2500));
					args.Process = false;
				}	
			}
		}
	}
}
