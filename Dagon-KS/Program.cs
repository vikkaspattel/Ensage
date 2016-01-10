using System;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace DagonKS
{
	internal class Program
	{
		private static readonly Menu Menu = new Menu("Dagon KS", "dagonks", true);
		private static Hero me;
		private static readonly int[] DagonDamage = new int[5] { 400, 500, 600, 700, 800 };
		private static readonly int[] DagonRange = new int[5] { 600, 650, 700, 750, 800 };
		static void Main(string[] args)
		{
			Menu.AddItem(new MenuItem("toggle", "Kill Steal").SetValue(true).SetTooltip("Auto use Dagon for Kill Steal"));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
		}
		public static void Game_OnUpdate(EventArgs args)
		{
			if (!Game.IsInGame)
			return;
			me = ObjectMgr.LocalHero;
			if (Menu.Item("toggle").GetValue<bool>())
			{
				var dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
				var enemy = ObjectMgr.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.UnitState.HasFlag(UnitState.MagicImmune))
					.ToList();
				foreach (var i in enemy)
				{
					var linken = i.Inventory.Items.FirstOrDefault(x => x.Name == "item_sphere");
					var sphere = i.Modifiers.Any(x => x.Name == "modifier_item_sphere_target");
					var ta = i.Modifiers.Any(x => x.Name == "modifier_templar_assassin_refraction_damage");
					var dazzle = i.Modifiers.Any(x => x.Name == "modifier_dazzle_shallow_grave");
					var abaddon = i.Modifiers.Any(x => x.Name == "modifier_abaddon_borrowed_time");
					var bm = i.Modifiers.Any(x => x.Name == "modifier_item_blade_mail_reflect");
					var pipe = i.Modifiers.Any(x => x.Name == "modifier_item_pipe_barrier");
				
					if (dagon.CanBeCasted() && Utils.SleepCheck("dagon"))
					{
						if ((linken != null && linken.Cooldown == 0) || (sphere || ta || dazzle || abaddon || bm || pipe || i.IsMagicImmune()))
							return;
						var range = DagonRange[dagon.Level - 1];
						var damage = Math.Floor(DagonRange[dagon.Level - 1] * (1 - i.MagicDamageResist));
						if (me.Distance2D(i) < range && i.Health < damage)
							dagon.UseAbility(i);
					}
				}
			}
		}
	}
}
