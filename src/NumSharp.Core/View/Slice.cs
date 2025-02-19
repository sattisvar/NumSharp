﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NumSharp
{
    /// <summary>
    /// NDArray can be indexed using slicing
    /// A slice is constructed by start:stop:step notation
    /// 
    /// Examples: 
    /// 
    /// a[start:stop]  # items start through stop-1
    /// a[start:]      # items start through the rest of the array
    /// a[:stop]       # items from the beginning through stop-1
    /// 
    /// The key point to remember is that the :stop value represents the first value that is not 
    /// in the selected slice. So, the difference between stop and start is the number of elements 
    /// selected (if step is 1, the default).
    /// 
    /// There is also the step value, which can be used with any of the above:
    /// a[:]           # a copy of the whole array
    /// a[start:stop:step] # start through not past stop, by step
    /// 
    /// The other feature is that start or stop may be a negative number, which means it counts 
    /// from the end of the array instead of the beginning. So:
    /// a[-1]    # last item in the array
    /// a[-2:]   # last two items in the array
    /// a[:-2]   # everything except the last two items
    /// Similarly, step may be a negative number:
    /// 
    /// a[::- 1]    # all items in the array, reversed
    /// a[1::- 1]   # the first two items, reversed
    /// a[:-3:-1]  # the last two items, reversed
    /// a[-3::- 1]  # everything except the last two items, reversed
    /// 
    /// NumSharp is kind to the programmer if there are fewer items than 
    /// you ask for. For example, if you  ask for a[:-2] and a only contains one element, you get an 
    /// empty list instead of an error.Sometimes you would prefer the error, so you have to be aware 
    /// that this may happen.
    /// 
    /// Adapted from Greg Hewgill's answer on Stackoverflow: https://stackoverflow.com/questions/509211/understanding-slice-notation
    /// 
    /// Note: special IsIndex == true
    /// It will pick only a single value at Start in this dimension effectively reducing the Shape of the sliced matrix by 1 dimension. 
    /// It can be used to reduce an N-dimensional array/matrix to a (N-1)-dimensional array/matrix
    /// 
    /// Example:
    /// a=[[1, 2], [3, 4]]
    /// a[:, 1] returns the second column of that 2x2 matrix as a 1-D vector
    /// </summary>
    [DebuggerStepThrough]
    public class Slice
    {
        public static readonly Slice All = new Slice(null, null);

        public static readonly Slice None = new Slice(0, 0, 1);

        public int? Start;
        public int? Stop;
        public int Step;
        public bool IsIndex;
        public bool IsEllipsis;
        public bool IsNewAxis;

        /// <summary>
        /// Length of the slice. 
        /// <remarks>
        /// The length is not guaranteed to be known for i.e. a slice like ":". Make sure to check Start and Stop 
        /// for null before using it</remarks>
        /// </summary>
        public int? Length => Stop - Start;

        /// <summary>
        /// ndarray can be indexed using slicing
        /// slice is constructed by start:stop:step notation
        /// </summary>
        /// <param name="start">Start index of the slice, null means from the start of the array</param>
        /// <param name="stop">Stop index (first index after end of slice), null means to the end of the array</param>
        /// <param name="step">Optional step to select every n-th element, defaults to 1</param>
        public Slice(int? start = null, int? stop = null, int step = 1)
        {
            Start = start;
            Stop = stop;
            Step = step;
        }

        public Slice(string slice_notation)
        {
            Parse(slice_notation);
        }

        /// <summary>
        /// Parses Python array slice notation and returns an array of Slice objects
        /// </summary>
        public static Slice[] ParseSlices(string multi_slice_notation)
        {
            return Regex.Split(multi_slice_notation, @",\s*").Where(s => !string.IsNullOrWhiteSpace(s)).Select(token => new Slice(token)).ToArray();
        }

        /// <summary>
        /// Creates Python array slice notation out of an array of Slice objects (mainly used for tests)
        /// </summary>
        public static string FormatSlices(params Slice[] slices)
        {
            return string.Join(",", slices.Select(s => s.ToString()));
        }

        private void Parse(string slice_notation)
        {
            if (slice_notation == "...")
            {
                Start = 0;
                Stop = 0;
                Step = 1;
                IsEllipsis = true;
                return;
            }
            else if (slice_notation == "")
            {
                Start = 0;
                Stop = 0;
                Step = 1;
                IsNewAxis = true;
                return;
            }

            if (string.IsNullOrEmpty(slice_notation))
                throw new ArgumentException("Slice notation expected, got empty string or null");
            var match = Regex.Match(slice_notation, @"^\s*([+-]?\s*\d+)?\s*:\s*([+-]?\s*\d+)?\s*(:\s*([+-]?\s*\d+)?)?\s*$|^\s*([+-]?\s*\d+)\s*$");
            if (!match.Success)
                throw new ArgumentException("Invalid slice notation");
            var start_string = Regex.Replace(match.Groups[1].Value ?? "", @"\s+", ""); // removing spaces from match to be able to parse what python allows, like: "+ 1" or  "-   9";
            var stop_string = Regex.Replace(match.Groups[2].Value ?? "", @"\s+", "");
            var step_string = Regex.Replace(match.Groups[4].Value ?? "", @"\s+", "");
            var single_pick_string = match.Groups[5].Value;
            if (!string.IsNullOrWhiteSpace(single_pick_string))
            {
                if (!int.TryParse(Regex.Replace(single_pick_string ?? "", @"\s+", ""), out var start))
                    throw new ArgumentException($"Invalid value for start: {start_string}");
                Start = start;
                Stop = start + 1;
                Step = 1; // special case for dimensionality reduction by picking a single element
                IsIndex = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(start_string))
                Start = null;
            else
            {
                if (!int.TryParse(start_string, out var start))
                    throw new ArgumentException($"Invalid value for start: {start_string}");
                Start = start;
            }

            if (string.IsNullOrWhiteSpace(stop_string))
                Stop = null;
            else
            {
                if (!int.TryParse(stop_string, out var stop))
                    throw new ArgumentException($"Invalid value for start: {stop_string}");
                Stop = stop;
            }

            if (string.IsNullOrWhiteSpace(step_string))
                Step = 1;
            else
            {
                if (!int.TryParse(step_string, out var step))
                    throw new ArgumentException($"Invalid value for start: {step_string}");
                Step = step;
            }
        }

        #region Equality comparison

        public static bool operator ==(Slice a, Slice b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Start == b.Start && a.Stop == b.Stop && a.Step == b.Step;
        }

        public static bool operator !=(Slice a, Slice b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(Slice))
                return false;

            var b = (Slice)obj;
            return Start == b.Start && Stop == b.Stop && Step == b.Step;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Slice Index(int index) => new Slice(index, index + 1) {IsIndex = true};

        public override string ToString()
        {
            if (IsIndex)
                return $"{Start ?? 0}";
            else if (IsNewAxis)
                return "newaxis";
            else if (IsEllipsis)
                return "ellipsis";
            var optional_step = Step == 1 ? "" : $":{Step}";
            return $"{(Start == 0 ? "" : Start.ToString())}:{(Stop == null ? "" : Stop.ToString())}{optional_step}";
        }

        // return the size of the slice, given the data dimension on this axis
        // note: this works only with sanitized shapes!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSize()
        {
            var astep = Math.Abs(Step);
            return (Math.Abs(Start.Value - Stop.Value) + (astep - 1)) / astep;
        }

        /// <summary>
        /// Converts the user Slice into an internal SliceDef which is easier to calculate with
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)768)]
        public SliceDef ToSliceDef(int dim)
        {
            if (IsIndex)
            {
                var index = Start ?? 0;
                if (index < 0)
                {
                    if (Math.Abs(index) > dim)
                        throw new ArgumentException($"Index {index} is out of bounds for the axis with size {dim}");
                    return new SliceDef(dim + index);
                }

                if (index > 0 && index >= dim)
                    throw new ArgumentException($"Index {index} is out of bounds for the axis with size {dim}");
                return new SliceDef(index);
            }

            if (Step == 0)
                return new SliceDef() {Count = 0, Start = 0, Step = 0};
            var astep = Math.Abs(Step);
            if (Step > 0)
            {
                var start = Start ?? 0;
                var stop = Stop ?? dim;
                if (start >= dim)
                    return new SliceDef() {Count = 0, Start = 0, Step = 0};
                if (start < 0)
                    start = Math.Abs(start) <= dim ? dim + start : 0;
                if (stop > dim)
                    stop = dim;
                if (stop < 0)
                    stop = Math.Abs(stop) <= dim ? dim + stop : 0;
                if (start >= stop)
                    return new SliceDef() {Count = 0, Start = 0, Step = 0};
                var count = (Math.Abs(start - stop) + (astep - 1)) / astep;
                return new SliceDef() {Start = start, Step = Step, Count = count};
            }
            else
            {
                // negative step!
                var start = Start ?? dim - 1;
                var stop = Stop ?? -1;
                if (start < 0)
                    start = Math.Abs(start) <= dim ? dim + start : 0;
                if (start >= dim)
                    start = dim - 1;
                if (Stop < 0)
                    stop = Math.Abs(stop) <= dim ? dim + stop : -1;
                if (start <= stop)
                    return new SliceDef() {Count = 0, Start = 0, Step = 0};
                var count = (Math.Abs(start - stop) + (astep - 1)) / astep;
                var retval = new SliceDef() {Start = start, Step = Step, Count = count};
                return retval;
            }
        }


        #region Operators

        public static Slice operator ++(Slice a)
        {
            if (a.Start.HasValue)
                a.Start++;
            if (a.Stop.HasValue)
                a.Stop++;
            return a;
        }

        public static Slice operator --(Slice a)
        {
            if (a.Start.HasValue)
                a.Start--;
            if (a.Stop.HasValue)
                a.Stop--;
            return a;
        }

        public static implicit operator Slice(int index) => new Slice(index, index + 1) {IsIndex = true};

        #endregion
    }

    public struct SliceDef
    {
        public int Start; // start index in array
        public int Step; // positive => forward from Start, 
        public int Count; // number of steps to take from Start (1 means just take Start, 0 means take nothing, -1 means this is an index)

        public SliceDef(int start, int step, int count)
        {
            (Start, Step, Count) = (start, step, count);
        }

        public SliceDef(int idx)
        {
            (Start, Step, Count) = (idx, 1, -1);
        }

        /// <summary>
        /// (Start>>Step*Count)
        /// </summary>
        /// <param name="def"></param>
        public SliceDef(string def)
        {
            if (def == "()")
            {
                (Start, Step, Count) = (0, 0, 0);
                return;
            }

            var m = Regex.Match(def, @"\((\d+)>>(-?\d+)\*(\d+)\)");
            Start = int.Parse(m.Groups[1].Value);
            Step = int.Parse(m.Groups[2].Value);
            Count = int.Parse(m.Groups[3].Value);
        }

        public bool IsIndex
        {
            [MethodImpl((MethodImplOptions)768)] get => Count == -1;
        }

        /// <summary>
        /// reverts the order of the slice sequence
        /// </summary>
        /// <returns></returns>
        [MethodImpl((MethodImplOptions)768)]
        public SliceDef Invert()
        {
            return new SliceDef() {Count = Count, Start = (Start + Step * Count), Step = -Step};
        }

        public override string ToString()
        {
            if (IsIndex)
                return $"[{Start}]";
            if (Count <= 0)
                return "()";
            return $"({Start}>>{Step}*{Count})";
        }

        /// <summary>
        /// Merge calculates the resulting one-time slice on the original data if it is sliced repeatedly
        /// </summary>
        [MethodImpl((MethodImplOptions)768)]
        public SliceDef Merge(SliceDef other)
        {
            if (other.Count == 0)
                return new SliceDef() {Start = 0, Step = 0, Count = 0};
            var self = this;
            if (other.IsIndex)
                return new SliceDef(self.Start + other.Start * self.Step);
            var result = new SliceDef() {Start = self.Start + other.Start * self.Step, Step = Step * other.Step, Count = other.Count,};
            return result;
        }
    }
}
