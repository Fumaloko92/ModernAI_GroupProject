using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThreadSafe
{
    public class World
    {
        private List<Resource> ressources = new List<Resource>();
        

        public World(List<Vector3> resourcePositions)
        {
            foreach(Vector3 pos in resourcePositions)
            {
                ressources.Add(new Resource(pos));
            }
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

            foreach (Resource ress in ressources)
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

            foreach (Resource ress in ressources)
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
            foreach (Resource ress in ressources)
            {
                if (ress.GetHashCode().Equals(resource.GetHashCode()))
                {
                    //remove resource from world - disabled for testing purposes
                    
                    resource.SetTaken(true);
                }
            }
        }
    }
}
