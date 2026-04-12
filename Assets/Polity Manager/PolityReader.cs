using System;

namespace Polity
{
    [Serializable]
    public struct Reader : IEquatable<Reader>
    {
        public string faction;
        public string group;
        // public string company;

        public readonly bool Equals(Reader reader)
        {
            return string.Equals(faction, reader.faction, StringComparison.OrdinalIgnoreCase)
                && string.Equals(group, reader.group, StringComparison.OrdinalIgnoreCase);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Reader other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(faction?.ToLowerInvariant(), group?.ToLowerInvariant());
        }

        public void Set(string factionName, string groupName = null)
        {
            faction = factionName;
            group = groupName;
        }

        public void Set(Reader reader)
        {
            faction = reader.faction;
            group = reader.group;
        }
    }
}