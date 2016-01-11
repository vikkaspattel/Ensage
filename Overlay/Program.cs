namespace overlays
{
    using System;
    using System.Linq;
	using Ensage;
    using Ensage.Common;
	using Ensage.Common.Menu;
	using SharpDX;

    internal class Program
    {
		private static readonly Menu Menu = new Menu("Overlay", "overlay", true);
		private static void Main()
        {
			Menu.AddItem(new MenuItem("toppanel", "Top Panel").SetValue(true));
			Menu.AddItem(new MenuItem("manabar", "Mana Bar").SetValue(true));
			Menu.AddItem(new MenuItem("lasthithelp", "Last Hit Tips").SetValue(true));
            Menu.AddToMainMenu();
            Drawing.OnDraw += Overlay1;
        }
		private static void Overlay1(EventArgs args)
        {
            if (!Game.IsInGame)
            {
                return;
            }
			var player = ObjectMgr.LocalPlayer;
			if (Menu.Item("manabar").GetValue<bool>())
			{
				if (player == null || player.Team == Team.Observer)
				{
					return;
				}

				var enemies = ObjectMgr.GetEntities<Hero>()
						.Where(x => x.IsVisible && x.IsAlive && x.MaximumMana > 0 && !x.IsIllusion && x.Team != player.Team)
						.ToList();
				foreach (var enemy in enemies)
				{
					var start = HUDInfo.GetHPbarPosition(enemy) + new Vector2(0, HUDInfo.GetHpBarSizeY(enemy) + 1);
					var manaperc = enemy.Mana / enemy.MaximumMana;
					var size = new Vector2(HUDInfo.GetHPBarSizeX(enemy), HUDInfo.GetHpBarSizeY(enemy) *2 / 5);
					
					Drawing.DrawRect(start, size + new Vector2(1, 1), Color.Black);
					Drawing.DrawRect(start, new Vector2(size.X * manaperc, size.Y), new Color(100, 135, 240, 255));
					Drawing.DrawRect(start + new Vector2(-1, -2), size + new Vector2(4, 3), Color.Black, true);
				}
			}
			if (Menu.Item("toppanel").GetValue<bool>())
			{
				for (uint i = 0; i < 10; i++)
				{
					var v = ObjectMgr.GetPlayerById(i).Hero;
					if (v == null || !v.IsAlive) continue;
					var pos = HUDInfo.GetTopPanelPosition(v);
					var sizeX = (float)HUDInfo.GetTopPanelSizeX(v);
					var sizeY = (float)HUDInfo.GetTopPanelSizeY(v);
					var healthDelta = new Vector2(v.Health * sizeX / v.MaximumHealth, 0);
					var manaDelta = new Vector2(v.Mana * sizeX / v.MaximumMana, 0);
					const int height = 7;
					
					Drawing.DrawRect(pos + new Vector2(0, sizeY + 1), new Vector2(healthDelta.X, height), new Color(0, 255, 0, 100));
					Drawing.DrawRect(pos + new Vector2(0, sizeY + 1), new Vector2(sizeX, height), Color.Black, true);
					
					Drawing.DrawRect(pos + new Vector2(0, sizeY + height), new Vector2(manaDelta.X, height), new Color(80, 120, 255, 255));
					Drawing.DrawRect(pos + new Vector2(0, sizeY + height), new Vector2(sizeX, height), Color.Black, true);
				}
			}
			if (Menu.Item("lasthithelp").GetValue<bool>())
			{
				var lasthittip =
				ObjectMgr.GetEntities<Creep>()
				.Where(x => x.IsVisible && x.IsAlive && x.Health < 200  && x.Team != player.Team)
					.ToList();
				foreach (var enemy in lasthittip)
				{
					var start = HUDInfo.GetHPbarPosition(enemy) + new Vector2(0, HUDInfo.GetHpBarSizeY(enemy) + 1);
					var manaperc = enemy.Mana / enemy.MaximumMana;
					var size = new Vector2(HUDInfo.GetHPBarSizeX(enemy), HUDInfo.GetHpBarSizeY(enemy) / 2);
				
					var text = string.Format("{0}", (int)enemy.Health);
					var textSize = Drawing.MeasureText(text, "Arial", new Vector2(size.Y * 2, size.X), FontFlags.AntiAlias);
					var textPos = start + new Vector2(size.X / 2 - textSize.X / 2, -textSize.Y / 2 + 5);
					Drawing.DrawText(text, textPos, new Vector2(size.Y * 3, size.X), Color.White, FontFlags.AntiAlias | FontFlags.DropShadow);
				}
			}
		}
	}
}
