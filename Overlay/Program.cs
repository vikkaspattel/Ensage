namespace overlays
{
    using System;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using System.Collections.Generic;
    using SharpDX;

    internal class Program
    {
		private static readonly Menu Menu = new Menu("Overlay", "overlay", true);
		private static bool inGame;
		private static Hero hero;
		private static readonly List<ParticleEffect> ParticleEffects = new List<ParticleEffect>();
		private static readonly Vector4[] Spots = { new Vector4(2690, -4409, 3529, -5248), new Vector4(3936, -3277, 5007, -4431), new Vector4(1088, -3200, 2303, -4543), new Vector4(-3307, 383, -2564, -413), new Vector4(-1023, -2728, 63, -3455), new Vector4(-2227, -3968, -1463, -4648), new Vector4(-4383, 1295, -3136, 400),  new Vector4(3344, 942, 4719, 7), new Vector4(-3455, 4927, -2688, 3968), new Vector4(-4955, 4071, -3712, 3264), new Vector4(3456, -384, 4543, -1151), new Vector4(-1967, 3135, -960, 2176), new Vector4(-831, 4095, 0, 3200), new Vector4(448, 3775, 1663, 2816)};
		private static Vector2 GetTopPanelPosition(Hero v)
		{
			Vector2 vec2;
			var handle = v.Handle;
			if (TopPos.TryGetValue(handle, out vec2)) return vec2;
			vec2 = HUDInfo.GetTopPanelPosition(v);
			TopPos.Add(handle,vec2);
			return vec2;
		}
		private static readonly Dictionary<uint,Vector2> TopPos=new Dictionary<uint, Vector2>();
		private static Vector2 GetTopPalenSize(Hero hero)
		{
			return new Vector2((float)HUDInfo.GetTopPanelSizeX(hero), (float)HUDInfo.GetTopPanelSizeY(hero));
		}
		private static void Main()
        {
			Menu.AddItem(new MenuItem("toppanel", "Top Panel").SetValue(true));
			Menu.AddItem(new MenuItem("manabar", "Mana Bar").SetValue(true));
			Menu.AddItem(new MenuItem("spawnbox", "Spawnbox")).SetValue(true).ValueChanged += (sender, arg) => { ValueChanged(arg.GetNewValue<bool>()); };
			Menu.AddItem(new MenuItem("lasthithelp", "Last Hit Tips").SetValue(true));
			Menu.AddToMainMenu();
			Drawing.OnDraw += Overlay1;
			Game.OnUpdate += Game_OnUpdate;
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
				foreach (var v in Ensage.Common.Objects.Heroes.All.Where(x=>x.IsAlive))
				{
					var pos = GetTopPanelPosition(v);
					var size = GetTopPalenSize(v);
					var healthDelta = new Vector2(v.Health * size.X / v.MaximumHealth, 0);
					var manaDelta = new Vector2(v.Mana * size.X / v.MaximumMana, 0);
					const int height = 7;
					
					Drawing.DrawRect(pos + new Vector2(0, size.Y + 1), new Vector2(healthDelta.X, height), new Color(0, 255, 0, 100));
					Drawing.DrawRect(pos + new Vector2(0, size.Y + 1), new Vector2(size.X, height), Color.Black, true);
					
					Drawing.DrawRect(pos + new Vector2(0, size.Y + height), new Vector2(manaDelta.X, height), new Color(80, 120, 255, 255));
					Drawing.DrawRect(pos + new Vector2(0, size.Y + height), new Vector2(size.X, height), Color.Black, true);
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
		private static void Game_OnUpdate(EventArgs args)
		{
			if (!Utils.SleepCheck("anotherSpawnBoxes"))
				return;
			if (!inGame)
			{
				hero = ObjectMgr.LocalHero;
				if (!Game.IsInGame || hero == null)
				{
					Utils.Sleep(1000, "anotherSpawnBoxes");
					return;
				}
				if (ParticleEffects.Any())
				{
					foreach (var particleEffect in ParticleEffects)
					particleEffect.Dispose();
					ParticleEffects.Clear();
				}
				if (Menu.Item("spawnbox").GetValue<bool>())
					DrawRectangles();
				inGame = true;
			}
			if (!Game.IsInGame)
			{
				inGame = false;
				return;
			}
			Utils.Sleep(10000, "anotherSpawnBoxes");
		}
		private static void ChangeColor(int value, Color color)
		{
			foreach (var effect in ParticleEffects)
			{
				effect.SetControlPoint(1, new Vector3(0, 125, 0));
			}
		}
			
		private static void ValueChanged(bool enabled)
		{
			if (enabled) DrawRectangles();
			else DeleteRectangles();
		}
			
		private static void DeleteRectangles()
		{
			foreach (var particleEffect in ParticleEffects)
			particleEffect.Dispose();
		}
		
		private static void DrawRectangles()
		{
			foreach (var spot in Spots)
			CreateRectangle(new Vector3(spot.X, spot.Y, 0), new Vector3(spot.Z, spot.W, 0));
		}
		private static void CreateRectangle(Vector3 position1, Vector3 position2)
		{
			const double bonus = 1.115;
			DrawLine(new Vector3((position1.X + position2.X) / 2, position1.Y, 0), (float) ((position2.X - position1.X) / 2 * bonus), 1, 0);
			DrawLine(new Vector3((position1.X + position2.X) / 2, position2.Y, 0), (float) ((position2.X - position1.X) / 2 * bonus), 1, 0);
			DrawLine(new Vector3(position1.X, (position1.Y + position2.Y) / 2, 0), (float) ((position1.Y - position2.Y) / 2 * bonus), 0, 1);
			DrawLine(new Vector3(position2.X, (position1.Y + position2.Y) / 2, 0), (float) ((position1.Y - position2.Y) / 2 * bonus), 0, 1);
		}
		private static void DrawLine(Vector3 position, float size, int directionf, int directionu)
		{
			var effect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", position);
			ParticleEffects.Add(effect);
			effect.SetControlPoint(1, new Vector3(0, 125, 0));
			effect.SetControlPoint(2, new Vector3(size, 255, 0));
			effect.SetControlPointOrientation(4, new Vector3(directionf, 0, 0), new Vector3(directionu, 0, 0), new Vector3(0, 0, 0));
		}
	}
}
