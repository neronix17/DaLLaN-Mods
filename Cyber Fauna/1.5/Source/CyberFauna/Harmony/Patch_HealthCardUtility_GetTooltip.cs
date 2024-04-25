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
	[HarmonyPatch(typeof(HealthCardUtility))]
	[HarmonyPatch("GetTooltip")]
	public class Patch_HealthCardUtility_GetTooltip
	{
		[HarmonyPrefix]
		public static bool Prefix(ref Pawn pawn, BodyPartRecord part, ref string __result)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(part.LabelCap + ": ");
			float maxHealth = part.def.GetMaxHealth(pawn);
			maxHealth = Mathf.RoundToInt(maxHealth);
			foreach (Hediff item in pawn.health.hediffSet.hediffs.Where((Hediff x) => x.Part == part))
			{
				HediffComp_PartHitPoints hediffComp_PartHitPoints = item.TryGetComp<HediffComp_PartHitPoints>();
				if (hediffComp_PartHitPoints != null)
				{
					maxHealth *= hediffComp_PartHitPoints.Props.multiplier;
				}
			}
			stringBuilder.AppendLine(string.Concat(str3: ((float)Mathf.RoundToInt(maxHealth)).ToString(), str0: " ", str1: pawn.health.hediffSet.GetPartHealth(part).ToString(), str2: " / "));
			float num = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part);
			if (num != 1f)
			{
				stringBuilder.AppendLine("Efficiency".Translate() + ": " + num.ToStringPercent());
			}
			__result = stringBuilder.ToString();
			return false;
		}
	}
}
