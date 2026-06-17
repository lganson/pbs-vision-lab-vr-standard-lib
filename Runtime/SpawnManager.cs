using System.Collections.Generic;
using UnityEngine;

namespace Standard_Library
{
    public class SpawnManager
    {
        private readonly List<SpawnCandidate> spawnCandidates;

        public SpawnManager(List<SpawnCandidate> spawnCandidates)
        {
            this.spawnCandidates = spawnCandidates;
        }

        public GameObject SpawnRandomCandidate()
        {
            if(spawnCandidates.Count == 0) throw new System.Exception("No SpawnCandidates found");
            int index = Random.Range(0, spawnCandidates.Count);
            SpawnCandidate sc = spawnCandidates[index];
            return SpawnCandidate(sc, index);
        }

        public GameObject SpawnNextCandidate()
        {
            if(spawnCandidates.Count == 0) throw new System.Exception("No SpawnCandidates found");
            SpawnCandidate sc = spawnCandidates[0];
            return SpawnCandidate(sc);
        }

        private GameObject SpawnCandidate(SpawnCandidate sc, int spawnIndex = 0)
        {
            spawnCandidates.RemoveAt(spawnIndex);
            GameObject go = Object.Instantiate(sc.prefab, sc.position, Quaternion.identity);
            go.name = (sc.conditionPrefix + " " + sc.prefab.name).Trim();
            return go;
        }
        public bool SpawnCandidateAvailable()
        {
            return spawnCandidates.Count > 0;
        }
    }

    public struct SpawnCandidate
    {
        public readonly GameObject prefab;
        public readonly Vector3 position;
        public readonly string conditionPrefix;
        public SpawnCandidate(GameObject prefab, Vector3 position)
        {
            this.prefab = prefab;
            this.position = position;
            conditionPrefix = "";
        }
        public SpawnCandidate(GameObject prefab, Vector3 position, string prefix)
        {
            this.prefab = prefab;
            this.position = position;
            conditionPrefix = prefix.Trim();
        }
    }
}