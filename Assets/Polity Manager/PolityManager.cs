using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Polity
{
    [HelpURL("https://github.com/khiemgluong/Polity-Manager/blob/main/README.md")]

    [DisallowMultipleComponent]
    public class Manager : MonoBehaviour
    {
        public const string VERSION = "3.0.0";
        public static Manager PM { get; private set; }
        public Faction[] factions = new Faction[0];
        public Relation[,] RelationMatrix { get; private set; }
        public enum Relation
        {
            Neutral,
            Allies,
            Enemies,
        }

        /* --------------------------------- EVENTS --------------------------------- */
        public static Action OnRelationChange;
        void Awake()
        {
            if (!CheckFactionNames(out string error))
            {
                Debug.LogError($"PolityManager: {error}.", gameObject);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else                    
                    Application.Quit(1);
#endif
            }

            if (PM != null && PM != this)
                Destroy(gameObject);
            else PM = this;

            foreach (Faction faction in factions)
                faction.Name = faction.Name?.Trim();

            LoadRelationMatrix();
        }

        void OnValidate()
        {
            ValidateRelationMatrix();
            // SerializeRelationMatrix();
        }

        public bool CheckFactionNames(out string error)
        {
            int emptyCount = factions.Count(f => string.IsNullOrEmpty(f.Name));
            if (emptyCount > 0)
            {
                error = emptyCount == 1
                    ? "There is one faction with no name."
                    : $"There are {emptyCount} factions with no name";
                return false;
            }

            var duplicates = factions
                .Where(f => !string.IsNullOrEmpty(f.Name))
                .GroupBy(f => f.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                string names = string.Join(", ", duplicates.Select(n => $"\"{n}\""));
                error = duplicates.Count == 1
                    ? $"Duplicate faction name: {names}"
                    : $"Duplicate faction names: {names}";
                return false;
            }

            error = null;
            return true;
        }


        [ContextMenu("Reset Relation Matrix")]
        void ResetRelationMatrix()
        {
            int size = factions.Length;
            RelationMatrix = new Relation[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    RelationMatrix[i, j] = Relation.Neutral;
            Debug.Log("Reset Relation Matrix to default (Neutral) for all factions.");
            relationMatrixJSON = "";
            ValidateRelationMatrix();
            SerializeRelationMatrix();
        }
        void ValidateRelationMatrix()
        {
            LoadRelationMatrix();
            if (RelationMatrix == null ||
                RelationMatrix.GetLength(0) != factions.Length ||
                RelationMatrix.GetLength(1) != factions.Length)
            {
                // Create a temporary matrix to hold existing data
                Relation[,] tempMatrix = new Relation[factions.Length, factions.Length];
                if (RelationMatrix != null)
                {
                    int minRows = Mathf.Min(RelationMatrix.GetLength(0), factions.Length);
                    int minCols = Mathf.Min(RelationMatrix.GetLength(1), factions.Length);

                    for (int i = 0; i < minRows; i++)
                        for (int j = 0; j < minCols; j++)
                            tempMatrix[i, j] = RelationMatrix[i, j];
                }
                // Replace the old matrix with the new matrix of appropriate size
                RelationMatrix = tempMatrix;
            }
        }


        [ContextMenu("Load Relation Matrix")]
        public void LoadRelationMatrix()
        {
            if (RelationMatrix == null)
            {
                int ln = factions.Length;
                RelationMatrix = new Relation[ln, ln];
                // Debug.Log($"Initialized new RelationMatrix of size {ln}x{ln}.");
            }
            RelationMatrix = DeserializeRelationMatrix();
        }

        /* -------------------------------------------------------------------------- */
        /*                             PUBLIC API METHODS                             */
        /* -------------------------------------------------------------------------- */
        class RelationMatrixWrapper
        { public List<Relation> relations = new(); public int rows, columns; }
        [SerializeField] string relationMatrixJSON = "";

        /* ------------------------------- SERIALIZERS ------------------------------ */
        public string SerializeRelationMatrix(Relation[,] relationMatrix)
        {
            RelationMatrixWrapper wrapper = new()
            {
                rows = relationMatrix.GetLength(0),
                columns = relationMatrix.GetLength(1)
            };

            for (int i = 0; i < wrapper.rows; i++)
                for (int j = 0; j < wrapper.columns; j++)
                    wrapper.relations.Add(relationMatrix[i, j]);
            relationMatrixJSON = JsonUtility.ToJson(wrapper);
            return relationMatrixJSON;
        }

        public string SerializeRelationMatrix() => SerializeRelationMatrix(RelationMatrix);

        public Relation[,] DeserializeRelationMatrix(string json)
        {
            if (json.Equals("") || json == null) return null;
            RelationMatrixWrapper wrapper = JsonUtility.FromJson<RelationMatrixWrapper>(json);
            Relation[,] matrix = new Relation[wrapper.rows, wrapper.columns];
            int index = 0;
            for (int i = 0; i < wrapper.rows; i++)
                for (int j = 0; j < wrapper.columns; j++)
                    matrix[i, j] = wrapper.relations[index++];
            return matrix;
        }
        public Relation[,] DeserializeRelationMatrix() =>
            DeserializeRelationMatrix(relationMatrixJSON);
        #region Getters
        /* --------------------------------- GETTERS -------------------------------- */
        public Relation CheckRelation(int factionIndex, int theirFactionIndex)
        {
            if (factionIndex < 0 || factionIndex >= factions.Length ||
                theirFactionIndex < 0 || theirFactionIndex >= factions.Length)
            {
                Debug.LogError("One or both polity indices are out of range. Returning default relation.");
                return default;
            }
            return RelationMatrix[factionIndex, theirFactionIndex];
        }
        public Relation CheckRelation(Member member, Member otherMember) =>
               CheckRelation(member.faction.Name, otherMember.faction.Name);
        public Relation CheckRelation(IMember member, IMember otherMember) =>
                CheckRelation(member.Faction.Name, otherMember.Faction.Name);
        public Relation CheckRelation(Faction faction, Faction otherFaction) =>
                CheckRelation(faction.Name, otherFaction.Name);
        public Relation CheckRelation(string factionName, string theirFactionName)
        {
            return CheckRelation(Array.FindIndex(factions, p => p.Name == factionName),
                            Array.FindIndex(factions, p => p.Name == theirFactionName));
        }

        public int RandomFactionIndex()
        {
            if (factions == null || factions.Length == 0)
            {
                Debug.LogError("No factions available in Manager. Cannot assign random faction.");
                return -1;
            }
            return UnityEngine.Random.Range(0, factions.Length);
        }

        public string RandomFactionName()
        {
            int index = RandomFactionIndex();
            return index != -1 ? factions[index].Name : null;
        }

        public Faction GetRandomFaction()
        {
            int index = RandomFactionIndex();
            return index != -1 ? factions[index] : null;
        }

        #endregion

        #region  Setters
        /* --------------------------------- SETTERS -------------------------------- */
        public void ChangeRelation(int factionIndex, int theirFactionIndex, Relation newRelation)
        {
            if (factionIndex == theirFactionIndex)
            {
                Debug.LogWarning($"Cannot change identical polities at index {factionIndex}.");
                return;
            }
            if (factionIndex < 0 || factionIndex >= factions.Length ||
                theirFactionIndex < 0 || theirFactionIndex >= factions.Length)
            {
                Debug.LogError("One or both polity indices are out of range.");
                return;
            }
            RelationMatrix[factionIndex, theirFactionIndex] = newRelation;
            RelationMatrix[theirFactionIndex, factionIndex] = newRelation;
            OnRelationChange?.Invoke();
            Debug.Log($"Set relation between {factions[factionIndex].Name} & {factions[theirFactionIndex].Name} to {newRelation}");
        }

        public void ChangeRelation(string factionName, string theirFactionName, Relation newRelation)
        {
            ChangeRelation(Array.FindIndex(factions, p => p.Name == factionName),
                           Array.FindIndex(factions, p => p.Name == theirFactionName),
                           newRelation);
        }

        #endregion
    }
}
