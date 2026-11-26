# Week 1 evidence

Keep this short. Bullet points, a small table and one diagram or code trace are enough. This is your checkpoint record, not an extra report. Replace the prompts as you work.

Name: Faye Storm  
Exact Unity version: 6000.3.26f1  
Checkpoint commit / repository: https://github.com/fayefie/FayeStorm-SalvageSprint-Assignment.git  
Current stage and anything unfinished: Week 1 complete

## Initial prediction

Three observations the rival needs, including one it might not have:
1. Seek a piece of scrap
2. Pickup and deposit scarp if found
3. Seek a new piece of scrap/move along survey route if none are seen

- I assumed it might not have the ability to switch targets if its old target was taken or disappeared

Seeing a pickup across a gap proves / does not prove reachability because: it's not certain that the rival has the ability to bridge the gap (I'm not sure I understand this question)

One question about NPC behaviour: Why does the rival stand still once it picks up the second scrap in the "Hidden pickup" scenario? (Update: this was fixed by a new update in `Navigator.cs` and `DecisionModel.cs` by the teacher)

## One complete trace

Use one actual situation. Include the observation data, derived facts, selected task and reason, movement request/result, and how feedback affects a later decision. Identify the responsible code locations. A tiny state diagram can accompany this trace.

If I am understanding this part correctly then:

Tracing what the rival code goes through when it finds a piece of scrap and carries it to the depot:
- The rival starts of from the depot and follow the given survey route until it finds an eligible target
- once the rival detects a piece of scrap within it's FOV, it runs a few checks:
  - Is the target currently available?
  - If yes, is the target either visible or previously remembered?
  - If yes, is the target not blocked?
- If all these checks pass, then the rival's current task changes from `TaskKind.Explore` to `TaskKind.Collect`
- The target's ID is set to the rival's retained ID and the rival will seek that target as long as it remains eligible
- If the target no longer becomes available by failing the `IsEligible` check - either by it becoming unavailable (for example picked up by player), fails the OR check, or becomes blocked somehow, then we move on to the next loop in the rival's code
- The code will check for another target that clears all parts of `IsEligible`, and if that target's distance squared is less than the default infinite float distance, then we set the nearest default distance to the new target, and the default -1 ID to the target's ID
- Once the scrap is picked up, current task changes from `TaskKind.Collect` to `TaskKind.Deliver`, and the rival will carry the scrap to the designated depot
- The rival then runs through the loop again.
  - If it has a visible or remembered target that are still eligible then it starts heading there
  - if not, it goes back to exploring along the survey route

## Scenario record

| Scenario | Predicted result | Observed result and short evidence / remaining issue |
|---|---|---|
| Normal loop and restart | I assumed the normal supplied loop and restart would work as intended | It did |
| Carrying cargo | Move back to depot to deposit cargo and seek new target | Rival does just that |
| Hidden / remembered pickup | Rival would follow survey route until it found a target | Rival is stuck in place. Fixed by telling rival to keep moving if a target is available and either remembered OR visible |
| Target taken | Rival would move on to next remembered/nearest target | Target does that and then becomes stuck at the depot. Fixed by telling rival to seek current target and if target is no longer eligible, move on to nearest/remembered target |
| Unreachable target | Rival would ignore the blocked target and follow the survey route until it found a new target | Rival is stuck in place if blocked target is closest target. Now fixed by telling rival to ignore blocked targets and continue along survey route |
| No usable pickup | I assumed the rival would continue following the survey points until it found a new piece of scrap/game ended | it did just that. No issues here |

Supplied test result, with any added tests identified:
Live walkthrough checked by / recording link if used: supplied in a .png that will be attached in the repo and as a file that will be submitted with `EVIDENCE.md`

## One correction and one independent check

The original weakness and why it happened: Rival stands still if closest target is blocked/out of reach. Happened because the rival had no way to handle what to do in such a case since the code doesn't check for it.
What I changed and why it addresses the cause: In my `IsEligible` method I added && !candidate.Blocked to the return, making it so that, when added to the second foreach loop, will seek the nearest target, as long as it is available AND either remembered or visible AND very importantly isn't blocked.
My counterexample or experiment: prediction, observed result, and one limitation: I'm sorry but I don't understand this part...

## Collaboration and help

People I discussed or playtested with: Cristian and Allan during group work on Wednesday.
Code, sources or substantive tools/AI assistance I used, and how I verified them: Checked my code against the supplied teacher week 1 reference and streamlined my own solutions that achieved the same purpose with more code. No other tools or AI used. 
If unfinished: what works, exact reproduction steps, and the help I need: I think it should be complete now. I've done all week 1 stages and all supplied tests pass correctly.
