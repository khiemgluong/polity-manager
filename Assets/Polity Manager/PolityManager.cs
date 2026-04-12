using System;
using UnityEngine;
using System.Collections.Generic;

namespace Polity
{
    [DisallowMultipleComponent]
    public class Manager : MonoBehaviour
    {
        public static Manager Singleton { get; private set; }
        [Tooltip("The largest, most important organizational unit in your game.")]
        public Faction[] factions = new Faction[0];
        public Relation[,] RelationMatrix { get; set; }
        [SerializeField] string polityRelationMatrixString = "";
        public enum Relation
        {
            Neutral,
            Allies,
            Enemies,
        }

        [Serializable]
        class PolityRelationMatrixWrapper
        { public List<Relation> relations = new(); public int rows, columns; }

        /* -------------------------------- FAMILIES -------------------------------- */

        /* --------------------------------- EVENTS --------------------------------- */
        public static Action OnRelationChange, OnFactionChange;
        void Awake()
        {
            if (Singleton != null && Singleton != this)
                Destroy(gameObject);
            else Singleton = this;
            LoadRelationMatrix();
            Debug.LogError($"Polity has no groups defined. Make sure to add at least one group to each polity in the Manager.");
        }

        void OnValidate()
        {
            ValidateRelationMatrix();
            SerializeRelationMatrix();
            List<string> polityNames = new();
        }

        [ContextMenu("Reset Polity Relation Matrix")]
        void ResetPolityRelationMatrix()
        {
            int size = factions.Length;
            RelationMatrix = new Relation[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    RelationMatrix[i, j] = Relation.Neutral;
            SerializeRelationMatrix();
            ValidateRelationMatrix();
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
            }
            RelationMatrix = DeserializeRelationMatrix();
        }

        /* -------------------------------------------------------------------------- */
        /*                             PUBLIC API METHODS                             */
        /* -------------------------------------------------------------------------- */

        /* ------------------------------- SERIALIZERS ------------------------------ */
        public string SerializeRelationMatrix(Relation[,] polityRelationMatrix)
        {
            PolityRelationMatrixWrapper wrapper = new()
            {
                rows = polityRelationMatrix.GetLength(0),
                columns = polityRelationMatrix.GetLength(1)
            };

            for (int i = 0; i < wrapper.rows; i++)
                for (int j = 0; j < wrapper.columns; j++)
                    wrapper.relations.Add(polityRelationMatrix[i, j]);
            polityRelationMatrixString = JsonUtility.ToJson(wrapper);
            return polityRelationMatrixString;
        }

        public string SerializeRelationMatrix() => SerializeRelationMatrix(RelationMatrix);

        public Relation[,] DeserializeRelationMatrix(string json)
        {
            if (json.Equals("") || json == null) return null;
            PolityRelationMatrixWrapper wrapper = JsonUtility.FromJson<PolityRelationMatrixWrapper>(json);
            Relation[,] matrix = new Relation[wrapper.rows, wrapper.columns];
            int index = 0;
            for (int i = 0; i < wrapper.rows; i++)
                for (int j = 0; j < wrapper.columns; j++)
                    matrix[i, j] = wrapper.relations[index++];
            return matrix;
        }
        public Relation[,] DeserializeRelationMatrix() =>
            DeserializeRelationMatrix(polityRelationMatrixString);
        #region Getters
        /* --------------------------------- GETTERS -------------------------------- */
        // / <summary>
        // / Gets the current PolityRelation from one PolityMember to another.
        // / </summary>
        // / <returns>The PolityRelation enum as Neutral, Allies, or Enemies.</returns>
        public Faction GetPolity(string polityName)
        {
            foreach (var polity in factions)
                if (polity.name.Equals(polityName))
                    return polity;
            Debug.LogError($"No polity found with name {polityName}");
            return null;
        }
        public Relation CheckRelation(Member member, Member otherMember) =>
               CheckRelation(member.faction.name, otherMember.faction.name);
        public Relation CheckRelation(string yourPolityName, string theirPolityName)
        {
            if (yourPolityName.Equals(theirPolityName)) return Relation.Allies;
            int yourIndex = Array.FindIndex(factions, p => p.name == yourPolityName);
            int theirIndex = Array.FindIndex(factions, p => p.name == theirPolityName);
            if (yourIndex == -1 || theirIndex == -1)
            { Debug.Log("One or both polity names not found."); return default; }

            Relation relation = RelationMatrix[yourIndex, theirIndex];
            return relation;
        }

