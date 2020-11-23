﻿using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class GBAIsometric_Spyro_Manager : IGameManager
    {
        public const int CellSize = 8;

        public abstract GameInfo_Volume[] GetLevels(GameSettings settings);

        public virtual string GetROMFilePath => $"ROM.gba";

        public abstract int DataTableCount { get; }
        public abstract int PortraitsCount { get; }
        public abstract int DialogCount { get; }
        public abstract int PrimaryLevelCount { get; }
        public abstract int LevelMapsCount { get; }
        public abstract int TotalLevelsCount { get; }
        public abstract int ObjectTypesCount { get; }
        public abstract int AnimSetsCount { get; }
        public abstract int LevelDataCount { get; }
        public abstract int MenuPageCount { get; }

        public abstract IEnumerable<string> GetLanguages { get; }

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Data Blocks", false, true, (input, output) => ExportDataBlocksAsync(settings, output, false)),
            new GameAction("Export Data Blocks (categorized)", false, true, (input, output) => ExportDataBlocksAsync(settings, output, true)),
            new GameAction("Export Assets", false, true, (input, output) => ExportAssetsAsync(settings, output)),
            new GameAction("Export Cutscenes", false, true, (input, output) => ExportCutscenes(settings, output)),
        };

        public async UniTask ExportDataBlocksAsync(GameSettings settings, string outputPath, bool categorize) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

                var palette = Util.CreateDummyPalette(16).Select(x => x.GetColor()).ToArray();

                for (int i = 0; i < rom.DataTable.DataEntries.Length; i++)
                {
                    var length = rom.DataTable.DataEntries[i].DataLength;

                    if (categorize && length == 512)
                    {
                        var pal = rom.DataTable.DoAtBlock(context, i, size => s.SerializeObjectArray<ARGB1555Color>(default, 256, name: $"Pal[{i}]"));

                        PaletteHelpers.ExportPalette(Path.Combine(outputPath, "Palettes", $"{i:000}_0x{rom.DataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.png"), pal, optionalWrap: 16);
                    }
                    else
                    {
                        var data = rom.DataTable.DoAtBlock(context, i, size => s.SerializeArray<byte>(default, size, name: $"Block[{i}]"));

                        if (categorize && length % 32 == 0)
                        {
                            var tex = Util.ToTileSetTexture(data, palette, false, CellSize, true, wrap: 32);
                            Util.ByteArrayToFile(Path.Combine(outputPath, "ObjTileSets", $"{i:000}_0x{rom.DataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());
                        }
                        else
                        {
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{i:000}_0x{rom.DataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.dat"), data);
                        }
                    }
                }
            }
        }

        public async UniTask ExportCutscenes(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

                var langIndex = 0;

                foreach (var lang in GetLanguages)
                {
                    using (var w = new StreamWriter(Path.Combine(outputPath, $"Cutscenes_{lang}.txt")))
                    {
                        foreach (var d in rom.DialogEntries.OrderBy(x => x.ID))
                        {
                            var data = d.DialogData;

                            w.WriteLine($"# Cutscene {d.ID} (0x{d.Offset.AbsoluteOffset:X8})");

                            foreach (var e in data.Entries)
                            {
                                switch (e.Values.First().Instruction)
                                {
                                    case GBAIsometric_Spyro_DialogData.Instruction.DrawPortrait:
                                        w.WriteLine($"[Draw portrait {e.PortraitIndex}]");
                                        break;

                                    case GBAIsometric_Spyro_DialogData.Instruction.DrawText:
                                    case GBAIsometric_Spyro_DialogData.Instruction.DrawMultiChoiceText:
                                        w.WriteLine($"{String.Join(" ", e.LocIndices.Select(x => x.GetString(langIndex)))}");
                                        break;

                                    case GBAIsometric_Spyro_DialogData.Instruction.MoveCamera:
                                        w.WriteLine("[Move camera]");
                                        break;
                                }

                                if (e.Values.First().Instruction == GBAIsometric_Spyro_DialogData.Instruction.DrawMultiChoiceText)
                                {
                                    w.WriteLine($"  > {e.MultiChoiceLocIndices[0].GetString(langIndex)} > {e.MultiChoiceLocIndices[2].GetString(langIndex)}");
                                    w.WriteLine($"  > {e.MultiChoiceLocIndices[1].GetString(langIndex)} > {e.MultiChoiceLocIndices[3].GetString(langIndex)}");
                                }
                            }

                            w.WriteLine();
                        }
                    }

                    if (rom.MenuPages != null)
                    {
                        using (var w = new StreamWriter(Path.Combine(outputPath, $"Menus_{lang}.txt")))
                        {
                            var menuIndex = 0;

                            foreach (var d in rom.MenuPages)
                            {
                                w.WriteLine($"# Menu {menuIndex} (0x{d.Offset.AbsoluteOffset:X8})");

                                var header = d.Header.GetString(langIndex);
                                var subHeader = d.SubHeader.GetString(langIndex);

                                if (header != null)
                                    w.WriteLine($"{header}");
                                if (subHeader != null)
                                    w.WriteLine($"{subHeader}");

                                foreach (var o in d.Options ?? new GBAIsometric_Spyro_MenuOption[0])
                                    w.WriteLine($"> {o.LocIndex.GetString(langIndex)}");

                                w.WriteLine();

                                menuIndex++;
                            }
                        }
                    }

                    langIndex++;
                }
            }
        }

        public async UniTask ExportAssetsAsync(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                await LoadFilesAsync(context);

                var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

                foreach (var portrait in rom.PortraitSprites ?? new GBAIsometric_Spyro_PortraitSprite[0])
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Portraits", $"{portrait.ID}.png"), portrait.ToTexture2D().EncodeToPNG());

                var index = 0;
                foreach (var cutscene in rom.CutsceneMaps ?? new GBAIsometric_Spyro_CutsceneMap[0])
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Cutscenes", $"{index++}.png"), cutscene.ToTexture2D().EncodeToPNG());

                foreach (var map in rom.LevelMaps ?? new GBAIsometric_Spyro_LevelMap[0])
                    Util.ByteArrayToFile(Path.Combine(outputPath, "Maps", $"{map.LevelID}.png"), map.ToTexture2D().EncodeToPNG());

                var objPal = rom.GetLevelData(settings).ObjPalette;

                if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                {
                    var lvlPal = objPal;
                    objPal = rom.CommonPalette;

                    for (int i = 0; i < 256; i++)
                    {
                        if (lvlPal[i].Color1555 != 0)
                            objPal[i] = lvlPal[i];
                    }
                }

                // Export animation sets
                for (var i = 0; i < rom.AnimSets.Length; i++)
                {
                    var animSet = rom.AnimSets[i];
                    var outPath = Path.Combine(outputPath, "AnimSets", $"{i}");
                    var pal = Util.ConvertAndSplitGBAPalette(objPal);

                    await ExportAnimSetAsync(outPath, animSet, pal);
                }
            }
        }

        public async UniTask ExportAnimSetAsync(string outputPath, GBAIsometric_Spyro_AnimSet animSet, Color[][] pal)
        {
            if (animSet == null)
                return;

            for (int a = 0; a < animSet.AnimBlock.Animations.Length; a++)
            {
                if (a % 10 == 0)
                    await Controller.WaitIfNecessary();

                var f = 0;

                var anim = animSet.AnimBlock.Animations[a];

                foreach (var tex in GetAnimationFrames(animSet, anim, pal, isExport: true))
                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{a}-{anim.AnimSpeed}", $"{f++}.png"), tex.EncodeToPNG());
            }
        }

        private Unity_IsometricCollisionTile GetCollisionTile(Context context, GBAIsometric_TileCollision block)
        {
            Unity_IsometricCollisionTile.AdditionalTypeFlags GetAddType()
            {
                Unity_IsometricCollisionTile.AdditionalTypeFlags addType = Unity_IsometricCollisionTile.AdditionalTypeFlags.None;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceUpLeft))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceUpLeft;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceUpRight))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceUpRight;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceDownLeft))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceDownLeft;

                if (block.AddType_Spyro.HasFlag(GBAIsometric_TileCollision.AdditionalTypeFlags_Spyro.FenceDownRight))
                    addType |= Unity_IsometricCollisionTile.AdditionalTypeFlags.FenceDownRight;

                return addType;
            }
            Unity_IsometricCollisionTile.ShapeType GetShapeType()
            {
                switch (block.Shape_Spyro)
                {
                    case GBAIsometric_TileCollision.ShapeType_Spyro.None:
                        return Unity_IsometricCollisionTile.ShapeType.None;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.Normal:
                        return Unity_IsometricCollisionTile.ShapeType.Normal;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.SlopeUpLeft:
                        return Unity_IsometricCollisionTile.ShapeType.SlopeUpLeft;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.SlopeUpRight:
                        return Unity_IsometricCollisionTile.ShapeType.SlopeUpRight;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeTop:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeTop;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeBottom:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeBottom;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeLeft:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeLeft;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.LevelEdgeRight:
                        return Unity_IsometricCollisionTile.ShapeType.LevelEdgeRight;
                    case GBAIsometric_TileCollision.ShapeType_Spyro.OutOfBounds:
                        return Unity_IsometricCollisionTile.ShapeType.OutOfBounds;
                    default:
                        return Unity_IsometricCollisionTile.ShapeType.Unknown;
                }
            }
            Unity_IsometricCollisionTile.CollisionType GetCollisionType()
            {
                switch (block.Type_Spyro)
                {
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Solid:
                        return Unity_IsometricCollisionTile.CollisionType.Solid;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Water:
                        return Unity_IsometricCollisionTile.CollisionType.Water;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.FreezableWater:
                        return Unity_IsometricCollisionTile.CollisionType.FreezableWater;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Wall:
                        return Unity_IsometricCollisionTile.CollisionType.Wall;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Lava:
                        return Unity_IsometricCollisionTile.CollisionType.Lava;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Pit:
                        return Unity_IsometricCollisionTile.CollisionType.Pit;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.HubworldPit:
                        return Unity_IsometricCollisionTile.CollisionType.HubworldPit;
                    case GBAIsometric_TileCollision.CollisionType_Spyro.Trigger:
                        return Unity_IsometricCollisionTile.CollisionType.Trigger;
                    default:
                        return Unity_IsometricCollisionTile.CollisionType.Unknown;
                }
            }
            return new Unity_IsometricCollisionTile()
            {
                Height = block.Height,
                AddType = GetAddType(),
                Shape = GetShapeType(),
                Type = GetCollisionType(),
                DebugText = $"Depth:{block.Depth} HeightFlags:{block.HeightFlags} UnkSpyro:{block.Unk_Spyro:X1} Shape:{block.Shape_Spyro} Type:{block.Type_Spyro} Add:{block.AddType_Spyro}"
            };
        }

        public Unity_IsometricData GetIsometricData(Context context, GBAIsometric_Spyro_Collision3DMapData collisionData, int width, int height, int groupWidth, int groupHeight)
        {
            return new Unity_IsometricData()
            {
                CollisionWidth = collisionData.Width,
                CollisionHeight = collisionData.Height,
                TilesWidth = width * groupWidth,
                TilesHeight = height * groupHeight,
                Collision = collisionData.Collision.Select(c => GetCollisionTile(context, c)).ToArray(),
                Scale = new Vector3(Mathf.Sqrt(8), 1f/Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sqrt(8)) // Height = 1.15 tiles, Length of the diagonal of 1 block = 8 tiles
            };
        }

        public async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAIsometric_Spyro_ROM>(GetROMFilePath, context);

            Func<byte, Unity_MapCollisionTypeGraphic> collGraphicFunc = x => ((GBAIsometric_Spyro3_TileCollisionType2D)x).GetCollisionTypeGraphic();
            Func<byte, string> collNameFunc = x => ((GBAIsometric_Spyro3_TileCollisionType2D)x).ToString();
            if (context.Settings.EngineVersion < EngineVersion.GBAIsometric_Spyro3) {
                collGraphicFunc = x => ((GBAIsometric_Spyro2_TileCollisionType2D)x).GetCollisionTypeGraphic();
                collNameFunc = x => ((GBAIsometric_Spyro2_TileCollisionType2D)x).ToString();
            }

            // Spyro 2 cutscenes
            if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2 && context.Settings.World == 4)
            {
                var cutsceneMap = rom.CutsceneMaps[context.Settings.Level];

                Controller.DetailedState = $"Loading tileset";
                await Controller.WaitIfNecessary();

                var fullTileSet = cutsceneMap.TileSets.SelectMany(x => x).ToArray();
                var cutsceneTileSet = LoadTileSet(cutsceneMap.Palette, fullTileSet);

                Controller.DetailedState = $"Loading maps";
                await Controller.WaitIfNecessary();

                var map = new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics,
                    Width = cutsceneMap.Map.Width,
                    Height = cutsceneMap.Map.Height,
                    TileSet = new Unity_MapTileMap[]
                    {
                        cutsceneTileSet
                    },
                    MapTiles = cutsceneMap.Map.MapData.Select(x => new Unity_Tile(x)).ToArray(),
                };

                Controller.DetailedState = $"Loading localization";
                await Controller.WaitIfNecessary();

                return new Unity_Level(
                    maps: new Unity_Map[]
                    {
                        map
                    },
                    objManager: new Unity_ObjectManager(context),
                    eventData: new List<Unity_Object>(),
                    cellSize: CellSize,
                    getCollisionTypeNameFunc: collNameFunc,
                    getCollisionTypeGraphicFunc: collGraphicFunc,
                    localization: LoadLocalization(context, rom)) { CellSizeOverrideCollision = context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3 ? (int?)16 : null };
            }

            var levelData = rom.GetLevelData(context.Settings);

            // Convert map arrays to map tiles
            Dictionary<GBAIsometric_Spyro_MapLayer, MapTile[]> mapTiles = levelData.MapLayers.Where(x => x != null).ToDictionary(x => x, GetMapTiles);

            Controller.DetailedState = $"Loading tileset";
            await Controller.WaitIfNecessary();

            // Load tileset
            var tileSet = LoadTileSet(levelData.TilePalette, levelData.MapLayers.Where(x => x != null).Select(x => x.TileSet).ToArray(), mapTiles);

            Controller.DetailedState = $"Loading collision";
            await Controller.WaitIfNecessary();

            var firstValidMap = levelData.MapLayers.First(x => x != null);
            Unity_IsometricData isometricData = levelData.Collision3D == null ? null : GetIsometricData(context, levelData.Collision3D, firstValidMap.Map.Width, firstValidMap.Map.Height, firstValidMap.TileAssemble.GroupWidth, firstValidMap.TileAssemble.GroupHeight);

            Controller.DetailedState = $"Loading maps";
            await Controller.WaitIfNecessary();

            var maps = levelData.MapLayers.Select(x => x).Select((map, i) =>
            {
                if (map == null)
                    return null;

                var width = map.Map.Width * map.TileAssemble.GroupWidth;
                var height = map.Map.Height * map.TileAssemble.GroupHeight;

                return new Unity_Map() {
                    Type = Unity_Map.MapType.Graphics,
                    Width = (ushort)width,
                    Height = (ushort)height,
                    TileSet = new Unity_MapTileMap[]
                    {
                        tileSet
                    },
                    MapTiles = mapTiles[map].Select(x => new Unity_Tile(x)).ToArray(),
                };
            });

            if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2 && context.Settings.World == 1)
                maps = maps.Reverse();

            // Create a collision map for 2D levels
            if (levelData.Collision2D != null)
            {
                int width, height;
                if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3) {
                    width = levelData.Collision2D.Width / levelData.Collision2D.TileWidth;
                    height = levelData.Collision2D.Height / levelData.Collision2D.TileHeight;
                } else {
                    width = levelData.Collision2D.Width * 4;
                    height = levelData.Collision2D.Height;
                }
                maps = maps.Append(new Unity_Map() {
                    Type = Unity_Map.MapType.Collision,
                    Width = (ushort)(width),
                    Height = (ushort)(height),
                    TileSet = new Unity_MapTileMap[]
                    {
                        tileSet
                    },
                    MapTiles = GetCollision2DMapTiles(context, levelData.Collision2D).Select(x => new Unity_Tile(x)).ToArray(),
                });
            }

            // Add the map if available
            var lvlMap = rom.LevelMaps?.FirstOrDefault(x => x.LevelID == rom.GetLevelDataID(context.Settings));
            if (context.Settings.World == 0 && lvlMap != null && Settings.LoadIsometricMapLayer)
            {
                maps = maps.Append(new Unity_Map() {
                    Type = Unity_Map.MapType.Graphics,
                    Width = lvlMap.Map.Width,
                    Height = lvlMap.Map.Height,
                    TileSet = new Unity_MapTileMap[]
                    {
                        LoadTileSet(lvlMap.Palette, lvlMap.TileSet, lvlMap.Map.MapData)
                    },
                    MapTiles = lvlMap.Map.MapData.Select(x => new Unity_Tile(x)).ToArray()
                });
            }

            var validMaps = maps.Where(x => x != null).ToArray();
            var objManager = new Unity_ObjectManager_GBAIsometricSpyro(context, rom.ObjectTypes, GetAnimSets(context, rom).ToArray());

            // Load objects
            var objects = new List<Unity_Object>();
            var objTable = rom.GetObjectTable(context.Settings);

            // Init the objects
            if (objTable != null)
            {
                InitObjects(objTable.Objects);
                objects.AddRange(objTable.Objects.Select(x => new Unity_Object_GBAIsometricSpyro(x, objManager)));
            }

            // Spyro 2: Snap object height to height of collision tile
            if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2 && isometricData != null) {
                foreach (var obj in objects) {
                    var objX = obj.XPosition / 16f;
                    var objY = obj.YPosition / 16f;
                    if (objX < 0 || objX >= isometricData.CollisionWidth ||
                        objY < 0 || objY >= isometricData.CollisionHeight) continue;
                    var col = isometricData.Collision[Mathf.FloorToInt(objY) * isometricData.CollisionWidth + Mathf.FloorToInt(objX)];

                    ((Unity_Object_GBAIsometricSpyro)obj).Object.Height = (short)(col.Height * 16);
                }
            }

            Controller.DetailedState = $"Loading localization";
            await Controller.WaitIfNecessary();

            return new Unity_Level(
                maps: validMaps,
                objManager: objManager,
                eventData: objects,
                cellSize: CellSize,
                getCollisionTypeNameFunc: collNameFunc,
                getCollisionTypeGraphicFunc: collGraphicFunc,
                defaultMap: 1,
                isometricData: isometricData,
                localization: LoadLocalization(context, rom),
                defaultCollisionMap: validMaps.Length - 1) { CellSizeOverrideCollision = context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3 ? (int?)16 : null };
        }

        public Dictionary<string, string[]> LoadLocalization(Context context, GBAIsometric_Spyro_ROM rom)
        {
            var langages = GetLanguages.ToArray();

            return rom.Localization.LocBlocks?.Select((x, i) => new
            {
                Lang = langages[i],
                Strings = x.Strings
            }).ToDictionary(x => x.Lang, x => x.Strings);
        }

        public IEnumerable<Unity_ObjectManager_GBAIsometricSpyro.AnimSet> GetAnimSets(Context context, GBAIsometric_Spyro_ROM rom)
        {
            var objPal = rom.GetLevelData(context.Settings).ObjPalette;

            if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
            {
                var lvlPal = objPal;
                objPal = rom.CommonPalette;

                for (int i = 0; i < 256; i++)
                {
                    if (lvlPal[i].Color1555 != 0)
                        objPal[i] = lvlPal[i];
                }
            }

            var pal = Util.ConvertAndSplitGBAPalette(objPal);

            // Add animation sets
            foreach (var animSet in rom.AnimSets)
            {
                yield return new Unity_ObjectManager_GBAIsometricSpyro.AnimSet(animSet, animSet.AnimBlock.Animations.Select(x =>
                {
                    return new Unity_ObjectManager_GBAIsometricSpyro.AnimSet.Animation(
                        () => GetAnimationFrames(animSet, x, pal).Select(f => f.CreateSprite()).ToArray(),
                        x.AnimSpeed,
                        GetFramePositions(animSet, x));
                }).ToArray());
            }
        }

        // Recreated from function at 0x08050200 (US rom for Spyro3)
        public void InitObjects(GBAIsometric_Object[] objects)
        {
            var readingWaypoints = false;
            var readingObjIndex = 0;
            int currentWayPointCount = 0;

            for (int index = 0; index < objects.Length; index++)
            {
                //var obj = objects[index];

                if (!readingWaypoints)
                {
                    // If the next object is a waypoint we read them until we reach the next normal object
                    if (index < objects.Length - 1 && !objects[index + 1].IsNormalObj)
                    {
                        // Save current object as it's the parent of the waypoints
                        readingObjIndex = index;

                        // Keep track of the amount of waypoints we read
                        currentWayPointCount = 0;

                        // Indicate that we're reading waypoints now
                        readingWaypoints = true;
                    }
                    else
                    {
                        // Init a normal object without waypoints
                        //FUN_0801534c(objects[index + 1].ObjectType, objects[index + 1].Value1, obj.XPosition);
                    }
                }
                else
                {
                    // Increment the waypoint count
                    currentWayPointCount += 1;

                    // If the next object is not a waypoint we stop reading them
                    if (index == objects.Length - 1 || objects[index + 1].IsNormalObj)
                    {
                        // Get the waypoints parent
                        var readingObj = objects[readingObjIndex];

                        // Init an object with waypoints
                        //FUN_08015408(readingObj.ObjectType, readingObj, currentWayPointCount);
                        
                        // Indicate that we're no longer reading waypoints
                        readingWaypoints = false;

                        // Save waypoint info
                        readingObj.WaypointIndex = (short)(readingObjIndex + 1);
                        readingObj.WaypointCount = (byte)currentWayPointCount;
                    }
                }

                //FUN_08003bbc();
            }
        }

        public MapTile[] GetMapTiles(GBAIsometric_Spyro_MapLayer mapLayer)
        {
            var width = mapLayer.Map.Width * mapLayer.TileAssemble.GroupWidth;
            var height = mapLayer.Map.Height * mapLayer.TileAssemble.GroupHeight;
            var tiles = new MapTile[width * height];

            for (int blockY = 0; blockY < mapLayer.Map.Height; blockY++)
            {
                for (int blockX = 0; blockX < mapLayer.Map.Width; blockX++)
                {
                    var tileBlock = mapLayer.TileAssemble.TileGroups[mapLayer.Map.MapData[blockY * mapLayer.Map.Width + blockX]];

                    var actualX = blockX * mapLayer.TileAssemble.GroupWidth;
                    var actualY = blockY * mapLayer.TileAssemble.GroupHeight;

                    for (int y = 0; y < mapLayer.TileAssemble.GroupHeight; y++)
                    {
                        for (int x = 0; x < mapLayer.TileAssemble.GroupWidth; x++)
                        {
                            MapTile mt = tileBlock[y * mapLayer.TileAssemble.GroupWidth + x];
                            tiles[(actualY + y) * width + (actualX + x)] = new MapTile() {
                                TileMapY = (ushort)(mt.TileMapY + (mapLayer.TileSet.Region * 512)),
                                VerticalFlip = mt.VerticalFlip,
                                HorizontalFlip = mt.HorizontalFlip,
                                PaletteIndex = mt.PaletteIndex
                            };
                        }
                    }
                }
            }

            return tiles;
        }
        public MapTile[] GetCollision2DMapTiles(Context context, GBAIsometric_Spyro_Collision2DMapData collision2D)
        {

            if (context.Settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2) {
                int width = collision2D.Width;
                int height = collision2D.Height;
                MapTile[] tiles = new MapTile[width * height * 4];

                for (int y = 0; y < collision2D.Height; y++) {
                    for (int x = 0; x < collision2D.Width; x++) {
                        int ind = y * width + x;
                        var c = collision2D.Collision[ind];
                        for (int i = 0; i < 4; i++) {
                            tiles[ind * 4 + i] = new MapTile() { CollisionType = (byte)BitHelpers.ExtractBits(c, 2, (4-i-1) * 2) };
                        }
                    }
                }
                return tiles;
            } else {
                return collision2D.Collision.Select(c => new MapTile() { CollisionType = c }).ToArray();
            }
        }

        public Unity_MapTileMap LoadTileSet(ARGB1555Color[] tilePal, GBAIsometric_Spyro_TileSet[] tileSets, Dictionary<GBAIsometric_Spyro_MapLayer, MapTile[]> mapTiles)
        {
            var palettes = Util.ConvertAndSplitGBAPalette(tilePal);

            const int tileSize = 32;
            const int regionSize = tileSize * 512;
            
            var tileSet = new byte[tileSets.Select(t => t.RegionOffset * tileSize + t.Region * regionSize + t.TileData.Length).Max()];

            // Fill regions with tile data
            foreach (var t in tileSets)
                Array.Copy(t.TileData, 0, tileSet, t.RegionOffset * tileSize + t.Region * regionSize, t.TileData.Length);

            int[] paletteIndices = new int[tileSet.Length];
            foreach (MapTile mt in mapTiles.SelectMany(mta => mta.Value))
                paletteIndices[mt.TileMapY] = mt.PaletteIndex;

            var tileSetTex = Util.ToTileSetTexture(tileSet, palettes.First(), false, CellSize, false, getPalFunc: i => palettes[paletteIndices[i]]);

            return new Unity_MapTileMap(tileSetTex, CellSize);
        }

        public Unity_MapTileMap LoadTileSet(ARGB1555Color[] tilePal, byte[] tileSet, MapTile[] mapTiles)
        {
            var palettes = Util.ConvertGBAPalette(tilePal).Split(16, 16).ToArray();

            int[] paletteIndices = new int[tileSet.Length];
            foreach (MapTile mt in mapTiles)
                paletteIndices[mt.TileMapY] = mt.PaletteIndex;

            var tileSetTex = Util.ToTileSetTexture(tileSet, palettes.First(), false, CellSize, false, getPalFunc: i => palettes[paletteIndices[i]]);

            return new Unity_MapTileMap(tileSetTex, CellSize);
        }

        public Unity_MapTileMap LoadTileSet(ARGB1555Color[] tilePal, byte[] tileSet)
        {
            var pal = Util.ConvertGBAPalette(tilePal);

            var tileSetTex = Util.ToTileSetTexture(tileSet, pal, true, CellSize, false);

            return new Unity_MapTileMap(tileSetTex, CellSize);
        }

        public Vector2Int[] GetFramePositions(GBAIsometric_Spyro_AnimSet animSet, GBAIsometric_Spyro_Animation anim) {
            Vector2Int[] pos = new Vector2Int[anim.Frames.Length + (anim.PingPong ? (anim.Frames.Length - 2) : 0)];
            for (int i = 0; i < anim.Frames.Length; i++) {
                var f = anim.Frames[i];
                pos[i] = new Vector2Int(f.XPosition, f.YPosition);
            }
            if (anim.PingPong) {
                for (int i = anim.Frames.Length; i < pos.Length; i++) {
                    pos[i] = pos[pos.Length - i];
                }
            }
            return pos;
        }

        public Texture2D[] GetAnimationFrames(GBAIsometric_Spyro_AnimSet animSet, GBAIsometric_Spyro_Animation anim, Color[][] pal, bool isExport = false)
        {
            int minX1 = 0, minY1 = 0, maxX2 = int.MinValue, maxY2 = int.MinValue;
            if (isExport) {
                if (anim.Frames.Length > 0) {
                    var fs = anim.Frames.Where(f => !animSet.AnimFrameImages[f.FrameImageIndex].IsNullFrame);
                    minX1 = fs.Min(f => f.XPosition);
                    minY1 = fs.Min(f => f.YPosition);
                    maxX2 = fs.Max(f => f.XPosition + animSet.AnimFrameImages[f.FrameImageIndex].Width);
                    maxY2 = fs.Max(f => f.YPosition + animSet.AnimFrameImages[f.FrameImageIndex].Height);
                } else {
                    maxX2 = 0;
                    maxY2 = 0;
                }
            }
            Texture2D[] texs = new Texture2D[anim.Frames.Length + (anim.PingPong ? (anim.Frames.Length - 2) : 0)];
            for(int i = 0; i < anim.Frames.Length; i++) {
                var frame = anim.Frames[i];
                var frameImg = animSet.AnimFrameImages[frame.FrameImageIndex];
                int w, h;
                if (isExport) {
                    w = (maxX2 - minX1);
                    h = (maxY2 - minY1);
                } else {
                    w = frameImg.Width;
                    h = frameImg.Height;
                    //frameImg.GetActualSize(out w, out h);
                }
                /*var w = isExport ? (maxW - minX) : frameImg.Width;
                var h = isExport ? (maxH - minY) : frameImg.Height;*/
                Texture2D tex = TextureHelpers.CreateTexture2D(w, h, clear: true);
                int totalTileInd = 0;

                void addObjToFrame(byte spriteSize, GBAIsometric_Spyro_AnimPattern.Shape spriteShape, int xpos, int ypos, int relativeTile, byte palIndex)
                {
                    // Get size
                    Util.GetGBASize((byte)spriteShape, spriteSize, out int width, out int height);

                    //var tileIndex = relativeTile;
                    var tileIndex = frameImg.TileIndex + totalTileInd;
                    totalTileInd += width * height;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int actualX = (x * CellSize) + xpos + (isExport ? (frame.XPosition - minX1) : 0);
                            int actualY = (y * CellSize) + ypos + (isExport ? (frame.YPosition - minY1) : 0);

                            tex.FillInTile(animSet.TileSet, tileIndex * 32, pal[palIndex], false, CellSize, true, actualX, actualY, ignoreTransparent: true);

                            tileIndex++;
                        }
                    }
                }

                if (!frameImg.HasPatterns)
                {
                    addObjToFrame(frameImg.SpriteSize, frameImg.SpriteShape, 0, 0, frameImg.TileIndex, frameImg.PalIndex);
                }
                else
                {
                    foreach (var pattern in frameImg.Patterns)
                        addObjToFrame(pattern.SpriteSize, pattern.SpriteShape, pattern.X, pattern.Y, pattern.RelativeTileIndex + frameImg.TileIndex, pattern.PalIndex);
                }

                tex.Apply();

                texs[i] = tex;
            }
            if (anim.PingPong) {
                for (int i = anim.Frames.Length; i < texs.Length; i++) {
                    texs[i] = texs[texs.Length - i];
                }
            }
            return texs;
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}