using System;
using System.Collections.Generic;
using System.Windows.Input;
using Ensage;

namespace ZoomImproved
{
	class Program
	{
		// TODO: make config and set zoom Enhanced on load
		// TODO: make key configurable
		private static readonly uint WM_MOUSEWHEEL = 0x020A;

		private static readonly uint VK_CTRL = 0x11;

		private static readonly ConVar ZoomVar = Game.GetConsoleVar("dota_camera_distance");

		static void Main()
		{

			ZoomVar.RemoveFlags(ConVarFlags.Cheat);
			Game.OnWndProc += Game_OnWndProc;
			Game.OnUpdate += Game_OnUpdate;
		}

		private static void Game_OnWndProc(WndEventArgs args)
		{
            
			if (args.Msg == WM_MOUSEWHEEL/* && Game.IsInGame*/ )
			{
                if (Game.IsKeyDown(VK_CTRL))
				{
					// Get HIWORD(wParam)
					var delta = (short)((args.WParam >> 16) & 0xFFFF);
					// GetValue
					var zoomValue = ZoomVar.GetInt();
					if (delta < 0)
						zoomValue += 50;
					else
						zoomValue -= 50;
                    // Set updated value
                    ZoomVar.SetValue(zoomValue);
					// Block processed input from game
					args.Process = false;
				}
			}
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
                               { "fog_enable", 0 }, { "r_farz", 18000 }
                           };
            foreach (var data in list)
            {
                var var = Game.GetConsoleVar(data.Key);
                var.RemoveFlags(ConVarFlags.Cheat);
                var.SetValue(data.Value);
            }
            loaded = true;
        }

        

	}
}
