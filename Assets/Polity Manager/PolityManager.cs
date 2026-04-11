using System;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

namespace Polities
{
    [DisallowMultipleComponent]
    public class Manager : MonoBehaviour
    {
        public static Manager Singleton { get; private set; }
        [Tooltip("The largest, most important organizational unit in your game.")]
        public Faction[] factions = new Faction[0];
        public PolityRelation[,] RelationMatrix { get; set; }
        [SerializeField] string polityRelationMatrixString = "";
        public enum PolityRelation
        {
            Neutral,
            Allies,
            Enemies,
        }

        [Serializable]
        class PolityRelationMatrixWrapper
        { public List<PolityRelation> relations = new(); public int rows, columns; }

        // public PolityUnits[] units;
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
            SerializeRelationMatrix();
            List<string> polityNames = new();
        }

        [ContextMenu("Reset Polity Relation Matrix")]
        void ResetPolityRelationMatrix()
        {
            int size = factions.Length;
            RelationMatrix = new PolityRelation[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    RelationMatrix[i, j] = PolityRelation.Neutral;
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
                PolityRelation[,] tempMatrix = new PolityRelation[factions.Length, factions.Length];
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
                CheckForDuplicatePolityNames();
            }
        }

        [ContextMenu("Find Duplicate Polity Names")]
        void CheckForDuplicatePolityNames()
        {
            Dictionary<string, int> nameIndex = new();
            for (int i = 0; i < factions.Length; i++)
            {
                // if (nameIndex.ContainsKey(factions[i].name))
                //     Debug.LogWarning($"Duplicate name found: {factions[i].name} at {i}");
                // else nameIndex[factions[i].name] = i;
            }
        }
        [ContextMenu("Load Polity Relation Matrix")]
        // Unity can't serialize & deserialize matrices, so this is a custom approach around it.
        public void LoadRelationMatrix()
        {
            if (RelationMatrix == null)
            {
                int ln = factions.Length;
                RelationMatrix = new PolityRelation[ln, ln];
            }
            RelationMatrix = DeserializeRelationMatrix();
        }

        /* -------------------------------------------------------------------------- */
        /*                             PUBLIC API METHODS                             */
        /* -------------------------------------------------------------------------- */

        /* ------------------------------- SERIALIZERS ------------------------------ */
        public string SerializeRelationMatrix(PolityRelation[,] polityRelationMatrix)
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

        public PolityRelation[,] DeserializeRelationMatrix(string json)
        {
            if (json.Equals("") || json == null) return null;
            PolityRelationMatrixWrapper wrapper = JsonUtility.FromJson<PolityRelationMatrixWrapper>(json);
            PolityRelation[,] matrix = new PolityRelation[wrapper.rows, wrapper.columns];
            int index = 0;
            for (int i = 0; i < wrapper.rows; i++)
                for (int j = 0; j < wrapper.columns; j++)
                    matrix[i, j] = wrapper.relations[index++];
            return matrix;
        }
        public PolityRelation[,] DeserializeRelationMatrix() =>
            DeserializeRelationMatrix(polityRelationMatrixString);
        #region Getters
        /* --------------------------------- GETTERS -------------------------------- */
        // / <summary>
        // / Gets the current PolityRelation from one PolityMember to another.
        // / </summary>
        // / <returns>The PolityRelation enum as Neutral, Allies, or Enemies.</returns>
        public PolityRelation CheckRelation(Member member, Member otherMember) =>
               CheckRelation(member.polity.name, otherMember.polity.name);
        public PolityRelation CheckRelation(string yourPolityName, string theirPolityName)
        {
            if (yourPolityName.Equals(theirPolityName)) return PolityRelation.Allies;
            int yourIndex = Array.FindIndex(factions, p => p.name == yourPolityName);
            int theirIndex = Array.FindIndex(factions, p => p.name == theirPolityName);
            if (yourIndex == -1 || theirIndex == -1)
            { Debug.Log("One or both polity names not found."); return default; }

            PolityRelation relation = RelationMatrix[yourIndex, theirIndex];
            return relation;
        }

