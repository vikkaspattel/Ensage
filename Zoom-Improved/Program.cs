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
		private static readonly uint WM_MOUSEWHEEL = 0x020A;
		private static readonly ConVar ZoomVar = Game.GetConsoleVar("dota_camera_distance");
		static void Main()
		{
			var slider = new MenuItem("distance", "Distance Value").SetValue(new Slider(1584, 1134, 2484));
			slider.ValueChanged += Slider_ValueChanged;
			Menu.AddItem(slider);
			Menu.AddToMainMenu();
			ZoomVar.RemoveFlags(ConVarFlags.Cheat);
			ZoomVar.SetValue(slider.GetValue<Slider>().Value);
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
			var list = new Dictionary<string, float>
			{
				{ "dota_camera_disable_zoom", 1 }, { "r_farz", 18000 }, { "fog_enable", 0 }, { "dota_camera_distance", 1584 }
			};
			foreach (var data in list)
			{
				var var = Game.GetConsoleVar(data.Key);
				var.RemoveFlags(ConVarFlags.Cheat);
				var.SetValue(data.Value);
			}
			loaded = true;
		}
		private static void Slider_ValueChanged(object sender, OnValueChangeEventArgs e)
		{
			ZoomVar.SetValue(e.GetNewValue<Slider>().Value);
		}
		private static void Game_OnWndProc(WndEventArgs args)
		{
		if (args.Msg == WM_MOUSEWHEEL && Game.IsInGame )
			{
				var delta = (short)((args.WParam >> 16) & 0xFFFF);
				var zoomValue = ZoomVar.GetInt();
				if (delta < 0 && zoomValue < 2435)
					
					zoomValue += 50;
					
				if (delta > 0 && zoomValue > 1183)
					zoomValue -= 50;
				ZoomVar.SetValue(zoomValue);
				Menu.Item("distance").SetValue(new Slider(zoomValue, 1134, 2484));
				args.Process = true;
			}
		}
	}
}