        public Texture2D GetPolityEmblem(Faction _struct)
        {
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return null; }
            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                {
                    // emblem = polity.emblem;
                    // if (!string.IsNullOrEmpty(_struct.coalitionName))
                    // {
                    //     // foreach (var polityClass in polity.classes)
                    //     //     if (_struct.className.Equals(polityClass.name))
                    //     //     {
                    //     //         emblem = polityClass.emblem;
                    //     //         if (!string.IsNullOrEmpty(_struct.factionName))
                    //     //         {
                    //     //             foreach (var faction in polityClass.factions)
                    //     //                 if (_struct.factionName.Equals(faction.name))
                    //     //                     return faction.emblem;
                    //     //             Debug.LogError("No Faction Found");
                    //     //             return emblem;
                    //     //         }
                    //     //         return emblem;
                    //     //     }
                    //     Debug.LogError("No Class Found");
                    //     return null;
                    // }
                    return null;
                }
            Debug.LogError("No Polity Found"); return null;
        }

        public Member GetPolityLeader(Faction _struct)
        {
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return null; }
            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                {
                    // leader = polity.leader;
                    // if (!string.IsNullOrEmpty(_struct.coalitionName))
                    // {
                    //     // foreach (var polityClass in polity.classes)
                    //     //     if (_struct.className.Equals(polityClass.name))
                    //     //     {
                    //     //         leader = polityClass.leader;
                    //     //         if (!string.IsNullOrEmpty(_struct.factionName))
                    //     //         {
                    //     //             foreach (var faction in polityClass.factions)
                    //     //                 if (_struct.factionName.Equals(faction.name))
                    //     //                     return faction.leader;
                    //     //             Debug.LogError("No Faction Found");
                    //     //             return leader;
                    //     //         }
                    //     //         return leader;
                    //     //     }
                    //     Debug.LogError("No Class Found");
                    // }
                }
            Debug.LogError("No Polity Found"); return null;
        }
        #endregion

        #region  Setters
        /* --------------------------------- SETTERS -------------------------------- */
        /// <summary>
        /// Sets a new relation of one polity to another by their name, to FactionRelation
        /// </summary>
        /// <param name="theirPolityName">The string of the polity name that is selected.</param>
        /// <param name="newRelation">The new relation to set; Neutral, Allies or Enemies</param>
        public void ChangeRelation(string polityName, string theirPolityName, Relation newRelation)
        {
            int thisIndex = Array.FindIndex(factions, p => p.name == polityName);
            int theirIndex = Array.FindIndex(factions, p => p.name == theirPolityName);
            if (polityName.Equals(theirPolityName))
            {
                Debug.LogWarning($"Cannot change identical polities {polityName}.");
                return;
            }
            if (polityName.Equals(theirPolityName))
            {
                Debug.LogWarning($"Cannot change identical polities {polityName}.");
                return;
            }
            if (thisIndex == -1 || theirIndex == -1)
            {
                Debug.LogError("One or both polity names not found.");
                return;
            }
            RelationMatrix[thisIndex, theirIndex] = newRelation;
            RelationMatrix[theirIndex, thisIndex] = newRelation;
            OnRelationChange?.Invoke();
            Debug.Log($"Set relation between {polityName} & {theirPolityName} to {newRelation}");
        }

        public void ChangeRelation(Member member, string theirPolityName, Relation newRelation)
            => ChangeRelation(member.faction.name, theirPolityName, newRelation);
        public void ChangeRelation(PolityReader reader, PolityReader theirReader, Relation newRelation)
            => ChangeRelation(reader.Struct.name, theirReader.Struct.name, newRelation);
        public void ChangeRelation(Faction _struct, Faction theirStruct, Relation newRelation)
            => ChangeRelation(_struct.name, theirStruct.name, newRelation);
        /// <summary>
        /// Adds a faction to a polity, requiring a matching polityName and className to work.
        /// </summary>
        // public Coalition AddFactionToPolity(Polity _struct, Texture2D emblem, Member leader)
        // {
        //     Coalition newFaction = new(_struct.name, emblem, leader);
        //     if (string.IsNullOrEmpty(_struct.name))
        //     { Debug.LogError("No Polity Name Provided"); return null; }

        //     return null;
        // }
        // public void AddFactionToPolity(Polity _struct) => AddFactionToPolity(_struct, null, null);

        /// <summary>
        /// Remove a faction of a polity, if the PolityStruct polityName, className and factionName all match.
        /// </summary>
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
