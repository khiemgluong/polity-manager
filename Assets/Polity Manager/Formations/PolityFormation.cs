using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    [System.Serializable]
    public class Formation
    {
        // ── Config ───────────────────────────────────────────────────────────────
        [SerializeField] private int columns = 3;
        [SerializeField] private int rows = 2;
        [SerializeField] private float spacing = 1.5f;

        // ── State ────────────────────────────────────────────────────────────────
        Leader leader;
        readonly Dictionary<IMember, Vector3> offsets = new();
        public IReadOnlyDictionary<IMember, Vector3> Offsets => offsets;

        // ── Setup ────────────────────────────────────────────────────────────────
        public Formation(Leader leader, int columns = 3, int rows = 2, float spacing = 1.5f)
        {
            this.leader = leader;
            this.columns = columns;
            this.rows = rows;
            this.spacing = spacing;
        }

        public void Add(IMember member)
        {
            if (offsets.ContainsKey(member)) return;
            RebuildOffsets();          // recalculate all slots whenever the count changes
        }

        public void Remove(IMember member)
        {
            if (!offsets.ContainsKey(member)) return;
            offsets.Remove(member);
            RebuildOffsets();
        }

        /// <summary>
        /// Called every frame from Leader.Update().
        /// Drives each member toward its world-space formation slot.
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// Returns the current world-space position for a member's slot.
        /// Rotates the stored local offset by the leader's facing direction.
        /// </summary>
        public Vector3 GetPosition(IMember member)
        {
            if (!offsets.TryGetValue(member, out Vector3 localOffset))
                return member.transform.position;

            // Rotate offset to stay relative to the leader's facing direction
            Vector3 rotatedOffset = leader.transform.rotation * localOffset;
            return leader.transform.position + rotatedOffset;
        }

        // ── Private ──────────────────────────────────────────────────────────────
        private void RebuildOffsets()
        {
            List<Vector3> slots = FormationShape.CreateSquareFormation(
                columns,
                Mathf.CeilToInt((float)leader.members.Count / columns),  // expand rows to fit
                spacing
            );
            offsets.Clear();
            for (int i = 0; i < leader.members.Count && i < slots.Count; i++)
                offsets[leader.members[i]] = slots[i];
        }
    }
}