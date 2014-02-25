using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public abstract class MonsterPatrolEntity : MonsterEntity
	{
		public List<Vector2> nodeList = new List<Vector2> ();
		private Vector2 currentNode;
		private int nodeIndex;

		private float speed = 5;
		private Vector2 moveDirection = new Vector2 (0, 0);

		public MonsterPatrolEntity () {
			nodeIndex = 0;
			currentNode = nodeList [nodeIndex];
		}

		override public void Update(KeyboardState state, GameTime time) {
			if (moveDirection.X > 0) {
				if (position.X >= currentNode.X) {
					moveDirection.X = 0;
					position.X = currentNode.X;
					GetNextNode (currentNode);
				} else {
					position.X += moveDirection.X;
				}
			}
			else if (moveDirection.X < 0) {
				if (position.X <= currentNode.X) {
					moveDirection.X = 0;
					position.X = currentNode.X;
					GetNextNode (currentNode);
				} else {
					position.X += moveDirection.Y;
				}
			}
			if (moveDirection.Y > 0) {
				if (position.Y >= currentNode.Y) {
					moveDirection.Y = 0;
					position.Y = currentNode.Y;
					GetNextNode (currentNode);
				} else {
					position.Y += moveDirection.Y;
				}
			}
			else if (moveDirection.Y < 0) {
				if (position.Y <= currentNode.Y) {
					moveDirection.Y = 0;
					position.Y = currentNode.Y;
					GetNextNode (currentNode);
				} else {
					position.Y += moveDirection.Y;
				}
			}
		}

		public void GetNextNode(Vector2 currentNode) {
			nodeIndex++;

			if (nodeIndex >= nodeList.Count)
				nodeIndex = 0;

			if (nodeList [nodeIndex].X > currentNode.X)
				moveDirection.X = speed;
			else if (nodeList [nodeIndex].X < currentNode.X)
				moveDirection.X = -speed;
			if (nodeList [nodeIndex].Y > currentNode.Y)
				moveDirection.Y = speed;
			else if (nodeList [nodeIndex].Y < currentNode.Y)
				moveDirection.Y = -speed;

			currentNode = nodeList [nodeIndex];
		}
	}
}