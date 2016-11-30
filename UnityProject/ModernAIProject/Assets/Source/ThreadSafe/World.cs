using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThreadSafe
{
    public class World
    {
        private List<Resource> resources = new List<Resource>();
        public void setResources(List<Resource> resources)
        {
            this.resources = resources;
        }

        public World(List<Vector3> resourcePositions)
        {
            foreach(Vector3 pos in resourcePositions)
            {
                resources.Add(new Resource(pos));
            }
        }
        World()
        {

        }

        public Resource GetRandomResource()
        {
            List<Resource> availableRes = GetAvailableResources(); //get all available resources

            if (availableRes.Count > 0)
            {
                Resource resource = availableRes[StaticRandom.Rand(0, availableRes.Count)];

                return resource;
            }
            else
            {
                return null;
            }
        }

        //gets all the resources which are still available
        List<Resource> GetAvailableResources()
        {
            List<Resource> availableRes = new List<Resource>();

            foreach (Resource ress in resources)
            {
                if (!ress.isTaken())
                {
                    availableRes.Add(ress);
                }
            }

            return availableRes;
        }

        public bool resourcesAvailable()
        {
            bool available = false;

            foreach (Resource ress in resources)
            {
                if (!ress.isTaken())
                {
                    available = true;
                    break;
                }
            }

            return available;
        }
        public void RemoveFromResourcePool(Resource resource)
        {
            foreach (Resource ress in resources)
            {
                if (ress.GetHashCode().Equals(resource.GetHashCode()))
                {
                    //remove resource from world - disabled for testing purposes
                    
                    resource.SetTaken(true);
                }
            }
        }

        /// <summary>
        /// Copies the world with the resources reset but still in same position
        /// </summary>
        /// <returns>copy of the world</returns>
        public World cleanCopy()
        {
            World newWorld = new World();

            List<Resource> newResources = resources;
            foreach(Resource r in newResources)
            {
                r.SetTaken(false);
            }

            newWorld.setResources(newResources);
            return newWorld;
        }
    }
}
