using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BanditReloaded
{
    public class BanditTimerComponent : MonoBehaviour
    {
        private void FixedUpdate()
        {
            del.Clear();
            foreach (BanditTimer b in hitList)
            {
                if (b.owner)
                {
                    b.length -= Time.fixedDeltaTime;
                    if (b.length <= 0f)
                    {
                        del.Add(b);
                    }
                }
            }
            foreach (BanditTimer b in del)
            {
                hitList.Remove(b);
            }
            del.Clear();
        }
        public void ResetCooldowns()
        {
            List<SkillLocator> resetList = new List<SkillLocator>();
            foreach (BanditTimer b in hitList)
            {
                bool repeat = false;
                foreach (SkillLocator s in resetList)
                {
                    if (s == b.owner)
                    {
                        repeat = true;
                        break;
                    }
                }
                if (!repeat)
                {
                    if (b.owner && b.owner.isActiveAndEnabled)
                    {
                        resetList.Add(b.owner);
                        b.owner.ResetSkills();
                    }
                }
            }
            hitList.Clear();
        }
        public void AddTimer(SkillLocator owner, float length)
        {
            BanditTimer b = new BanditTimer(owner, length);
            hitList.Add(b);
        }
        private class BanditTimer
        {
            public SkillLocator owner;
            public float length;
            public BanditTimer(SkillLocator o, float l)
            {
                owner = o;
                length = l;
            }
        }
        private List<BanditTimer> hitList = new List<BanditTimer>();
        List<BanditTimer> del = new List<BanditTimer>();
    }
}
