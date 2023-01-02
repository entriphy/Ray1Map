using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.LV;
using Cysharp.Threading.Tasks;
using Games.PS2Klonoa;
using UnityEngine;
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
            var sectorPack = levelPack.CommonAssets.SectorData.Sectors[sector];
            if (sector < levelPack.MiscAssets.SectorConfigs.Files.Length)
                level.CameraClear = new Unity_CameraClear(levelPack.MiscAssets.SectorConfigs.Files[sector].FogColor?.GetColor() 
                                                          ?? Color.black);
            
            // Create object manager
            Unity_ObjectManager_PS2Klonoa_LV objManager = new Unity_ObjectManager_PS2Klonoa_LV(context);
            level.ObjManager = objManager;
            
            // Load layers
            // TODO: Create loader classes
            Controller.DetailedState = "Loading level textures";
            await Controller.WaitIfNecessary();
            var texturesFile = sectorPack.Textures;
            var textures = texturesFile.GetTextures();
            
            Controller.DetailedState = "Loading level geometry";
            await Controller.WaitIfNecessary();
            var geometryFile = sectorPack.Geometry;
            level.Layers = new[]
            {
                Load_Layers_LevelObject(loader, Controller.obj.levelController.editor.layerTiles, geometryFile, textures)
            };
            
            return level;
        }

        public Unity_Layer Load_Layers_LevelObject(Loader loader, GameObject parent, VIFGeometry_File geometry, Dictionary<int, Texture2D> textures)
        {
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
        
        #endregion
    }
}