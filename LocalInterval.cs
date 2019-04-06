// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;
using System;
using NodaTime.Intervals.Internal;

namespace NodaTime.Intervals
{
    /// <summary>
    /// An LocalInterval between two instants in time (start and end).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The LocalInterval includes the start LocalDateTime and excludes the end LocalDateTime. However, an LocalInterval
    /// may be missing its start or end, in which case the LocalInterval is deemed to be infinite in that
    /// direction.
    /// </para>
    /// <para>
    /// The end may equal the start (resulting in an empty LocalInterval), but will not be before the start.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    public readonly struct LocalInterval : IEquatable<LocalInterval>
    {
        /// <summary>The start of the LocalInterval.</summary>
        private readonly LocalDateTime? start;

        /// <summary>The end of the LocalInterval. This will never be earlier than the start.</summary>
        private readonly LocalDateTime? end;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalInterval"/> struct.
        /// The LocalInterval includes the <paramref name="start"/> LocalDateTime and excludes the
        /// <paramref name="end"/> LocalDateTime. The end may equal the start (resulting in an empty LocalInterval), but must not be before the start.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>.</exception>
        /// <param name="start">The start <see cref="LocalDateTime"/>.</param>
        /// <param name="end">The end <see cref="LocalDateTime"/>.</param>
        public LocalInterval(LocalDateTime start, LocalDateTime end)
        {
            if (end < start)
            {
                throw new ArgumentOutOfRangeException(nameof(end), "The end parameter must be equal to or later than the start parameter");
            }
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalInterval"/> struct from two nullable <see cref="LocalDateTime"/>
        /// values.
        /// </summary>
        /// <remarks>
        /// If the start is null, the LocalInterval is deemed to stretch to the start of time. If the end is null,
        /// the LocalInterval is deemed to stretch to the end of time.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>,
        /// if they are both non-null.</exception>
        /// <param name="start">The start <see cref="LocalDateTime"/> or null.</param>
        /// <param name="end">The end <see cref="LocalDateTime"/> or null.</param>
        public LocalInterval(LocalDateTime? start, LocalDateTime? end)
        {
            this.start = start;
            this.end = end;
            if (end < start)
            {
                throw new ArgumentOutOfRangeException(nameof(end), "The end parameter must be equal to or later than the start parameter");
            }
        }

        /// <summary>
        /// Gets the start LocalDateTime - the inclusive lower bound of the LocalInterval.
        /// </summary>
        /// <remarks>
        /// This will never be later than <see cref="End"/>, though it may be equal to it.
        /// </remarks>
        /// <value>The start <see cref="LocalDateTime"/>.</value>
        /// <exception cref="InvalidOperationException">The LocalInterval extends to the start of time.</exception>
        /// <seealso cref="HasStart"/>
        public LocalDateTime Start
        {
            get
            {
                Helpers.CheckState(start.HasValue, "LocalInterval extends to start of time");
                return start.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this LocalInterval has a fixed start point, or <c>false</c> if it
        /// extends to the start of time.
        /// </summary>
        /// <value><c>true</c> if this LocalInterval has a fixed start point, or <c>false</c> if it
        /// extends to the start of time.</value>
        public bool HasStart => start != null;

        /// <summary>
        /// Gets the end LocalDateTime - the exclusive upper bound of the LocalInterval.
        /// </summary>
        /// <value>The end <see cref="LocalDateTime"/>.</value>
        /// <exception cref="InvalidOperationException">The LocalInterval extends to the end of time.</exception>
        /// <seealso cref="HasEnd"/>
        public LocalDateTime End
        {
            get
            {
                Helpers.CheckState(end.HasValue, "LocalInterval extends to end of time");
                return end.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this LocalInterval has a fixed end point, or <c>false</c> if it
        /// extends to the end of time.
        /// </summary>
        /// <value><c>true</c> if this LocalInterval has a fixed end point, or <c>false</c> if it
        /// extends to the end of time.</value>
        public bool HasEnd => end.HasValue;

        /// <summary>
        /// Returns the Period of the LocalInterval.
        /// </summary>
        /// <remarks>
        /// This will always be a non-negative Period, though it may be zero.
        /// </remarks>
        /// <value>The Period of the LocalInterval.</value>
        /// <exception cref="InvalidOperationException">The LocalInterval extends to the start or end of time.</exception>
        public Period Period => End - Start;
        
        /// <summary>
        /// Returns the Duration of the LocalInterval.
        /// </summary>
        /// <remarks>
        /// This will always be a non-negative Duration, though it may be zero.
        /// </remarks>
        /// <value>The Duration of the LocalInterval.</value>
        /// <exception cref="InvalidOperationException">The LocalInterval extends to the start or end of time.</exception>

        public Duration Duration => Period.ToDuration();

        public TimeInterval TimeInterval => new TimeInterval(start?.TimeOfDay, end?.TimeOfDay);

        /// <summary>
        /// Returns whether or not this LocalInterval contains the given LocalDateTime.
        /// </summary>
        /// <param name="LocalDateTime">LocalDateTime to test.</param>
        /// <returns>True if this LocalInterval contains the given LocalDateTime; false otherwise.</returns>
        [Pure]
        public bool Contains(LocalDateTime LocalDateTime) => LocalDateTime >= start && LocalDateTime < end;

        /// <summary>
        /// Deconstruct this value into its components.
        /// </summary>
        /// <param name="_start">The start of the LocalInterval.</param>
        /// <param name="_end">The end of the LocalInterval.</param>
        [Pure]
        public void Deconstruct(out LocalDateTime? _start, out LocalDateTime? _end)
        {
            _start = start;
            _end = end;
          }

        #region Implementation of IEquatable<LocalInterval>
        /// <summary>
        /// Indicates whether the value of this LocalInterval is equal to the value of the specified LocalInterval.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this LocalDateTime is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(LocalInterval other) => start == other.start && end == other.end;
        #endregion

        #region object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is LocalInterval other && Equals(other);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => Helpers.Hash(start, end);
        /// <summary>
        /// Returns a string representation of this LocalInterval, in extended ISO-8601 format: the format
        /// is "start/end" where each LocalDateTime uses a format of "uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF'Z'".
        /// If the start or end is infinite, the relevant part uses "StartOfTime" or "EndOfTime" to
        /// represent this.
        /// </summary>
        /// <returns>A string representation of this LocalInterval.</returns>
        public override string ToString()
        {
            var pattern = LocalDateTimePattern.ExtendedIso;
            return (start.HasValue ? pattern.Format(start.Value) : "-") 
                   + "/" 
                   + (end.HasValue ? pattern.Format(end.Value) : "-");
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(LocalInterval left, LocalInterval right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(LocalInterval left, LocalInterval right) => !(left == right);
        #endregion

    }
}
