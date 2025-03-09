using System;
using UnityEngine;
using static KhiemLuong.PolityManager;
namespace KhiemLuong
{
    [Serializable]
    public class PolityReader
    {
        // [SerializedDictionary("Scene Name", "Polity")]
        // [SerializeField] SerializedDictionary<string, PolityStruct> polities = new();
        [SerializeField] PolityStruct polityStruct;
        [SerializeField] int polityIndex, classIndex, factionIndex;
        [SerializeField] bool isPolityLeader, isClassLeader, isFactionLeader;
        public System.Collections.Generic.List<PolityMember> parents, partners, children;

        #region  Encapsulated
        /* ------------------------------ ENCAPSULATED ------------------------------ */
        public PolityStruct Struct
        { get => polityStruct; private set => polityStruct = value; }
        // public string PolityName
        // { get => polityName; private set => polityName = value; }
        // public string ClassName
        // { get => className; private set => className = value; }
        // public string FactionName
        // { get => factionName; private set => factionName = value; }
        #endregion
        #region  Get Polity
        public PolityStruct GetPolity()
        {
            return polityStruct;
        }
        #endregion


        #region Set Polity
        public void SetPolity(ref PolityStruct _struct)
        {
            polityStruct = _struct;
        }
        #endregion
        public override bool Equals(object obj)
        {
            if (obj is PolityReader other)
            {
                return string.Equals(polityStruct.polityName, other.Struct.polityName) &&
                    string.Equals(polityStruct.className
                        ?? string.Empty, other.Struct.className ?? string.Empty) &&
                    string.Equals(polityStruct.factionName
                        ?? string.Empty, other.Struct.factionName ?? string.Empty);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // return base.GetHashCode();
            return HashCode.Combine(polityStruct.polityName?.ToLowerInvariant(),
                                    polityStruct.className?.ToLowerInvariant(),
                                    polityStruct.factionName?.ToLowerInvariant());
        }
    }
}