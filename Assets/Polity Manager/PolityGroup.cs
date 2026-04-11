using System.Collections.Generic;
using UnityEngine;

namespace Polities
{
    public class Group
    {
        public Member leader;
        public List<Member> followers;
        public string name;
        public Texture2D emblem;

        public Group(string name, Texture2D emblem, Member leader)
        {
            this.name = name;
            this.emblem = emblem;
            this.leader = leader;
        }
        public void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}