using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axescript
{
	class Program
	{
		private static readonly Menu Menu = new Menu("Axe Blink Ulti", "axescript", true);
		private const int blinkRadius = 1180;
		static void Main(string[] args)
		{
			Menu.AddItem(new MenuItem("blink", "Use Blink").SetValue(true));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
		}
		private static void Game_OnUpdate(EventArgs args)
		{
			if (!Game.IsInGame)
			{
				return;
			}
			Hero me = ObjectMgr.LocalHero;
			if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Axe)
			{
				return;
			}
			Item blink = me.FindItem("item_blink");
			Ability ult = me.Spellbook.SpellR;
			Hero target = null;
			if (ult != null && ult.Level > 0)
			{
				target = GetLowHpHeroInDistance(me, blinkRadius);
				if (Menu.Item("blink").GetValue<bool>())
				{
					if (target != null && me.Health > 400 && blink != null && blink.CanBeCasted() && Utils.SleepCheck("blink"))
					{
						if (!useAbility(blink, "blink", target, true, me))
						{
							return;
						}
					}
				}
				target = GetLowHpHeroInDistance(me, 400);
				if (target != null && ult != null && (ult.Level > 0) && ult.CanBeCasted() && Utils.SleepCheck("ult"))
				{
					if (!useAbility(ult, "ult", target, false, me))
					{
						return;
					}
					else
					{
						return;
					}
				}
			}
		}
		private static bool useAbility(Ability ability, string codeWord, Hero target, bool isPos, Hero me)
		{
			if (ability == null)
			{
				return true;
			}
			if (ability.CanBeCasted() && !ability.IsInAbilityPhase && Utils.SleepCheck(codeWord))
			{
				if (target != null)
				{
					if (isPos)
					{
						ability.UseAbility(target.Position);
					}
					else
					{
						ability.UseAbility(target);
					}
				}
				else
				{
					ability.UseAbility();
				}
				Utils.Sleep(ability.GetCastDelay(me, target, true) * 1000, codeWord);
				if (ability.CanBeCasted())
				{
					return false;
				}
			}
			return true;
			}
		private static float calculateDistance(Hero me, Hero target)
		{
			var pos = target.Position;
			var mePosition = me.Position;
			return mePosition.Distance2D(pos);
		}
		private static Hero GetLowHpHeroInDistance(Hero me, float maxDistance)
		{
			Item aghanim = me.FindItem("item_ultimate_scepter");
			int[] ultDamage;
			if (aghanim != null)
			{
				ultDamage = new int[3] { 300, 425, 550 };
			}
			else
			{
				ultDamage = new int[3] { 250, 325, 400 };
			}
			var ultLevel = me.Spellbook.SpellR.Level;
			var damage = ultDamage[ultLevel - 1];
			var enemies = ObjectMgr.GetEntities<Hero>()
			.Where(x => x.IsAlive && !x.IsIllusion && x.Team != me.Team && (damage > (x.Health - 5))).ToList();
			Hero target = getHeroInDistance(me, enemies, maxDistance);
			return target;
			}
		private static Hero getHeroInDistance(Hero me, List<Hero> enemies, float maxDistance)
		{
			Hero target = null;
			float minDistance = maxDistance;
			foreach (var hero in enemies)
			{
				var distance = me.Distance2D(hero);
				if (distance <= maxDistance && distance <= minDistance)
				{
					minDistance = distance;
					target = hero;
				}
			}
			return target;
		}
	}
}
