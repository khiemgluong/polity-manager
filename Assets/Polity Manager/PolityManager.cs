using System;
using UnityEngine;
using System.Collections.Generic;

namespace Polity
{
    [DisallowMultipleComponent]
    public class Manager : MonoBehaviour
    {
        public static Manager Singleton { get; private set; }
        public Faction[] factions = new Faction[0];
        public Relation[,] RelationMatrix { get; private set; }
        public enum Relation
        {
            Neutral,
            Allies,
            Enemies,
        }


        /* --------------------------------- EVENTS --------------------------------- */
        public static Action OnRelationChange, OnFactionChange;
        void Awake()
        {
            if (Singleton != null && Singleton != this)
                Destroy(gameObject);
            else Singleton = this;
            LoadRelationMatrix();
        }

        void OnValidate()
        {
            ValidateRelationMatrix();
            // SerializeRelationMatrix();
        }

        [ContextMenu("Reset Polity Relation Matrix")]
        void ResetPolityRelationMatrix()
        {
            int size = factions.Length;
            RelationMatrix = new Relation[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    RelationMatrix[i, j] = Relation.Neutral;
            ValidateRelationMatrix();
            relationMatrixJSON = "";
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


        [ContextMenu("Load Polity Relation Matrix")]
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
        public Faction GetFaction(int index)
        {
            if (index < 0 || index >= factions.Length)
            {
                Debug.LogError($"Invalid faction index: {index}. Returning default Faction.");
                return default;
            }
            return factions[index];
        }
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
               CheckRelation(member.reader.faction, otherMember.reader.faction);
        public Relation CheckRelation(IMember member, IMember otherMember) =>
                CheckRelation(member.Reader.faction, otherMember.Reader.faction);
        public Relation CheckRelation(Reader reader, Reader otherReader) =>
                CheckRelation(reader.faction, otherReader.faction);
        public Relation CheckRelation(string factionName, string theirFactionName)
        {
            return CheckRelation(Array.FindIndex(factions, p => p.name == factionName),
                            Array.FindIndex(factions, p => p.name == theirFactionName));
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
            Debug.Log($"Set relation between {factions[factionIndex].name} & {factions[theirFactionIndex].name} to {newRelation}");
        }
        /// <summary>
        /// Sets a new relation of one polity to another by their name, to FactionRelation
        /// </summary>
        /// <param name="theirFactionName">The string of the polity name that is selected.</param>
        /// <param name="newRelation">The new relation to set; Neutral, Allies or Enemies</param>
        public void ChangeRelation(string factionName, string theirFactionName, Relation newRelation)
        {
            ChangeRelation(Array.FindIndex(factions, p => p.name == factionName),
                           Array.FindIndex(factions, p => p.name == theirFactionName),
                           newRelation);
        }

        public void AddGroup(int factionIndex, string groupName)
        {
            if (factionIndex < 0 || factionIndex >= factions.Length)
            {
                Debug.LogError($"Invalid faction index: {factionIndex}. No group added.");
                return;
            }
            if (string.IsNullOrEmpty(groupName))
            {
                Debug.LogError("Group name cannot be null or empty. No group added.");
                return;
            }
            if (factions[factionIndex].groups == null)
                factions[factionIndex].groups = new List<Group>();

            if (factions[factionIndex].groups.Exists(g => g.name == groupName))
            {
                Debug.LogWarning($"Group '{groupName}' already exists in faction '{factions[factionIndex].name}'. No duplicate group added.");
                return;
            }
            factions[factionIndex].groups.Add(new Group { name = groupName });
        }

        public void AddGroup(string factionName, string groupName)
        {
            int factionIndex = Array.FindIndex(factions, p => p.name == factionName);
            if (factionIndex == -1)
            {
                Debug.LogError($"Faction '{factionName}' not found. No group added.");
                return;
            }
            AddGroup(factionIndex, groupName);
        }

        public void RemoveGroup(int factionIndex, string groupName)
        {
            if (factionIndex < 0 || factionIndex >= factions.Length)
            {
                Debug.LogError($"Invalid faction index: {factionIndex}. No group removed.");
                return;
            }
            if (string.IsNullOrEmpty(groupName))
            {
                Debug.LogError("Group name cannot be null or empty. No group removed.");
                return;
            }
            if (factions[factionIndex].groups == null || !factions[factionIndex].groups.Exists(g => g.name == groupName))
            {
                Debug.LogWarning($"Group '{groupName}' does not exist in faction '{factions[factionIndex].name}'. No group removed.");
                return;
            }
            factions[factionIndex].groups.RemoveAll(g => g.name == groupName);
        }

        public void RemoveGroup(string factionName, string groupName)
        {
            int factionIndex = Array.FindIndex(factions, p => p.name == factionName);
            if (factionIndex == -1)
            {
                Debug.LogError($"Faction '{factionName}' not found. No group removed.");
                return;
            }
            RemoveGroup(factionIndex, groupName);
        }

        public void RemoveFactionFromPolity(Faction _struct)
        {
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return; }

            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                {

                    Debug.LogError("No Class Found"); return;
                }
        }
        #endregion

        /* -------------------------------------------------------------------------- */
        /*                             SERIALIZED CLASSES                             */
        /* -------------------------------------------------------------------------- */
        [Serializable]
        public class Faction
        {
            public string name;
            public List<Group> groups;
        }

        [Serializable]
        public class Group
        {
            public string name;
        }

    }
}
