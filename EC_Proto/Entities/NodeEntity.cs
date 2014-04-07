using TiledMax;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class NodeEntity : Entity
	{
		public string nodeName;
		public string nextNodeName;

		public NodeEntity (Rectangle position, Properties properties) {
			this.position = new Vector2(position.X, position.Y);
			if (properties.ContainsKey("nodeName")) {
				nodeName = (string)properties["nodeName"];
			}
			if (properties.ContainsKey ("next")) {
				nextNodeName = (string)properties ["next"];
			}
		}

		public NodeEntity (String nodeName, String nextNode, Rectangle position) {
			this.position = new Vector2(position.X, position.Y);

			this.nodeName = nodeName;
			this.nextNodeName = nextNode;

		}

		override public void Update (KeyboardState state, GameTime gametime) {

		}
	}
}

