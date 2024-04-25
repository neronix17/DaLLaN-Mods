using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MechalitCore
{
	public class HediffCompProperties_PartHitPoints : HediffCompProperties
	{
		public float multiplier;

		public HediffCompProperties_PartHitPoints()
		{
			compClass = typeof(HediffComp_PartHitPoints);
		}
	}
}
