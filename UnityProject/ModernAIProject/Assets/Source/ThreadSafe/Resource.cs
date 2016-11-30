using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThreadSafe
{
    public class Resource
    {
        Vector3 pos;
        bool taken = false;

        public Resource(Vector3 pos)
        {
            this.pos = pos;
        }
        public void SetPosition(Vector3 pos)
        {
            this.pos = pos;
        }
        public Vector3 GetPosition()
        {
            return pos;
        }
        public void SetTaken(bool taken)
        {
            this.taken = taken;
        }
        public bool isTaken()
        {
            return taken;
        }
        
    }
}
