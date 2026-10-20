#nullable enable
using System;
using System.Threading.Tasks;

namespace OsmSharp.Replication
{
    /// <summary>
    /// Contains extensions methods for the replication diff enumerator.
    /// </summary>
    public static class ReplicationDiffEnumeratorExtensions
    {
        /// <summary>
        /// Moves the enumerator to the previous diff.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns></returns>
        public static async Task<bool> MovePrevious(this ReplicationDiffEnumerator enumerator)
        {
            var currentSequence = enumerator.State.SequenceNumber;
            currentSequence--;
            return await enumerator.MoveTo(currentSequence);
        }

        /// <summary>
        /// Moves the enumerator to the last diff overlapping the given timestamp.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="timestamp">The timestamp to look for.</param>
        /// <returns></returns>
        public static async Task<bool> MoveTo(this ReplicationDiffEnumerator enumerator, DateTime timestamp)
        {
            var sequenceNumber = await enumerator.Config.GuessSequenceNumberAt(timestamp);
            if (!await enumerator.MoveTo(sequenceNumber))
            {
                return false;
            }

            if (enumerator.State.Overlaps(timestamp))
            {
                return true;
            }
            
            // this is the weird case where the timestamps don't match the sequence numbers
            
            // first assume they match locally and offset them.
            var diff = (int)System.Math.Floor((timestamp - enumerator.State.EndTimestamp).TotalSeconds / enumerator.Config.Period);
            sequenceNumber += diff;
            if (!await enumerator.MoveTo(sequenceNumber))
            {
                return false;
            }
            
            // if overlap, things are fine, if not keep moving.
            var sequenceAfter = long.MaxValue;
            var sequenceBefore = long.MinValue;
            while (!enumerator.State.Overlaps(timestamp, out var before))
            {
                if (before) 
                {
                    if (sequenceAfter > enumerator.State.SequenceNumber) sequenceAfter = enumerator.State.SequenceNumber;
                }
                else
                {
                    if (sequenceBefore < enumerator.State.SequenceNumber) sequenceBefore = enumerator.State.SequenceNumber;
                }

                var window = sequenceAfter - sequenceBefore;
                if (window == 0 || window == 1)
                {
                    // both don't overlap but there is nothing in between.
                    // return the first thing after.
                    await enumerator.MoveTo(sequenceAfter);
                    return true;
                }
                
                // they don't overlap start moving up or down, we assume our heuristic is close enough already.
                if (enumerator.State.EndTimestamp > timestamp)
                {
                    if (!await enumerator.MovePrevious())
                    {
                        return false;
                    }
                }
                else
                {
                    if (enumerator.CurrentIsLatest)
                    { // don't keep searching if the latest was hit
                      // the timestamp cannot be found in this case.
                        return false;
                    }
                    if (!await enumerator.MoveNext())
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}