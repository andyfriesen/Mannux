using System;
using System.Runtime.Serialization;

namespace Import {
    [Serializable]
    public class MapEnt {
        public int x;
        public int y;

        public string type;	// what kind of entity
        public string[] data;	// entity specific.

        // known ent types
        public static string[] enttypes = new string[]
		{
			"player",				// not really an entity.  More like a spawning point if the game should begin on this map.  There shouldn't be more than one of these on any given map.
			
			// door types here
			// data array contents:
			// 0 - dest map
			// 1 - dest X
			// 2 - dest Y
			"door",					// basic door.  No requirements to open.
			// Make different door types for different looks, and different openning conditions.  Maybe change this.
			
			// enemy types here
			"ripper",
			"hopper",
			
			// switches etc
			
			// plot NPCs
			
			// other entities
			
		};
    }
}
