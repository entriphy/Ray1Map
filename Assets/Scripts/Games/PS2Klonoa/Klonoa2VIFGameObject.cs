using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.LV;
using Games.PS2Klonoa;
using Ray1Map.PS2Klonoa;
using UnityEngine;

namespace Ray1Map.PS2Klonoa
{
    public class Klonoa2VIFGameObject
    {
        public Klonoa2VIFGameObject(VIFGeometry_File geometry, Dictionary<int, Texture2D> textures, Context context)
        {
            Geometry = geometry;
            Textures = textures;
            Context = context;
        }

        public VIFGeometry_File Geometry { get; }
        private Dictionary<int, Texture2D> Textures { get; }
        public Context Context { get; }

        public GameObject CreateGameObject(string name)
        {
            GameObject gaoParent = new GameObject(name);
            gaoParent.transform.position = Vector3.zero;
            Dictionary<string, Material> materialDict = new Dictionary<string, Material>();

            for (int s = 0; s < Geometry.Sections.Length; s++)
            {
                GameObject sectionObj = new GameObject($"Section{s}");
                sectionObj.transform.SetParent(gaoParent.transform, false);
                var section = Geometry.Sections[s];
                
                var blocks = section.ParseBlocks(Context, "blocks").ToArray();
                for (int b = 0; b < blocks.Length; b++)
                {
                    VIFGeometry_Block block = blocks[b];
                    GameObject obj = new GameObject($"Section{s}_Block{b}");
                    obj.transform.SetParent(sectionObj.transform, false);
                    obj.transform.position = block.GetBasePosition();
                    obj.layer = LayerMask.NameToLayer("3D Object");
                    MeshFilter mf = obj.AddComponent<MeshFilter>();
                    MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                    mf.mesh = block.GetMesh();
                    string materialKey = $"tbp{block.TEX0.TPB0}_cbp{block.TEX0.CBP}_{block.TEX0.TW}x{block.TEX0.TH}";
                    if (!materialDict.TryGetValue(materialKey, out var material))
                    {
                        material = new Material(Controller.obj.levelController.controllerTilemap.unlitTransparentCutoutMaterial);
                        if (block.TEX0.TPB0 > 0)
                            material.SetTexture("_MainTex", Textures.GetTexture(block.TEX0));
                        materialDict[materialKey] = material;
                    }
                    
                    mr.material = material;
                }
            }
            
            return gaoParent;
        }
    }
}