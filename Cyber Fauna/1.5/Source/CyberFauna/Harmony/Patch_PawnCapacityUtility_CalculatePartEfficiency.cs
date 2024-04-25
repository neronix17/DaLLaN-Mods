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
	[HarmonyPatch(typeof(PawnCapacityUtility))]
	[HarmonyPatch("CalculatePartEfficiency")]
	public class Patch_PawnCapacityUtility_CalculatePartEfficiency
	{
		[HarmonyPrefix]
		public static bool Prefix(ref HediffSet diffSet, BodyPartRecord part, ref bool ignoreAddedParts, ref List<PawnCapacityUtility.CapacityImpactor> impactors, ref float __result)
		{
			for (BodyPartRecord parent = part.parent; parent != null; parent = parent.parent)
			{
				if (diffSet.HasDirectlyAddedPartFor(parent))
				{
					Hediff_AddedPart firstHediffMatchingPart = diffSet.GetFirstHediffMatchingPart<Hediff_AddedPart>(parent);
					impactors?.Add(new PawnCapacityUtility.CapacityImpactorHediff
					{
						hediff = firstHediffMatchingPart
					});
					__result = firstHediffMatchingPart.def.addedPartProps.partEfficiency;
					return false;
				}
			}
			if (part.parent != null && diffSet.PartIsMissing(part.parent))
			{
				__result = 0f;
				return false;
			}
			float num = 1f;
			if (!ignoreAddedParts)
			{
				for (int i = 0; i < diffSet.hediffs.Count; i++)
				{
					if (diffSet.hediffs[i] is Hediff_AddedPart hediff_AddedPart && hediff_AddedPart.Part == part)
					{
						num *= hediff_AddedPart.def.addedPartProps.partEfficiency;
						if (hediff_AddedPart.def.addedPartProps.partEfficiency != 1f)
						{
							impactors?.Add(new PawnCapacityUtility.CapacityImpactorHediff
							{
								hediff = hediff_AddedPart
							});
						}
					}
				}
			}
			float b = -1f;
			float num2 = 0f;
			bool flag = false;
			for (int j = 0; j < diffSet.hediffs.Count; j++)
			{
				if (diffSet.hediffs[j].Part == part && diffSet.hediffs[j].CurStage != null)
				{
					HediffStage curStage = diffSet.hediffs[j].CurStage;
					num2 += curStage.partEfficiencyOffset;
					flag |= curStage.partIgnoreMissingHP;
					if (curStage.partEfficiencyOffset != 0f && curStage.becomeVisible)
					{
						impactors?.Add(new PawnCapacityUtility.CapacityImpactorHediff
						{
							hediff = diffSet.hediffs[j]
						});
					}
				}
			}
			if (!flag)
			{
				float maxHealth = part.def.GetMaxHealth(diffSet.pawn);
				maxHealth = Mathf.RoundToInt(maxHealth);
				foreach (Hediff item in diffSet.pawn.health.hediffSet.hediffs.Where((Hediff x) => x.Part == part))
				{
					HediffComp_PartHitPoints hediffComp_PartHitPoints = item.TryGetComp<HediffComp_PartHitPoints>();
					if (hediffComp_PartHitPoints != null)
					{
						maxHealth *= hediffComp_PartHitPoints.Props.multiplier;
					}
				}
				float num3 = diffSet.GetPartHealth(part) / maxHealth;
				if (num3 != 1f)
				{
					if (DamageWorker_AddInjury.ShouldReduceDamageToPreservePart(part))
					{
						num3 = Mathf.InverseLerp(0.1f, 1f, num3);
					}
					impactors?.Add(new PawnCapacityUtility.CapacityImpactorBodyPartHealth
					{
						bodyPart = part
					});
					num *= num3;
				}
			}
			num += num2;
			if (num > 0.0001f)
			{
				num = Mathf.Max(num, b);
			}
			__result = Mathf.Max(num, 0f);
			return false;
		}
	}
}
