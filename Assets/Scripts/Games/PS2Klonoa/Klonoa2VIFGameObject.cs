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
        public Klonoa2VIFGameObject(VIFGeometry_File geometry, Context context)
        {
            Geometry = geometry;
            Context = context;
        }

        public VIFGeometry_File Geometry { get; }
        public Context Context { get; }

        public GameObject CreateGameObject(string name)
        {
            GameObject gaoParent = new GameObject(name);
            gaoParent.transform.position = Vector3.zero;

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
                    mr.material = Controller.obj.levelController.controllerTilemap.unlitMaterial;
                }
            }
            
            return gaoParent;
        }
    }
}