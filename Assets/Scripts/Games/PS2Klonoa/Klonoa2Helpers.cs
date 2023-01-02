using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.LV;
using BinarySerializer.PS2;
using Cysharp.Threading.Tasks;
using Ray1Map;
using UnityEngine;

namespace Ray1Map.PS2Klonoa
{
    public static class Klonoa2Helpers
    {
        #region VIF Geometry helpers

        public static Mesh GetMesh(this VIFGeometry_Block block)
        {
            Mesh unityMesh = new Mesh();
            unityMesh.SetVertices(block.GetVertices());
            unityMesh.SetTriangles(block.GetTriangles(), 0);
            unityMesh.SetColors(block.GetColors());
            unityMesh.SetUVs(0, block.GetUVs());
            unityMesh.RecalculateNormals();
            return unityMesh;
        }
        
        public static Vector3[] GetVertices(this VIFGeometry_Block block)
        {
            return block.Vertices.Select(x => x.GetPositionVector()).ToArray();
        }

        public static Color[] GetColors(this VIFGeometry_Block block)
        {
            return block.VertexColors.Select(x => x.GetColor()).ToArray();
        }

        public static Vector2[] GetUVs(this VIFGeometry_Block block)
        {
            return block.UVs.Select(x => x.GetUV()).ToArray();
        }

        public static int[] GetTriangles(this VIFGeometry_Block block)
        {
            List<int> triangles = new List<int>();
            int currentTriangle = 0;
            foreach (var tag in block.GIFtags) {
                bool direction = true;
                
                for (int i = 0; i < tag.NLOOP - 2; i++) {
                    triangles.Add(currentTriangle + i);
                    if (direction)
                    {
                        triangles.Add(currentTriangle + i + 1);
                        triangles.Add(currentTriangle + i + 2);
                    }
                    else
                    {
                        triangles.Add(currentTriangle + i + 2);
                        triangles.Add(currentTriangle + i + 1);
                    }
                    
                    direction = !direction;
                }
                
                currentTriangle += tag.NLOOP;
            }

            return triangles.ToArray();
        }

        public static Vector3 GetPositionVector(this VIFGeometry_Vertex vertex)
        {
            return new Vector3(vertex.X / 16f, -vertex.Y / 16f, vertex.Z / 16f);
        }
        
        public static Color GetColor(this VIFGeometry_Color color)
        {
            return new Color(color.Red, color.Green, color.Blue);
        }

        public static Vector2 GetUV(this VIFGeometry_UV uv)
        {
            return new Vector2(uv.U / 4096f, uv.V / 4096f);
        }

        public static Texture2D GetTexture(this VIFGeometry_Block block, Texture2D textureAtlas)
        {
            return textureAtlas.GetTexture(block.TEX0);
        }

        public static Vector3 GetBasePosition(this VIFGeometry_Block block)
        {
            return new Vector3(block.BasePosition.X, -block.BasePosition.Y, block.BasePosition.Z);
        }
        
        #endregion
        
        #region GS Texture helpers

        public static Dictionary<int, Texture2D> GetTextures(this GSTextures_File textures)
        {
            // TODO: This is a very janky way of doing this, figure out how to do this with GS.VRAM class
            var textureDict = new Dictionary<int, Texture2D>(); // tbp, texture_atlas
            for (int i = 0; i < textures.Packets.Length / 2; i++)
            {
                var texturePacket = textures.Packets[i * 2];
                var palettePacket = textures.Packets[i * 2 + 1];
                var palette = palettePacket.Palette.Select(x => x.GetColor()).ToArray();

                if (!textureDict.TryGetValue(texturePacket.BITBLTBUF.DBP, out var texture))
                {
                    texture = TextureHelpers.CreateTexture2D(512, 512, true, true);
                    textureDict[texturePacket.BITBLTBUF.DBP] = texture;
                }

                int width = texturePacket.TRXREG.RRW;
                int height = texturePacket.TRXREG.RRH;
                int startX = texturePacket.TRXPOS.DSAX;
                int startY = texturePacket.TRXPOS.DSAY;
                if (texturePacket.BITBLTBUF.DPSM == GS.PixelStorageMode.PSMT4)
                {
                    texture.FillRegion(texturePacket.ImgData, 0, palette,
                        Util.TileEncoding.Linear_4bpp, startX, startY, width, height);
                }
                else
                {
                    texture.FillRegion(texturePacket.ImgData, 0, palette, 
                        Util.TileEncoding.Linear_8bpp, startX, startY, width, height, paletteFunction: (
                            (b, x, y) =>
                            {
                                // Deswizzle palette
                                int mod = b % 0x20;
                                if (mod >= 0x08 && mod <= 0x0F)
                                    b += 0x08;
                                else if (mod >= 0x10 && mod <= 0x17)
                                    b -= 0x08;
                                return palette[b];
                            }));
                }
            }

            return textureDict;
        }

        public static Texture2D GetTexture(this Texture2D textureAtlas, GSReg_TEX0_1 tex0)
        {
            int width = (int)Math.Pow(2, tex0.TW);
            int height = (int)Math.Pow(2, tex0.TH);
            return textureAtlas.Crop(new RectInt(0, 0, width, height), false, false);
        }

        public static Texture2D GetTexture(this Dictionary<int, Texture2D> textures, GSReg_TEX0_1 tex0)
        {
            return textures[tex0.TPB0].GetTexture(tex0);
        }

        #endregion
    }
}