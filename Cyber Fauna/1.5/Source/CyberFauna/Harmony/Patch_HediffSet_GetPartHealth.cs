using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CyberFauna
{
	[HarmonyPatch(typeof(HediffSet))]
	[HarmonyPatch("GetPartHealth")]
	public class Patch_HediffSet_GetPartHealth
	{
		[HarmonyPrefix]
		public static bool Prefix(ref HediffSet __instance, ref BodyPartRecord part, ref float __result)
		{
			if (part == null)
			{
				__result = 0f;
			}
			else
			{
				float num = part.def.GetMaxHealth(__instance.pawn);
				for (int i = 0; i < __instance.hediffs.Count; i++)
				{
					if (__instance.hediffs[i] is Hediff_MissingPart && __instance.hediffs[i].Part == part)
					{
						__result = 0f;
						return false;
					}
					if (__instance.hediffs[i].Part == part)
					{
						HediffComp_PartHitPoints hediffComp_PartHitPoints = __instance.hediffs[i].TryGetComp<HediffComp_PartHitPoints>();
						if (hediffComp_PartHitPoints != null)
						{
							num *= hediffComp_PartHitPoints.Props.multiplier;
						}
						if (__instance.hediffs[i] is Hediff_Injury hediff_Injury)
						{
							num -= hediff_Injury.Severity;
						}
					}
				}
				num = Mathf.Max(num, 0f);
				if (!part.def.destroyableByDamage)
				{
					num = Mathf.Max(num, 1f);
				}
				__result = Mathf.RoundToInt(num);
			}
			return false;
		}
	}
}
