using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.LV;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Ray1Map;
using Loader = BinarySerializer.Klonoa.LV.Loader;

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
        
        public abstract KlonoaSettings_LV GetKlonoaSettings(GameSettings settings);
        
        public HeadPack_ArchiveFile Load_Headpack(Context context, KlonoaSettings_LV settings)
        {
            return FileFactory.Read<HeadPack_ArchiveFile>(context, settings.FilePath_HEAD, 
                (_, head) => head.Pre_HasMultipleLanguages = settings.HasMultipleLanguages);
        }
        
        #endregion
        
        #region Game Actions
        
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                // TODO: Add game actions
            };
        }
        
        #endregion
        
        #region Load
        
        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get settings
            GameSettings settings = context.GetR1Settings();
            int lev = settings.World;
            int sector = settings.Level;
            KlonoaSettings_LV config = GetKlonoaSettings(settings);

            // Create the level
            var level = new Unity_Level()
            {
                CellSize = 16,
                FramesPerSecond = 60,
                StartIn3D = true,
                StartPosition = new Vector3(20, 20, 20), // TODO: Find appropriate start position
                IsometricData = new Unity_IsometricData
                {
                    CollisionMapWidth = 0,
                    CollisionMapHeight = 0,
                    TilesWidth = 0,
                    TilesHeight = 0,
                    CollisionMap = null,
                    ViewAngle = Quaternion.Euler(90, 0, 0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * 1
                },
            };
            
            // Add files to context
            await context.AddLinearFileAsync(config.FilePath_HEAD);
            await context.AddLinearFileAsync(config.GetFilePath(Loader.BINType.KL));
            context.AddKlonoaSettings(config);
            
            // Load HEADPACK.BIN and create loader
            Controller.DetailedState = "Loading HEADPACK.BIN";
            await Controller.WaitIfNecessary();
            HeadPack_ArchiveFile headpack = Load_Headpack(context, config);
            Loader loader = Loader.Create(context, headpack);

            // Load level packs
            Controller.DetailedState = "Loading preload data";
            await Controller.WaitIfNecessary();
            var preloadPack = loader.LoadBINFile<LevelPreloadPack_ArchiveFile>(Loader.BINType.KL, lev * 2);
                
            Controller.DetailedState = "Loading level data";
            await Controller.WaitIfNecessary();
            var levelPack = loader.LoadBINFile<LevelDataPack_ArchiveFile>(Loader.BINType.KL, lev * 2 + 1);
            if (sector < levelPack.MiscAssets.SectorConfigs.Files.Length)
                level.CameraClear = new Unity_CameraClear(levelPack.MiscAssets.SectorConfigs.Files[sector].FogColor?.GetColor() 
                                                          ?? Color.black);
            
            // Create object manager
            Unity_ObjectManager_PS2Klonoa_LV objManager = new Unity_ObjectManager_PS2Klonoa_LV(context);
            level.ObjManager = objManager;
            
            // Load layers
            // TODO: Create loader classes
            await Load_Layers(loader, level, levelPack, sector);

            return level;
        }

        public async UniTask Load_Layers(Loader loader, Unity_Level level, LevelDataPack_ArchiveFile levelPack, int sector)
        {
            List<Unity_Layer> layers = new List<Unity_Layer>();
            var sectorPack = levelPack.CommonAssets.SectorData.Sectors[sector];
            var parentObj = Controller.obj.levelController.editor.layerTiles;
            
            layers.Add(await Load_Layers_LevelObject(loader, parentObj, sectorPack));
            layers.AddRange(await Load_Layers_Background(loader, parentObj, levelPack, sector));

            level.Layers = layers.ToArray();
        }

        public async UniTask<Unity_Layer> Load_Layers_LevelObject(Loader loader, GameObject parent, LevelSector_ArchiveFile sectorPack)
        {
            Controller.DetailedState = "Loading level textures";
            await Controller.WaitIfNecessary();
            var texturesFile = sectorPack.Textures;
            var textures = texturesFile.GetTextures();
            
            Controller.DetailedState = "Loading level geometry";
            await Controller.WaitIfNecessary();
            var geometry = sectorPack.Geometry;
            
            var vifGeometryGameObj = new Klonoa2VIFGameObject(geometry, textures, loader.Context);
            GameObject obj = vifGeometryGameObj.CreateGameObject("Map");
            // Bounds levelBounds = new Bounds();
            var layerDimensions = new Rect(0, 0, 1, 1); // TODO: Calculate layer dimensions properly 

            obj.transform.SetParent(parent.transform, false);
            const float scale = 1 / 32f;
            obj.transform.localScale = new Vector3(scale, scale, scale);

            return new Unity_Layer_GameObject(true)
            {
                Name = "Map",
                ShortName = "MAP",
                Graphics = obj,
                Dimensions = layerDimensions,
                DisableGraphicsWhenCollisionIsActive = true
            };
        }

        public async UniTask<List<Unity_Layer>> Load_Layers_Background(Loader loader, GameObject parent, LevelDataPack_ArchiveFile levelPack, int sector)
        {
            var backgroundLayers = new List<Unity_Layer>();

            for (int i = 0; i < 6; i++)
            {
                var geometry = levelPack.MiscAssets.ScriptData.Files[sector].BackgroundGeometry[i];
                if (geometry == null)
                    continue;
                var texturesDict = new Dictionary<int, Texture2D>();
                foreach (var textures in levelPack.MiscAssets.BackgroundTextures.Textures[sector].Textures)
                {
                    if (textures == null)
                        continue;
                    textures.GetTextures().ToList().ForEach(x => texturesDict.Add(x.Key, x.Value));
                }

                var vifGeometryGameObj = new Klonoa2VIFGameObject(geometry, texturesDict, loader.Context);
                GameObject obj = vifGeometryGameObj.CreateGameObject("Background " + i);
                // Bounds levelBounds = new Bounds();
                var layerDimensions = new Rect(0, 0, 1, 1); // TODO: Calculate layer dimensions properly 

                obj.transform.SetParent(parent.transform, false);
                // TODO: Find correct scale value
                float scale = 1.5f - 0.0005f * i; // Slightly scale each background to prevent z-fighting
                obj.transform.localScale = new Vector3(scale, scale, scale);

                backgroundLayers.Add(new Unity_Layer_GameObject(true)
                {
                    Name = "Background " + i,
                    ShortName = "BG" + i,
                    Graphics = obj,
                    Dimensions = layerDimensions,
                    DisableGraphicsWhenCollisionIsActive = true
                });
            }

            return backgroundLayers;
        }
        
        #endregion
    }
}