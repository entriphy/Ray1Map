using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.LV;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Games.PS2Klonoa
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
            unityMesh.RecalculateNormals();
            return unityMesh;
        }
        
        public static Vector3[] GetVertices(this VIFGeometry_Block block)
        {
            return block.Vertices.
                Select(x => x.GetPositionVector()).
                ToArray();
        }

        public static Color[] GetColors(this VIFGeometry_Block block)
        {
            return block.VertexColors.
                Select(x => x.GetColor()).
                ToArray();
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
        
        public static Color GetColor(this VIFGeometry_Color vertex)
        {
            return new Color(vertex.Red, vertex.Green, vertex.Blue);
        }

        public static Vector3 GetBasePosition(this VIFGeometry_Block block)
        {
            return new Vector3(block.BasePosition.X, -block.BasePosition.Y, block.BasePosition.Z);
        }
        
        #endregion
    }
}