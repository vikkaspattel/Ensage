using Ensage;
using Ensage.Common.Menu;

namespace ZoomImproved
{
	class Program
	{
		private static readonly Menu Menu = new Menu("zOOm", "zOOm", true);
		private static readonly uint WM_MOUSEWHEEL = 0x020A;
		private static readonly ConVar ZoomVar = Game.GetConsoleVar("dota_camera_distance");
		
		static void Main()
		{
			var slider = new MenuItem("distance", "Distance Value").SetValue(new Slider(1550, 1134, 3000));
			slider.ValueChanged += Slider_ValueChanged;
			Menu.AddItem(slider);
			Menu.AddToMainMenu();
			ZoomVar.RemoveFlags(ConVarFlags.Cheat);
			ZoomVar.SetValue(slider.GetValue<Slider>().Value);
			Game.GetConsoleVar("r_farz").SetValue(18000);
			Game.GetConsoleVar("fog_enable").SetValue(0);
			Game.GetConsoleVar("dota_camera_disable_zoom").SetValue(1);
			Game.OnWndProc += Game_OnWndProc;
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
				if (delta < 0)
					zoomValue += 50;
				else
					zoomValue -= 50;
				ZoomVar.SetValue(zoomValue);
				Menu.Item("distance").SetValue(new Slider(zoomValue, 1134, 3000));
				args.Process = false;
			}
		}
	}
}
