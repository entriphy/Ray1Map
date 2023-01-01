using System;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map;

namespace Ray1Map.PS2Klonoa
{
    public abstract class PS2Klonoa_LV_BaseManager : BaseGameManager
    {
        #region Manager
        public override GameInfo_Volume[] GetLevels(GameSettings settings) =>
            GameInfo_Volume.SingleVolume(Levels.Select((x, i) =>
                new GameInfo_World(x.ID, x.Name, Enumerable.Range(0, x.SectorCount).ToArray())).ToArray());

        public virtual (string Name, int ID, int SectorCount)[] Levels => new (string Name, int ID, int SectorCount)[]
        {
            ("Sea of Tears",          1, 7),
            ("La-Lakoosha",           2, 8),
            // 3: Empty              
            ("Joilant Fun Park",      4, 13),
            ("Jungle Slider",         5, 6),
            ("Underground Factory",   6, 10),
            ("Volk City",             7, 7),
            ("Volkan Inferno",        8, 7),
            ("Ishras Ark",            9, 9),
            ("Mts. of Mira-Mira",    10, 5),
            ("Maze of Memories",     11, 18),
            ("Noxious La-Lakoosha",  12, 9),
            ("Dark Sea of Tears",    13, 7),
            ("Empty Sea of Tears",   14, 5),
            ("The Ark Revisited",    15, 9),
            ("Kingdom of Sorrow",    16, 11),
            ("The Forgotten Path",   17, 3),
            ("Chamber o' Fun",       18, 6),
            ("Chamber o' Horrors",   19, 9),
            // 20: Empty
            ("Boss: Folgaran",       21, 1),
            ("Boss: Leptio",         22, 3),
            ("Boss: Biskarsh",       23, 2),
            ("Boss: Polonte",        24, 1),
            // 25: Empty
            ("Boss: Cursed Leorina", 26, 2),
            ("Boss: King of Sorrow", 27, 3),
        };

        public override UniTask<Unity_Level> LoadAsync(Context context)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}