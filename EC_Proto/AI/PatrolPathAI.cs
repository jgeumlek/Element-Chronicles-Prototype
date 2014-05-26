using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	public class PatrolPathAI : AI
	{
		private List<Vector2> nodePositions = new List<Vector2>();
		int currentNode;
		int nextNode;
		Vector2 moveDirection;

		PhysicsEntity actor;

		static int nextNodeWaitTime = 5000;
		TimeSpan nextNodeTimer = new TimeSpan (0, 0, 0, 0, nextNodeWaitTime); // If AI hasn't reached next node after this time, switch nodes

		bool reachedNode = false;

		static int millisecondsToWait = 500;
		TimeSpan timer = new TimeSpan (0, 0, 0, 0, millisecondsToWait);

		public PatrolPathAI (List<Vector2> nodes)
		{
			nodePositions = nodes;
			currentNode = 0;
			nextNode = 1;
		}

		public void update (PhysicsEntity actor, GameTime time) {
			this.actor = actor;
			if (timer == new TimeSpan (0, 0, 0, 0, millisecondsToWait)) {
				SetMoveDirection ();
				MoveEntity (actor);
			}

			if ((actor.position - nodePositions [nextNode]).LengthSquared() < 2) {
				reachedNode = true;
			}

			if (reachedNode) {
				nextNodeTimer = new TimeSpan (0, 0, 0, 0, nextNodeWaitTime);
				timer -= time.ElapsedGameTime;
				if (timer <= TimeSpan.Zero) {
					UpdateNodes ();
				}
			} else {
				nextNodeTimer -= time.ElapsedGameTime;
				if (nextNodeTimer <= TimeSpan.Zero) {
					UpdateNodes ();
					nextNodeTimer = new TimeSpan (0, 0, 0, 0, nextNodeWaitTime);
				}
			}
		}

		private bool HasNodes() {
			return nodePositions.Count > 0;//.Length > 0;
		}

		private void SetMoveDirection() {
			if (nodePositions.Count > 0)
				moveDirection = nodePositions [nextNode] - actor.position;
			moveDirection.Normalize ();
		}

		private void MoveEntity (PhysicsEntity actor) {
			actor.Impulse (0.2f * moveDirection);
		}

		private void UpdateNodes() {
			reachedNode = false;
			timer = new TimeSpan (0, 0, 0, 0, millisecondsToWait);
			currentNode++;
			nextNode++;
			if (nextNode == nodePositions.Count) {
				nextNode = 0;
			}
			if (currentNode == nodePositions.Count) {
				currentNode = 0;
			}
		}
	}
}

