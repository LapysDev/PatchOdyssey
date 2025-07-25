using UnityEditor;
using UnityEngine;

namespace Peak.Tools.Arc
{
    [RequireComponent(typeof(MeshFilter))]
    public class ArcCreator : MonoBehaviour
    {
        [Header("References")]
        public MeshFilter meshFilter = null!;

        [Header("Settings - Gizmos")]
        public bool DrawGizmos = true;
        public bool EnableLabels = true;

        [Header("Settings - Mesh")]
        public bool CreateMesh = true;
        public Transform originTransform = null!;
        [Range(0f, 360f)]public float angle;
        public float length;
        [Min(0.001f)] public float startRadius = 0f;
        public int segmentCount = 10;

        private Vector3[] verticies = System.Array.Empty<Vector3>();
        private int[] triangles = System.Array.Empty<int>();

        private void OnValidate()
        {
            if(CreateMesh)
                DrawMesh();
            else if(CreateMesh == false && meshFilter.sharedMesh != null)
                meshFilter.sharedMesh = null;
        }

        private void DrawMesh()
        {
            Debug.Assert(meshFilter != null, "Mesh Creation Needs Mesh Filter Component!");
            if(segmentCount < 2)
                return;

            float angleBetween = angle / (segmentCount - 1);

            verticies = new Vector3[segmentCount * 2];
            triangles = new int[(segmentCount - 1) * 6];

            int pointIndex = 0;
            int triangleIndex = 0;

            for(int i = 0; i < segmentCount; i++)
            {
                var origin = originTransform.position;
                var startPoint = origin + ((Quaternion.AngleAxis((-angle * 0.5f) + (angleBetween * i), originTransform.up) * originTransform.forward)).normalized * startRadius;

                verticies[pointIndex] = startPoint;
                pointIndex++;
                verticies[pointIndex] = startPoint.normalized * length;
                pointIndex++;

                if(i < segmentCount - 1)
                {
                    triangles[triangleIndex] = pointIndex - 2;
                    triangles[triangleIndex + 1] = pointIndex - 1;
                    triangles[triangleIndex + 2] = pointIndex;
                    triangles[triangleIndex + 3] = pointIndex;
                    triangles[triangleIndex + 4] = pointIndex - 1;
                    triangles[triangleIndex + 5] = pointIndex + 1;

                    triangleIndex += 6;
                }
            }

            var mesh = new Mesh { 
                name = "Procedural Mesh",
                vertices = verticies,
                triangles = triangles};

            mesh.RecalculateNormals();
            meshFilter!.sharedMesh = mesh;
            // UnityEngine.Debug.Log($"[Arc Creator]: {verticies.Length} {triangles.Length}");
            // UnityEngine.Debug.Log($"[Arc Creator]: [{string.Join(", ", verticies)}] [{string.Join(", ", triangles)}]");
        }

        private void OnDrawGizmos()
        {
            if(DrawGizmos == false && EnableLabels == false) 
                return;

            float angleBetween = angle / (segmentCount - 1);

            for(int i = 0; i < segmentCount; i++)
            {
                var startVector = originTransform.position + ((Quaternion.AngleAxis((-angle * 0.5f) + (angleBetween * i), originTransform.up) * originTransform.forward)).normalized * startRadius;

                if(EnableLabels)
                    Handles.Label(startVector, (-angle * 0.5f + (angleBetween * i)).ToString());
                
                if(DrawGizmos)
                    Debug.DrawRay(startVector, startVector + (startVector.normalized * length), Color.black);
            }
        }
    }
}
