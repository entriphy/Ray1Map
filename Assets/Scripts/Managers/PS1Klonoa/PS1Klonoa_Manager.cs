﻿using BinarySerializer;
using BinarySerializer.PS1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BinarySerializer.KlonoaDTP;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class PS1Klonoa_Manager : BaseGameManager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Levels.Select((x, i) => new GameInfo_World(i, x.Item1, Enumerable.Range(0, x.Item2).ToArray())).ToArray());

        public virtual (string, int)[] Levels => new (string, int)[]
        {
            ("FIX", 0),
            ("MENU", 0),
            ("CODE", 0),

            ("Vision 1-1", 3),
            ("Vision 1-2", 5),
            ("Rongo Lango", 2),

            ("Vision 2-1", 4),
            ("Vision 2-2", 6),
            ("Pamela", 2),

            ("Vision 3-1", 5),
            ("Vision 3-2", 10),
            ("Gelg Bolm", 1),

            ("Vision 4-1", 3),
            ("Vision 4-2", 8),
            ("Baladium", 2),

            ("Vision 5-1", 7),
            ("Vision 5-2", 9),
            ("Joka", 1),

            ("Vision 6-1", 8),
            ("Vision 6-2", 8),
            ("", 2),
            ("", 2),
            ("", 3),
            ("", 3),
            ("", 9),
        };

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Extract BIN", false, true, (input, output) => Extract_BINAsync(settings, output, false)),
                new GameAction("Extract BIN (unpack archives)", false, true, (input, output) => Extract_BINAsync(settings, output, true)),
                new GameAction("Extract Code", false, true, (input, output) => Extract_CodeAsync(settings, output)),
                new GameAction("Extract Graphics", false, true, (input, output) => Extract_GraphicsAsync(settings, output)),
                new GameAction("Extract Backgrounds", false, true, (input, output) => Extract_BackgroundsAsync(settings, output)),
                new GameAction("Extract Sprites", false, true, (input, output) => Extract_SpriteFramesAsync(settings, output)),
                new GameAction("Extract ULZ blocks", false, true, (input, output) => Extract_ULZAsync(settings, output)),
            };
        }

        public async UniTask Extract_BINAsync(GameSettings settings, string outputPath, bool unpack)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var s = context.Deserializer;

            var loader = Loader.Create(context, idxData, config);

            var archiveDepths = new Dictionary<IDXLoadCommand.FileType, int>()
            {
                [IDXLoadCommand.FileType.Unknown] = 0,

                [IDXLoadCommand.FileType.Archive_TIM_Generic] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SongsText] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SaveText] = 1,
                [IDXLoadCommand.FileType.Archive_TIM_SpriteSheets] = 1,

                [IDXLoadCommand.FileType.OA05] = 0,
                [IDXLoadCommand.FileType.SEQ] = 0,

                [IDXLoadCommand.FileType.Archive_BackgroundPack] = 2,

                [IDXLoadCommand.FileType.FixedSprites] = 1,
                [IDXLoadCommand.FileType.Archive_SpritePack] = 1,
                
                [IDXLoadCommand.FileType.Archive_LevelPack] = 1,
                
                [IDXLoadCommand.FileType.Archive_Unk0] = 1,
                [IDXLoadCommand.FileType.Archive_Unk4] = 2,
                [IDXLoadCommand.FileType.Archive_Unk5] = 1,

                [IDXLoadCommand.FileType.Code] = 0,
            };

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    var type = cmd.FILE_Type;

                    if (unpack)
                    {
                        var archiveDepth = archiveDepths[type];

                        if (archiveDepth > 0)
                        {
                            // Be lazy and hard-code instead of making some recursive loop
                            if (archiveDepth == 1)
                            {
                                var archive = loader.LoadBINFile<RawData_ArchiveFile>(i);

                                for (int j = 0; j < archive.Files.Length; j++)
                                {
                                    var file = archive.Files[j];

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"{j}.bin"), file.Data);
                                }
                            }
                            else if (archiveDepth == 2)
                            {
                                var archives = loader.LoadBINFile<ArchiveFile<RawData_ArchiveFile>>(i);

                                for (int a = 0; a < archives.Files.Length; a++)
                                {
                                    var archive = archives.Files[a];

                                    for (int j = 0; j < archive.Files.Length; j++)
                                    {
                                        var file = archive.Files[j];

                                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"{a}_{j}.bin"), file.Data);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception($"Unsupported archive depth");
                            }

                            return;
                        }
                    }

                    // Read the raw data
                    var data = s.SerializeArray<byte>(null, cmd.FILE_Length);

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{i} ({type})", $"Data.bin"), data);
                });
            }
        }

        public async UniTask Extract_CodeAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    if (cmd.FILE_Type != IDXLoadCommand.FileType.Code)
                        return;
                    
                    var codeFile = loader.LoadBINFile<RawData_File>(i);

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - 0x{cmd.GetFileDestinationAddress(loader):X8}.dat"), codeFile.Data);
                });
            }
        }

        public async UniTask Extract_GraphicsAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    var index = 0;

                    switch (cmd.FILE_Type)
                    {
                        case IDXLoadCommand.FileType.Code:
                            loader.ProcessBINFile(i);
                            break;

                        case IDXLoadCommand.FileType.Archive_TIM_Generic:
                        case IDXLoadCommand.FileType.Archive_TIM_SongsText:
                        case IDXLoadCommand.FileType.Archive_TIM_SaveText:
                        case IDXLoadCommand.FileType.Archive_TIM_SpriteSheets:

                            // Read the data
                            TIM_ArchiveFile timFiles = loader.LoadBINFile<TIM_ArchiveFile>(i);

                            foreach (var tim in timFiles.Files)
                            {
                                export(() => GetTexture(tim), cmd.FILE_Type.ToString().Replace("Archive_TIM_", String.Empty));
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_BackgroundPack:

                            // Read the data
                            var bgPack = loader.LoadBINFile<BackgroundPack_ArchiveFile>(i);

                            foreach (var tim in bgPack.TIMFiles.Files)
                            {
                                export(() => GetTexture(tim), "Background");
                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_SpritePack:

                            // Read the data
                            LevelSpritePack_ArchiveFile spritePack = loader.LoadBINFile<LevelSpritePack_ArchiveFile>(i);

                            var exported = new HashSet<PlayerSprite_File>();

                            var pal = spritePack.PlayerSprites.Files.FirstOrDefault(x => x?.TIM?.Clut != null)?.TIM.Clut.Palette.Select(x => x.GetColor()).ToArray();

                            foreach (var file in spritePack.PlayerSprites.Files)
                            {
                                if (file != null && !exported.Contains(file))
                                {
                                    exported.Add(file);

                                    export(() =>
                                    {
                                        if (file.TIM != null)
                                            return GetTexture(file.TIM, palette: pal);
                                        else
                                            return GetTexture(file.Raw_ImgData, pal, file.Raw_Width, file.Raw_Height, PS1_TIM.TIM_ColorFormat.BPP_8);
                                    }, "PlayerSprites");
                                }

                                index++;
                            }

                            break;

                        case IDXLoadCommand.FileType.Archive_LevelPack:

                            // Need to process the data for the object 3D export
                            loader.ProcessBINFile(i);

                            // TODO: Remove try/catch
                            try
                            {
                                // Read the data
                                var lvlPack = loader.LoadBINFile<LevelPack_ArchiveFile>(i);

                                var cutscenePack = lvlPack.CutscenePack;

                                if (cutscenePack != null)
                                {
                                    export(() => GetTexture(cutscenePack.CharacterNamesImgData.Data, null, 0x0C, 0x50, PS1_TIM.TIM_ColorFormat.BPP_4), "CharacterNames");

                                    foreach (var file in cutscenePack.File_2.Files)
                                    {
                                        export(() => GetTexture(file.ImgData, null, file.Width / 2, file.Height, PS1_TIM.TIM_ColorFormat.BPP_8), "CutsceneFrames");

                                        index++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning(ex);
                            }
                            break;
                    }

                    void export(Func<Texture2D> getTex, string type) => exportTex(getTex, type, $"{i} - {index}");
                });

                if (blockIndex < loader.Config.BLOCK_FirstLevel)
                    continue;

                // Process code level data
                loader.ProcessCodeLevelData();

                var exported = new HashSet<BinarySerializable>();

                for (int sector = 0; sector < loader.CodeLevelData.SectorModifiers.Length; sector++)
                {
                    var modifiers = loader.CodeLevelData.SectorModifiers[sector];

                    for (int modifierIndex = 0; modifierIndex < modifiers.Modifiers.Length; modifierIndex++)
                    {
                        var modifier = modifiers.Modifiers[modifierIndex];

                        foreach (var dataFile in modifier.DataFiles)
                        {
                            if (dataFile.TIM != null)
                            {
                                if (exported.Contains(dataFile.TIM))
                                    continue;

                                exported.Add(dataFile.TIM);

                                exportTex(() => GetTexture(dataFile.TIM), "Obj3D", $"{sector} - {modifierIndex}");
                            }
                            else if (dataFile.TIMFiles != null)
                            {
                                if (exported.Contains(dataFile.TIMFiles))
                                    continue;

                                exported.Add(dataFile.TIMFiles);

                                for (var timIndex = 0; timIndex < dataFile.TIMFiles.Files.Length; timIndex++)
                                {
                                    var tim = dataFile.TIMFiles.Files[timIndex];
                                    exportTex(() => GetTexture(tim), "Obj3D", $"{sector} - {modifierIndex} - {timIndex}");
                                }
                            }
                        }
                    }
                }

                void exportTex(Func<Texture2D> getTex, string type, string name)
                {
                    try
                    {
                        var tex = getTex();

                        if (tex != null)
                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {type}", $"{name}.png"),
                                tex.EncodeToPNG());
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting with ex: {ex}");
                    }
                }
            }
        }

        public async UniTask Extract_SpriteFramesAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 0; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var entry = idxData.Entries[blockIndex];

                loader.SwitchBlocks(blockIndex);

                // Load the BIN
                loader.LoadAndProcessBINBlock();

                // Enumerate every set of frames
                for (int framesSet = 0; framesSet < loader.SpriteFrames.Length; framesSet++)
                {
                    var spriteFrames = loader.SpriteFrames[framesSet];

                    if (spriteFrames == null)
                        continue;

                    // Enumerate every frame
                    for (int frameIndex = 0; frameIndex < spriteFrames.Files.Length; frameIndex++)
                    {
                        try
                        {
                            var sprites = spriteFrames.Files[frameIndex].Textures;

                            foreach (var s in sprites)
                            {
                                if (s.FlipX)
                                    s.XPos = (short)(s.XPos - s.Width - 1);
                                if (s.FlipY)
                                    s.YPos = (short)(s.YPos - s.Height - 1);
                            }

                            var minX = sprites.Min(x => x.XPos);
                            var minY = sprites.Min(x => x.YPos);
                            var maxX = sprites.Max(x => x.XPos + x.Width);
                            var maxY = sprites.Max(x => x.YPos + x.Height);

                            var width = maxX - minX;
                            var height = maxY - minY;

                            var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

                            foreach (var sprite in sprites)
                            {
                                var texPage = sprite.TexturePage;

                                FillTextureFromVRAM(
                                    tex: tex,
                                    vram: loader.VRAM,
                                    width: sprite.Width,
                                    height: sprite.Height,
                                    colorFormat: PS1_TIM.TIM_ColorFormat.BPP_4,
                                    texX: sprite.XPos - minX,
                                    texY: sprite.YPos - minY,
                                    clutX: sprite.PalOffsetX,
                                    clutY: 500 + sprite.PalOffsetY,
                                    texturePageOriginX: 64 * (texPage % 16),
                                    texturePageOriginY: 128 * (texPage / 16), // TODO: Fix this
                                    texturePageOffsetX: sprite.TexturePageOffsetX,
                                    texturePageOffsetY: sprite.TexturePageOffsetY,
                                    flipX: sprite.FlipX,
                                    flipY: sprite.FlipY);
                            }

                            tex.Apply();

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex}", $"{framesSet} - {frameIndex}.png"), tex.EncodeToPNG());
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Error exporting sprite frame: {ex}");
                        }
                    }
                }

                try
                {
                    PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), loader.VRAM);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error exporting VRAM: {ex}");
                }
            }
        }

        public async UniTask Extract_BackgroundsAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);
            var config = GetLoaderConfig(settings);
            await LoadFilesAsync(context, config);

            // Load the IDX
            var idxData = Load_IDX(context);

            var loader = Loader.Create(context, idxData, config);

            // Enumerate every entry
            for (var blockIndex = 3; blockIndex < idxData.Entries.Length; blockIndex++)
            {
                var vram = new PS1_VRAM();

                loader.SwitchBlocks(blockIndex);

                // Process each BIN file
                loader.LoadBINFiles((cmd, i) =>
                {
                    try
                    {
                        // Check the file type
                        if (cmd.FILE_Type != IDXLoadCommand.FileType.Archive_BackgroundPack)
                            return;

                        // Read the data
                        var bg = loader.LoadBINFile<BackgroundPack_ArchiveFile>(i);

                        // TODO: Some maps use different textures! How do we find the index? For now export all variants
                        for (int tileSetIndex = 0; tileSetIndex < bg.TIMFiles.Files.Length; tileSetIndex++)
                        {
                            var tim = bg.TIMFiles.Files[tileSetIndex];
                            var cel = bg.CELFiles.Files[tileSetIndex];

                            // The game hard-codes this
                            if (tileSetIndex == 1)
                            {
                                tim.XPos = 0x1C0;
                                tim.YPos = 0x100;
                                tim.Width = 0x40;
                                tim.Height = 0x100;

                                tim.Clut.XPos = 0x120;
                                tim.Clut.YPos = 0x1F0;
                                tim.Clut.Width = 0x10;
                                tim.Clut.Height = 0x10;
                            }

                            loader.AddToVRAM(tim);

                            for (int j = 0; j < bg.BGDFiles.Files.Length; j++)
                            {
                                var map = bg.BGDFiles.Files[j];

                                var tex = TextureHelpers.CreateTexture2D(map.MapWidth * map.CellWidth, map.MapHeight * map.CellHeight, clear: true);

                                for (int mapY = 0; mapY < map.MapHeight; mapY++)
                                {
                                    for (int mapX = 0; mapX < map.MapWidth; mapX++)
                                    {
                                        var cellIndex = map.Map[mapY * map.MapWidth + mapX];

                                        if (cellIndex == 0xFF)
                                            continue;

                                        var cell = cel.Cells[cellIndex];

                                        FillTextureFromVRAM(
                                            tex: tex, 
                                            vram: vram, 
                                            width: map.CellWidth, 
                                            height: map.CellHeight, 
                                            colorFormat: tim.ColorFormat, 
                                            texX: mapX * map.CellWidth, 
                                            texY: mapY * map.CellHeight, 
                                            clutX: cell.ClutX * 16, 
                                            clutY: cell.ClutY, 
                                            texturePageOriginX: tim.XPos, 
                                            texturePageOriginY: tim.YPos, 
                                            texturePageOffsetX: cell.XOffset, 
                                            texturePageOffsetY: cell.YOffset);
                                    }
                                }

                                tex.Apply();

                                Util.ByteArrayToFile(Path.Combine(outputPath, $"{blockIndex} - {i} - {j}_{tileSetIndex}.png"), tex.EncodeToPNG());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error exporting with ex: {ex}");
                    }
                });

                PaletteHelpers.ExportVram(Path.Combine(outputPath, $"VRAM_{blockIndex}.png"), vram);
            }
        }

        public async UniTask Extract_ULZAsync(GameSettings settings, string outputPath)
        {
            using var context = new R1Context(settings);

            await LoadFilesAsync(context);

            var s = context.Deserializer;

            s.Goto(context.GetFile(Loader.FilePath_BIN).StartPointer);

            while (s.CurrentFileOffset < s.CurrentLength)
            {
                var v = s.Serialize<int>(default);

                if (v != 0x1A7A6C55)
                    continue;

                var offset = s.CurrentPointer - 4;

                s.DoAt(offset, () =>
                {
                    try
                    {
                        var bytes = s.DoEncoded(new ULZEncoder(), () => s.SerializeArray<byte>(default, s.CurrentLength));

                        Util.ByteArrayToFile(Path.Combine(outputPath, $"0x{offset.AbsoluteOffset:X8}.bin"), bytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error decompressing at {offset} with ex: {ex}");
                    }
                });
            }
        }

        public LoaderConfiguration GetLoaderConfig(GameSettings settings)
        {
            // TODO: Support other versions
            return new LoaderConfiguration_DTP_US();
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            // Get settings
            var settings = context.GetR1Settings();
            var lev = settings.World;
            var sector = settings.Level;
            var config = GetLoaderConfig(settings);

            // Load the files
            await LoadFilesAsync(context, config);

            Controller.DetailedState = "Loading IDX";
            await Controller.WaitIfNecessary();

            // Load the IDX
            var idxData = Load_IDX(context);

            Controller.DetailedState = "Loading BIN";
            await Controller.WaitIfNecessary();

            // Create the loader
            var loader = Loader.Create(context, idxData, config);

            // Only parse the selected sector
            loader.SectorToParse = sector;

            var logAction = new Func<string, Task>(async x =>
            {
                Controller.DetailedState = x;
                await Controller.WaitIfNecessary();
            });

            // Load the fixed BIN
            loader.SwitchBlocks(loader.Config.BLOCK_Fix);
            await loader.LoadAndProcessBINBlockAsync(logAction);

            // Load the level BIN
            loader.SwitchBlocks(lev);
            await loader.LoadAndProcessBINBlockAsync(logAction);

            Controller.DetailedState = "Loading code level data";
            await Controller.WaitIfNecessary();

            // Load code level data
            loader.ProcessCodeLevelData();

            // Copy debug string to clipboard if any
            if (ModifierObject.DebugStringBuilder.Length > 0)
                ModifierObject.DebugStringBuilder.ToString().CopyToClipboard();

            // Load the layers
            var layers = await Load_LayersAsync(loader, sector);

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            // TODO: Load objects

            var movementPaths = loader.LevelPack.Sectors[sector].MovementPaths.Files;

            Debug.Log($"Map has {movementPaths.Length} movement paths");

            return new Unity_Level(
                layers: layers,
                cellSize: 16,
                objManager: new Unity_ObjectManager(context),
                // Load dummy objects for movement paths
                eventData: new List<Unity_Object>(movementPaths.SelectMany(x => x.Blocks).Select(x => new Unity_Object_PS1Klonoa(x)
                {
                    Position = new Vector3(x.XPos / 16f, -(x.YPos / 16f), -x.ZPos / 16f),
                })),
                //eventData: new List<Unity_Object>(loader.CodeLevelData.Objects3D[sector].Objects.First(x => x.Type == Object3D.Object3DType.Type_7).Data_Type7.Entries.Select(x => new Unity_Object_PS1Klonoa()
                //{
                //    Position = new Vector3(x.Short_00 / 16f, -(x.Short_02 / 16f), -x.Short_04 / 16f),
                //})),
                isometricData: new Unity_IsometricData
                {
                    CollisionWidth = 0,
                    CollisionHeight = 0,
                    TilesWidth = 0,
                    TilesHeight = 0,
                    Collision = null,
                    Scale = Vector3.one,
                    ViewAngle = Quaternion.Euler(90, 0, 0),
                    CalculateYDisplacement = () => 0,
                    CalculateXDisplacement = () => 0,
                    ObjectScale = Vector3.one * 8
                });
        }

        public async UniTask<Unity_Layer[]> Load_LayersAsync(Loader loader, int sector)
        {
            var modifiers = loader.CodeLevelData.SectorModifiers[sector].Modifiers;
            var texAnimations = modifiers.
                Where(x => x.PrimaryType == PrimaryObjectType.Modifier_41).
                SelectMany(x => x.DataFiles).
                Where(x => x.TIMFiles != null).
                Select(x => x.TIMFiles.Files).
                ToArray();

            Debug.Log($"Map has {texAnimations.Length} texture animations");

            var layers = new List<Unity_Layer>();

            const float scale = 16f;

            Controller.DetailedState = "Loading backgrounds";
            await Controller.WaitIfNecessary();

            // TODO: Load backgrounds - easiest to convert to textures instead of using tilemaps, unless it's easier for animations?

            Controller.DetailedState = "Loading level geometry";
            await Controller.WaitIfNecessary();

            var obj = CreateGameObject(loader.LevelPack.Sectors[sector].LevelModel, loader, scale, "Map", texAnimations, out bool isAnimated);
            var levelDimensions = GetDimensions(loader.LevelPack.Sectors[sector].LevelModel) / scale;
            obj.transform.position = new Vector3(0, 0, 0);

            var parent3d = Controller.obj.levelController.editor.layerTiles.transform;
            layers.Add(new Unity_Layer_GameObject(true, isAnimated: isAnimated)
            {
                Name = "Map",
                ShortName = "MAP",
                Graphics = obj,
                Collision = null,
                Dimensions = levelDimensions,
                DisableGraphicsWhenCollisionIsActive = true
            });
            obj.transform.SetParent(parent3d);

            Controller.DetailedState = "Loading 3D objects";
            await Controller.WaitIfNecessary();

            GameObject gao_3dObjParent = null;

            bool isObjAnimated = false;
            foreach (var modifier in modifiers)
            {
                if (modifier.PrimaryType == PrimaryObjectType.None || modifier.PrimaryType == PrimaryObjectType.Invalid)
                    continue;

                if (modifier.PrimaryType == PrimaryObjectType.Modifier_41)
                {
                    var pos = modifier.DataFiles.FirstOrDefault(x => x.Position != null)?.Position;
                    var transform = modifier.DataFiles.FirstOrDefault(x => x.Transform != null)?.Transform;
                    var tmdFiles = modifier.DataFiles.Where(x => x.TMD != null).Select(x => x.TMD);

                    var index = 0;
                    foreach (var tmd in tmdFiles)
                    {
                        createObj(tmd, transform?.Position ?? pos, transform?.Rotation, index);
                        index++;
                    }
                }
                else
                {
                    Debug.LogWarning($"Skipped unsupported modifier object of primary type {modifier.PrimaryType}");
                }

                // Helper for creating an object
                void createObj(PS1_TMD tmd, ObjPosition_File pos = null, ObjRotation_File rot = null, int index = 0)
                {
                    if (gao_3dObjParent == null)
                    {
                        gao_3dObjParent = new GameObject("3D Objects");
                        gao_3dObjParent.transform.localPosition = Vector3.zero;
                        gao_3dObjParent.transform.localRotation = Quaternion.identity;
                        gao_3dObjParent.transform.localScale = Vector3.one;
                    }

                    var gameObj = CreateGameObject(tmd, loader, scale, $"Object3D_{modifier.Offset}_{index}", texAnimations, out isAnimated);

                    if (isAnimated)
                        isObjAnimated = true;

                    gameObj.transform.SetParent(gao_3dObjParent.transform);

                    if (pos != null)
                        gameObj.transform.position = new Vector3(pos.XPos / scale, -pos.YPos / scale, pos.ZPos / scale);

                    // TODO: Fix rotation
                    if (rot != null)
                        gameObj.transform.localRotation = Quaternion.Euler(
                            x: rot.RotationX / 8f,
                            y: rot.RotationY / 8f,
                            z: rot.RotationZ / 8f);
                }
            }

            if (gao_3dObjParent != null)
            {
                layers.Add(new Unity_Layer_GameObject(true, isAnimated: isObjAnimated)
                {
                    Name = "3D Objects",
                    ShortName = $"3DO",
                    Graphics = gao_3dObjParent
                });
                gao_3dObjParent.transform.SetParent(parent3d);
            }

            Controller.DetailedState = "Loading collision";
            await Controller.WaitIfNecessary();

            // TODO: Load collision

            return layers.ToArray();
        }

        public GameObject CreateGameObject(PS1_TMD tmd, Loader loader, float scale, string name, PS1_TIM[][] texAnimations, out bool isAnimated)
        {
            isAnimated = false;
            var textureCache = new Dictionary<int, Texture2D>();
            var textureAnimCache = new Dictionary<long, Texture2D[]>();

            GameObject gaoParent = new GameObject(name);
            gaoParent.transform.position = Vector3.zero;

            // Create each object
            foreach (var obj in tmd.Objects)
            {
                // Helper methods
                Vector3 toVertex(PS1_TMD_Vertex v) => new Vector3(v.X / scale, -v.Y / scale, v.Z / scale);
                Vector2 toUV(PS1_TMD_UV uv) => new Vector2(uv.U / 255f, uv.V / 255f);
                RectInt getRect(PS1_TMD_UV[] uv)
                {
                    int xMin = uv.Min(x => x.U);
                    int xMax = uv.Max(x => x.U) + 1;
                    int yMin = uv.Min(x => x.V);
                    int yMax = uv.Max(x => x.V) + 1;
                    int w = xMax - xMin;
                    int h = yMax - yMin;

                    return new RectInt(xMin, yMin, w, h);
                }

                // TODO: Implement normals
                if (obj.NormalsCount != 0)
                    Debug.LogWarning($"TMD object has {obj.NormalsCount} normals");

                // Add each primitive
                foreach (var packet in obj.Primitives)
                {
                    // TODO: Implement
                    if (!packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.LGT))
                        Debug.LogWarning($"Packet has light source");

                    // TODO: Implement
                    if (packet.Flags.HasFlag(PS1_TMD_Packet.PacketFlags.FCE))
                        Debug.LogWarning($"Polygon is double faced");

                    // TODO: Implement other types
                    if (packet.Mode.Code != PS1_TMD_PacketMode.PacketModeCODE.Polygon)
                    {
                        Debug.LogWarning($"Skipped packet with code {packet.Mode.Code}");
                        continue;
                    }

                    Mesh unityMesh = new Mesh();

                    var vertices = packet.Vertices.Select(x => toVertex(obj.Vertices[x])).ToArray();
                    int[] triangles;

                    // Set vertices
                    unityMesh.SetVertices(vertices);

                    if (packet.Mode.IsQuad)
                    {
                        triangles = new int[]
                        {
                            // Lower left triangle
                            0, 1, 2,
                            // Upper right triangle
                            3, 2, 1
                        };
                    }
                    else
                    {
                        triangles = new int[]
                        {
                            0, 1, 2,
                        };
                    }

                    unityMesh.SetTriangles(triangles, 0);

                    var colors = packet.RGB.Select(x => x.Color.GetColor()).ToArray();

                    if (colors.Length == 1)
                        colors = Enumerable.Repeat(colors[0], packet.Mode.IsQuad ? 4 : 3).ToArray();

                    unityMesh.SetColors(colors);

                    if (packet.UV != null) 
                        unityMesh.SetUVs(0, packet.UV.Select(toUV).ToArray());

                    unityMesh.RecalculateNormals();

                    GameObject gao = new GameObject($"Packet_{packet.Offset}");

                    MeshFilter mf = gao.AddComponent<MeshFilter>();
                    MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                    gao.layer = LayerMask.NameToLayer("3D Collision");
                    gao.transform.SetParent(gaoParent.transform);
                    gao.transform.localScale = Vector3.one;
                    gao.transform.localPosition = Vector3.zero;
                    mf.mesh = unityMesh;
                    mr.material = Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial;

                    // Add texture
                    if (packet.Mode.TME)
                    {
                        var rect = getRect(packet.UV);

                        var key = packet.CBA.ClutX | packet.CBA.ClutY << 6 | packet.TSB.TX << 16 | packet.TSB.TY << 24;
                        long animKey = (long)key | (long)rect.x << 32 | (long)rect.y << 40;

                        if (!textureAnimCache.ContainsKey(animKey))
                        {
                            PS1_TIM[] foundAnim = null;

                            // Check if the texture region falls within an animated area
                            foreach (var anim in texAnimations)
                            {
                                foreach (var tim in anim)
                                {
                                    // Check the page
                                    if ((tim.XPos * 2) / PS1_VRAM.PageWidth == packet.TSB.TX &&
                                        tim.YPos / PS1_VRAM.PageHeight == packet.TSB.TY)
                                    {
                                        var is4Bit = tim.ColorFormat == PS1_TIM.TIM_ColorFormat.BPP_4;

                                        var timRect = new RectInt(
                                            xMin: ((tim.XPos * 2) % PS1_VRAM.PageWidth) * (is4Bit ? 2 : 1),
                                            yMin: (tim.YPos % PS1_VRAM.PageHeight),
                                            width: tim.Width * 2 * (is4Bit ? 2 : 1),
                                            height: tim.Height);

                                        // Check page offset
                                        if (rect.Overlaps(timRect))
                                        {
                                            foundAnim = anim;
                                            break;
                                        }
                                    }
                                }

                                if (foundAnim != null)
                                    break;
                            }

                            if (foundAnim != null)
                            {
                                var textures = new Texture2D[foundAnim.Length];

                                for (int i = 0; i < textures.Length; i++)
                                {
                                    loader.AddToVRAM(foundAnim[i]);
                                    textures[i] = GetTexture(packet, loader.VRAM);
                                }

                                textureAnimCache.Add(animKey, textures);
                            }
                            else
                            {
                                textureAnimCache.Add(animKey, null);
                            }
                        }

                        if (!textureCache.ContainsKey(key))
                            textureCache.Add(key, GetTexture(packet, loader.VRAM));

                        var t = textureCache[key];

                        var animTextures = textureAnimCache[animKey];

                        if (animTextures != null)
                        {
                            isAnimated = true;
                            var animTex = gao.AddComponent<AnimatedTextureComponent>();
                            animTex.material = mr.material;
                            animTex.animatedTextureSpeed = 2; // TODO: Is this correct?
                            animTex.animatedTextures = animTextures;
                        }

                        t.wrapMode = TextureWrapMode.Repeat;
                        mr.material.SetTexture("_MainTex", t);
                        mr.name = $"TX: {packet.TSB.TX}, TY:{packet.TSB.TY}, X: {rect.x}, Y:{rect.y}, W:{rect.width}, H:{rect.height}";
                    }
                }
            }

            //Debug.Log($"{textureCache.Count} textures");

            return gaoParent;
        }

        public Vector3 GetDimensions(PS1_TMD tmd)
        {
            var verts = tmd.Objects.SelectMany(x => x.Vertices).ToArray();
            var width = verts.Max(v => v.X) - verts.Min(v => v.X);
            var height = verts.Max(v => v.Y) - verts.Min(v => v.Y);
            var depth = verts.Max(v => v.Z) - verts.Min(v => v.Z);
            return new Vector3(width, height, depth);
        }

        public IDX Load_IDX(Context context)
        {
            return FileFactory.Read<IDX>(Loader.FilePath_IDX, context);
        }

        public void FillTextureFromVRAM(
            Texture2D tex,
            PS1_VRAM vram,
            int width, int height,
            PS1_TIM.TIM_ColorFormat colorFormat,
            int texX, int texY,
            int clutX, int clutY,
            int texturePageOriginX, int texturePageOriginY,
            int texturePageOffsetX, int texturePageOffsetY,
            int texturePageX = 0, int texturePageY = 0,
            bool flipX = false, bool flipY = false,
            bool useDummyPal = false)
        {
            var dummyPal = useDummyPal ? Util.CreateDummyPalette(colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8 ? 256 : 16) : null;

            texturePageOriginX *= 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte paletteIndex;

                    if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + texturePageOffsetX + x,
                            texturePageOriginY + texturePageOffsetY + y);
                    }
                    else if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_4)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + (texturePageOffsetX + x) / 2,
                            texturePageOriginY + texturePageOffsetY + y);

                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);
                    }
                    else
                    {
                        throw new Exception($"Non-supported color format");
                    }

                    // Get the color from the palette
                    var c = useDummyPal ? dummyPal[paletteIndex] : vram.GetColor1555(0, 0, clutX + paletteIndex, clutY);

                    if (c.Alpha == 0)
                        continue;

                    var texOffsetX = flipX ? width - x - 1 : x;
                    var texOffsetY = flipY ? height - y - 1 : y;

                    // Set the pixel
                    tex.SetPixel(texX + texOffsetX, tex.height - (texY + texOffsetY) - 1, c.GetColor());
                }
            }
        }

        public Texture2D GetTexture(PS1_TIM tim, bool flipTextureY = true, Color[] palette = null)
        {
            if (tim.XPos == 0 && tim.YPos == 0)
                return null;

            var pal = palette ?? tim.Clut?.Palette?.Select(x => x.GetColor()).ToArray();

            return GetTexture(tim.ImgData, pal, tim.Width, tim.Height, tim.ColorFormat, flipTextureY);
        }

        public Texture2D GetTexture(PS1_TMD_Packet packet, PS1_VRAM vram)
        {
            if (!packet.Mode.TME)
                throw new Exception($"Packet has no texture");

            PS1_TIM.TIM_ColorFormat colFormat = packet.TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => PS1_TIM.TIM_ColorFormat.BPP_4,
                PS1_TSB.TexturePageTP.CLUT_8Bit => PS1_TIM.TIM_ColorFormat.BPP_8,
                PS1_TSB.TexturePageTP.Direct_15Bit => PS1_TIM.TIM_ColorFormat.BPP_16,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {packet.TSB.TP}")
            };
            int width = packet.TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => 256,
                PS1_TSB.TexturePageTP.CLUT_8Bit => 128,
                PS1_TSB.TexturePageTP.Direct_15Bit => 64,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {packet.TSB.TP}")
            };

            var tex = TextureHelpers.CreateTexture2D(width, 256, clear: true);

            FillTextureFromVRAM(
                tex: tex,
                vram: vram,
                width: width,
                height: 256,
                colorFormat: colFormat,
                texX: 0,
                texY: 0,
                clutX: packet.CBA.ClutX * 16,
                clutY: packet.CBA.ClutY,
                texturePageX: packet.TSB.TX,
                texturePageY: packet.TSB.TY,
                texturePageOriginX: 0,
                texturePageOriginY: 0,
                texturePageOffsetX: 0,
                texturePageOffsetY: 0,
                flipY: true);

            tex.Apply();

            return tex;
        }

        public Texture2D GetTexture(byte[] imgData, Color[] pal, int width, int height, PS1_TIM.TIM_ColorFormat colorFormat, bool flipTextureY = true)
        {
            Util.TileEncoding encoding;

            int palLength;

            switch (colorFormat)
            {
                case PS1_TIM.TIM_ColorFormat.BPP_4:
                    width *= 2 * 2;
                    encoding = Util.TileEncoding.Linear_4bpp;
                    palLength = 16;
                    break;

                case PS1_TIM.TIM_ColorFormat.BPP_8:
                    width *= 2;
                    encoding = Util.TileEncoding.Linear_8bpp;
                    palLength = 256;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            pal ??= Util.CreateDummyPalette(palLength).Select(x => x.GetColor()).ToArray();

            var tex = TextureHelpers.CreateTexture2D(width, height);

            tex.FillRegion(
                imgData: imgData,
                imgDataOffset: 0,
                pal: pal,
                encoding: encoding,
                regionX: 0,
                regionY: 0,
                regionWidth: tex.width,
                regionHeight: tex.height,
                flipTextureY: flipTextureY);

            return tex;
        }

        public async UniTask LoadFilesAsync(Context context, LoaderConfiguration config)
        {
            // The game only loads portions of the BIN at a time
            await context.AddLinearSerializedFileAsync(Loader.FilePath_BIN);
            
            // The IDX gets loaded into a fixed memory location
            await context.AddMemoryMappedFile(Loader.FilePath_IDX, 0x80010000);

            // The exe has to be loaded to read certain data from it
            await context.AddMemoryMappedFile(config.FilePath_EXE, config.Address_EXE);
        }
    }

    public class Unity_Object_PS1Klonoa : Unity_Object_3D
    {
        public Unity_Object_PS1Klonoa(BinarySerializable serializableData)
        {
            SerializableData = serializableData;
        }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override Vector3 Position { get; set; }
        public override ILegacyEditorWrapper LegacyWrapper => null;
        public override BinarySerializable SerializableData { get; }
        public override string PrimaryName => $"DUMMY";
        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => null;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;

        private bool _isUIStateArrayUpToDate;

        protected override bool IsUIStateArrayUpToDate => _isUIStateArrayUpToDate;

        protected override void RecalculateUIStates()
        {
            UIStates = new UIState[0];
            _isUIStateArrayUpToDate = true;
        }
    }
}