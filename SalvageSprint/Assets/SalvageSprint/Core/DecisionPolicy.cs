namespace SalvageSprint.Core
{
    // Week 1 assignment starter. Work through README stages in this same project.
    // STAGE 2: distinguish visible candidates from unknown hidden objects.
    // STAGE 3: retain a usable remembered target. STAGE 4: use failed-route feedback.
    // Delivery is supplied so the unmodified game has a complete scoring loop.
    public static class DecisionPolicy
    {
        public const string Label = "Part 1 starter: nearest candidate";

        // Infer the agent's facts from its observation. This is a simple, naive implementation that ignores visibility and memory.
        public static AgentFacts Infer(Observation observation)
        {
            if (observation.Carrying)
                return new AgentFacts(true, -1, false);

            int retainedId = observation.CurrentTargetId;

            // retain current target if still valid
            if (retainedId >= 1)
            {
                foreach (Candidate candidate in observation.Candidates)
                {
                    if (candidate.Id == retainedId)
                    {
                        // seeks candidate with retained id regardless if a closer object is discovered
                        if (IsEligible(candidate))
                        {
                            return new AgentFacts(false, retainedId, true);

                        }

                        // if the retained target is no longer eligible (e.g. picked up by player), break and seek new object
                        break;
                    }
                }
            }

            int nearest = -1;
            float distance = float.PositiveInfinity;
            foreach (Candidate candidate in observation.Candidates)
            {
                // Deliberately naive: ignores Visible, Remembered and Blocked.
                if (!IsEligible(candidate))
                {
                    continue;
                }

                if (candidate.DistanceSquared < distance)
                {
                    nearest = candidate.Id;
                    distance = candidate.DistanceSquared;
                }
            }
            return new AgentFacts(false, nearest, false);
        }

        private static bool IsEligible(Candidate candidate)
        {
            return candidate.Available && (candidate.Visible || candidate.Remembered);
        }

        public static Decision Choose(Observation observation) => Choose(Infer(observation));

        public static Decision Choose(AgentFacts facts)
        {
            // Deliberately naive: ignores RetainedTarget and any other facts.
            if (facts.NeedsDelivery)
                return new Decision(TaskKind.Deliver, -1, "Cargo held: return to depot");

            if (facts.HasUsableTarget)
            {
                string reason = facts.RetainedTarget
                    ? "Retained target. Will continue while eligible"
                    : "New candidate. Going to nearest eligible";

                return new Decision(TaskKind.Collect, facts.SelectedTargetId, reason);
            }

            return new Decision(TaskKind.Explore, -1, "No candidates: visit the next survery point");
        }
    }
}
