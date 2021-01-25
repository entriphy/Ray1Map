﻿using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
{
	public class Gameloft_RK_Manager : Gameloft_BaseManager {
		public override string[] ResourceFiles => new string[] {
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"10",
			"11",
			"12",
			"13",
			"14",
			"15",
			"16",
			"17",
			"18",
			"19",
			"20",
		};

		public virtual string GetLevelPath(GameSettings settings) => "20";
		public virtual int GetLevelResourceIndex(GameSettings settings) => settings.Level;

		public override string[] SingleResourceFiles => new string[] {
			"s"
		};

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 8).ToArray()),
		});

		public override async UniTask LoadFilesAsync(Context context) {
			await context.AddLinearSerializedFileAsync(GetLevelPath(context.Settings));
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			await UniTask.CompletedTask;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.Settings), context);
			var ind = GetLevelResourceIndex(context.Settings) * 2;
			var level1 = resf.SerializeResource<Gameloft_Level3D>(context.Deserializer, default, ind, name: $"Level_{ind}");
			var level2 = resf.SerializeResource<Gameloft_Level3D>(context.Deserializer, default, ind+1, name: $"Level_{ind+1}");

			throw new NotImplementedException();
		}
	}
}