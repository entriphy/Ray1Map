﻿using System;
using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEX_GlobalList {
		public List<Jade_TextureReference> Textures { get; set; }
		public List<Jade_PaletteReference> Palettes { get; set; }

		private Dictionary<Jade_Key, List<Jade_PaletteReference>> KeyPaletteDictionary { get; set; }
		private Dictionary<Jade_Key, List<Jade_TextureReference>> KeyTextureDictionary { get; set; }

		public void AddTexture(Jade_TextureReference tex) {
			if (tex == null || tex.IsNull) return;
			if (Textures == null) Textures = new List<Jade_TextureReference>();
			if (KeyTextureDictionary == null) KeyTextureDictionary = new Dictionary<Jade_Key, List<Jade_TextureReference>>();
			if (Textures.FindItem(t => t.Key == tex.Key) == null) Textures.Add(tex);
			if (!KeyTextureDictionary.ContainsKey(tex.Key)) KeyTextureDictionary[tex.Key] = new List<Jade_TextureReference>();
			KeyTextureDictionary[tex.Key].Add(tex);
		}
		public void AddPalette(Jade_PaletteReference pal) {
			if (pal == null || pal.IsNull) return;
			if (Palettes == null) Palettes = new List<Jade_PaletteReference>();
			if (KeyPaletteDictionary == null) KeyPaletteDictionary = new Dictionary<Jade_Key, List<Jade_PaletteReference>>();
			if (Palettes.FindItem(t => t.Key == pal.Key) == null) Palettes.Add(pal);
			if (!KeyPaletteDictionary.ContainsKey(pal.Key)) KeyPaletteDictionary[pal.Key] = new List<Jade_PaletteReference>();
			KeyPaletteDictionary[pal.Key].Add(pal);
		}

		public void FillInReferences() {
			foreach (var t in Textures) {
				var list = KeyTextureDictionary[t.Key];
				for (int i = 1; i < list.Count; i++) {
					list[i].Info = t.Info;
					list[i].Content = t.Content;
				}
			}
			foreach(var p in Palettes) {
				var list = KeyPaletteDictionary[p.Key];
				for (int i = 1; i < list.Count; i++) {
					list[i].Value = p.Value;
				}
			}
		}
	}
}
