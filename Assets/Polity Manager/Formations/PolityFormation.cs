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
        private Leader leader;
        private readonly List<IMember> members = new();
        private readonly Dictionary<IMember, Vector3> offsets = new();

        // Expose read-only view for the editor drawer
        public IReadOnlyDictionary<IMember, Vector3> FormationOffsets => offsets;
        public Leader Leader => leader;

        // ── Setup ────────────────────────────────────────────────────────────────
        public Formation(Leader leader, int columns = 3, int rows = 2, float spacing = 1.5f)
        {
            this.leader = leader;
            this.columns = columns;
            this.rows = rows;
            this.spacing = spacing;
        }

        // ── Public API ───────────────────────────────────────────────────────────
        public void Add(IMember member)
        {
            if (offsets.ContainsKey(member)) return;

            members.Add(member);
            RebuildOffsets();          // recalculate all slots whenever the count changes
        }

        public void Remove(IMember member)
        {
            if (!offsets.ContainsKey(member)) return;

            members.Remove(member);
            offsets.Remove(member);
            RebuildOffsets();
        }

        /// <summary>
        /// Called every frame from Leader.Update().
        /// Drives each member toward its world-space formation slot.
        /// </summary>
        public void Update()
        {
            Debug.Log("Formation position size " + offsets.Count);
            foreach (var (member, offset) in offsets)
            {
                Vector3 worldTarget = GetPosition(member);
                // member.MoveTowards(worldTarget);   // IMember decides how to move (NavMesh, Rigidbody, etc.)
            }
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
            List<Vector3> slots = FormationShape.CreateSquareFormation(columns, rows, spacing);

            offsets.Clear();
            for (int i = 0; i < members.Count && i < slots.Count; i++)
            {
                offsets[members[i]] = slots[i];
            }
        }
    }
}