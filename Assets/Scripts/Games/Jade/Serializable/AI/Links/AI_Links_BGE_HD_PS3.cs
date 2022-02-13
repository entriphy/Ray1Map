﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.Jade {
	public class AI_Links_BGE_HD_PS3 : AI_Links_BGE_HD_Xbox360 {
		protected override void InitFunctionDefs() {
			base.InitFunctionDefs();

			#region Function Defs (Unnamed)
			uint[] overrides = new uint[] {
				0x2C04581A,
				0x2C04581B,
				0x2C04581C,
				0x2C04581D,
			};
			#endregion

			HashSet<uint> fdLookup = new HashSet<uint>(FunctionDefs.Select(fd => fd.Key));
			List<uint> addedKeys = new List<uint>();
			foreach (var u in overrides) {
				if(fdLookup.Contains(u)) continue;
				addedKeys.Add(u);
			}
			if (addedKeys.Any()) {
				var fdefs = FunctionDefs;
				int len = fdefs.Length;
				Array.Resize(ref fdefs, len + addedKeys.Count);
				for (int i = 0; i < addedKeys.Count; i++) {
					fdefs[len + i] = new AI_FunctionDef(addedKeys[i], $"Custom_{addedKeys[i]:X8}");
				}
				FunctionDefs = fdefs;
			}
		}
	}
}
