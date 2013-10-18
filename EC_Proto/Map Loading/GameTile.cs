using System;

namespace EC_Proto
{
	public class GameTile
	{
		public bool solid = false;
		public int ID;
		public SpriteChoice display = new SpriteChoice();

		//Still need work on entity spawning!
		//Maybe the tile could hold a reference to an entity constructor?
		//Parsing Strings might not be the best idea.
		public String SpawnType = "";
		public GameTile ()
		{

		}
	}
}

