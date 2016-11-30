using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafe
{
    class GatheringPlace
    {
        private List<Resource> resources;

        public void RemoveResource(Resource resource)
        {
            resources.Remove(resource);
        }
    }
}
