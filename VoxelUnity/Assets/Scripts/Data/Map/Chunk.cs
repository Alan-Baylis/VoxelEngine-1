﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms;
using UnityEngine;

namespace Assets.Scripts.Data.Map
{
    public class Chunk : MonoBehaviour
    {
        public const int ChunkSize = 16;
        public Mesh ChunkMesh;
        public ChunkData ChunkData;
        public float Scale = 1f;
        private Material[] _materials;

        public void InitializeChunk(Vector3 pos, ChunkData data, Material[] mats)
        {
            ChunkData = data;
            gameObject.transform.position = pos;
            _materials = mats;
            data.ChunkUpdated += chunkData =>
            {
                OnChunkUpdated();
            };
            ChunkMesh = GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>().mesh : GetComponent<MeshFilter>().mesh;
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>();
            }
            OnChunkUpdated();
        }


        protected void Load()
        {
            OnChunkUpdated();
        }

        public void OnChunkUpdated()
        {
            Dictionary<int, int[]> triangles;
            Vector3[] vertices;
            Vector3[] normals;
            GreedyMeshing.CreateMesh(out vertices, out triangles, out normals, ChunkData.Voxels, ChunkData.NeighbourBorders);
            
            ChunkMesh.Clear();
            ChunkMesh.vertices = vertices;
            ChunkMesh.normals = normals;
            ChunkMesh.subMeshCount = triangles.Keys.Count;

            var keyArray = triangles.Keys.ToArray();
            var myMats = new Material[triangles.Keys.Count];
            for (int i = 0; i < triangles.Keys.Count; i++)
            {
                ChunkMesh.SetTriangles(triangles[keyArray[i]], i);
                myMats[i] = _materials[keyArray[i]-1];
            }
            gameObject.GetComponent<MeshRenderer>().sharedMaterials = myMats;
        }
    }
}
