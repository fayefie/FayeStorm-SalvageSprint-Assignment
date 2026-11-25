using System;
using System.Collections.Generic;
using SalvageSprint.Core;
using UnityEngine;

namespace SalvageSprint
{
    public sealed class PickupSensor : MonoBehaviour
    {
        [SerializeField] private MatchController match;
        [SerializeField, Min(1)] private float radius = 10;
        public float Radius => radius;
        public void Configure(MatchController owner) { match = owner; }

        /// <summary>
        /// Builds a snapshot of pickup observations for the rival's decision policy.
        /// </summary>
        /// <param name="actor">The rival whose cargo state is observed</param>
        /// <param name="rememberedId">
        /// The retained pickup ID, or -1 if no pickup is remembered. We need this to include the retained
        /// pickup in the observation even if it is currently out of range or blocked.
        /// "retained" means: the rival has previously seen this pickup and is still trying to reach it, even if it is currently out of range or blocked.
        /// </param>
        /// <param name="blocked">
        /// A function supplied by the brain. Given a pickup ID, returns true
        /// wile that pickup is temporarily blocked after a navigation failure.
        /// This method records the result; the policy decides how to use it.
        /// "blocked" means: the rival has previously tried to reach this pickup but failed, so it should be ignored for a while.
        /// </param>
        /// <returns>The rival's vargo, retained target ID and cadidate data.</returns>
        public Observation Capture(RaceActor actor, int rememberedId, Func<int, bool> isBlocked)
        {
            var candidates = new List<Candidate>();
            foreach (SalvagePickup item in match.Pickups)
            {
                if (!match.IsAvailable(item.Id)) continue;
                
                float distance = MatchController.FlatDistanceSquared(transform.position, item.transform.position);
                bool remembered = item.Id == rememberedId;
                bool inRange = distance <= radius * radius;
                
                if (!inRange && !remembered) continue;
                
                bool visible = inRange && !Physics.Linecast(transform.position + Vector3.up,
                    item.transform.position + Vector3.up * .6f,
                    match.SightBlockers, QueryTriggerInteraction.Ignore);
                
                bool pickupIsBlocked = isBlocked(item.Id);
                // Broad-phase candidates are data to process, not automatically known targets.
                candidates.Add(new Candidate(
                    id: item.Id,
                    distanceSquared: distance,
                    available: true,
                    visible: visible,
                    remembered: remembered,
                    blocked: pickupIsBlocked));
            }
            return new Observation(actor.Carrying, rememberedId, candidates.ToArray());
        }

        private void OnDrawGizmosSelected()
        { Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, radius); }
    }
}