        public Texture2D GetPolityEmblem(Polity _struct)
        {
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return null; }
            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                {
                    // emblem = polity.emblem;
                    if (!string.IsNullOrEmpty(_struct.coalitionName))
                    {
                        // foreach (var polityClass in polity.classes)
                        //     if (_struct.className.Equals(polityClass.name))
                        //     {
                        //         emblem = polityClass.emblem;
                        //         if (!string.IsNullOrEmpty(_struct.factionName))
                        //         {
                        //             foreach (var faction in polityClass.factions)
                        //                 if (_struct.factionName.Equals(faction.name))
                        //                     return faction.emblem;
                        //             Debug.LogError("No Faction Found");
                        //             return emblem;
                        //         }
                        //         return emblem;
                        //     }
                        Debug.LogError("No Class Found");
                        return null;
                    }
                    return null;
                }
            Debug.LogError("No Polity Found"); return null;
        }

        public Member GetPolityLeader(Polity _struct)
        {
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return null; }
            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                {
                    // leader = polity.leader;
                    if (!string.IsNullOrEmpty(_struct.coalitionName))
                    {
                        // foreach (var polityClass in polity.classes)
                        //     if (_struct.className.Equals(polityClass.name))
                        //     {
                        //         leader = polityClass.leader;
                        //         if (!string.IsNullOrEmpty(_struct.factionName))
                        //         {
                        //             foreach (var faction in polityClass.factions)
                        //                 if (_struct.factionName.Equals(faction.name))
                        //                     return faction.leader;
                        //             Debug.LogError("No Faction Found");
                        //             return leader;
                        //         }
                        //         return leader;
                        //     }
                        Debug.LogError("No Class Found");
                    }
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
        public void ChangeRelation(string polityName, string theirPolityName, PolityRelation newRelation)
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

        public void ChangeRelation(Member member, string theirPolityName, PolityRelation newRelation)
            => ChangeRelation(member.polity.name, theirPolityName, newRelation);
        public void ChangeRelation(PolityReader reader, PolityReader theirReader, PolityRelation newRelation)
            => ChangeRelation(reader.Struct.name, theirReader.Struct.name, newRelation);
        public void ChangeRelation(Polity _struct, Polity theirStruct, PolityRelation newRelation)
            => ChangeRelation(_struct.name, theirStruct.name, newRelation);
        /// <summary>
        /// Adds a faction to a polity, requiring a matching polityName and className to work.
        /// </summary>
        public Coalition AddFactionToPolity(Polity _struct, Texture2D emblem, Member leader)
        {
            Coalition newFaction = new(_struct.name, emblem, leader);
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return null; }

            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                    if (!string.IsNullOrEmpty(_struct.coalitionName))
                    {
                        // foreach (var polityClass in polity.classes)
                        //     if (_struct.className.Equals(polityClass.name))
                        //         if (!string.IsNullOrEmpty(_struct.factionName))
                        //         {
                        //             bool factionExists = false;
                        //             foreach (var faction in polityClass.factions)
                        //                 if (_struct.factionName.Equals(faction.name))
                        //                 {
                        //                     Debug.LogWarning("Faction already exists");
                        //                     factionExists = true; break;
                        //                 }
                        //             if (!factionExists)
                        //             {
                        //                 polityClass.factions.Add(newFaction);
                        //                 OnFactionChange?.Invoke();
                        //                 return newFaction;
                        //             }
                        //             Debug.LogError("No Faction Found");
                        //         }
                        // Debug.LogError("No Class Found");
                    }
            return null;
        }
        public void AddFactionToPolity(Polity _struct) => AddFactionToPolity(_struct, null, null);

        /// <summary>
        /// Remove a faction of a polity, if the PolityStruct polityName, className and factionName all match.
        /// </summary>
        public void RemoveFactionFromPolity(Polity _struct)
        {
            if (string.IsNullOrEmpty(_struct.name))
            { Debug.LogError("No Polity Name Provided"); return; }

            foreach (var polity in factions)
                if (_struct.name.Equals(polity.name))
                {
                    if (string.IsNullOrEmpty(_struct.coalitionName))
                    { Debug.LogError("No Class Name Provided"); return; }

                    // foreach (var polityClass in polity.classes)
                    //     if (_struct.className.Equals(polityClass.name))
                    //     {
                    //         if (string.IsNullOrEmpty(_struct.factionName))
                    //         { Debug.LogError("No Faction Name Provided"); return; }
                    //         for (int i = 0; i < polityClass.factions.Count; i++)
                    //             if (_struct.factionName.Equals(polityClass.factions[i].name))
                    //             {
                    //                 polityClass.factions.RemoveAt(i);
                    //                 Debug.Log("Faction found and removed");
                    //                 OnFactionChange?.Invoke(); return;
                    //             }
                    //         return;
                    //     }
                    Debug.LogError("No Class Found"); return;
                }
        }
        #endregion

        /* -------------------------------------------------------------------------- */
        /*                             SERIALIZED CLASSES                             */
        /* -------------------------------------------------------------------------- */

        [Serializable]
        public class Faction : PolityBase
        {
            public List<Unit> units;
        }

        /// <summary>
        /// Could represent a temporary political unit, which can be added and removed at runtime.
        /// </summary>
        [Serializable]
        public class Coalition : PolityBase
        {
            public Coalition(string name, Texture2D emblem, Member leader)
            {
                base.name = name;
            }
        }

        public abstract class PolityBase
        {
            [Tooltip("The name of the political unit.")]
            public string name;
            /// <summary>
            /// Can represent a standard, vexillum, ensign, coat of arms or a team color.
            /// </summary>
            public Texture2D emblem;
            /// <summary>
            /// The leader of this specific unit, e.g. an emperor, queen or manager.
            /// </summary>
        }
    }
}
