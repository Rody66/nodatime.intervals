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
	/// An TimeInterval between two instants in time (start and end).
	/// </summary>
	/// <remarks>
	/// <para>
	/// The TimeInterval includes the start LocalTime and excludes the end LocalTime. However, an TimeInterval
	/// may be missing its start or end, in which case the TimeInterval is deemed to be infinite in that
	/// direction.
	/// </para>
	/// <para>
	/// The end may equal the start (resulting in an empty TimeInterval), but will not be before the start.
	/// </para>
	/// </remarks>
	/// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
	public readonly struct TimeInterval : IEquatable<TimeInterval>
	{
		/// <summary>The start of the TimeInterval.</summary>
		private readonly LocalTime? start;

		/// <summary>The end of the TimeInterval. This will never be earlier than the start.</summary>
		private readonly LocalTime? end;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeInterval"/> struct.
		/// The TimeInterval includes the <paramref name="start"/> LocalTime and excludes the
		/// <paramref name="end"/> LocalTime. The end may equal the start (resulting in an empty TimeInterval), but must not be before the start.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>.</exception>
		/// <param name="start">The start <see cref="LocalTime"/>.</param>
		/// <param name="end">The end <see cref="LocalTime"/>.</param>
		public TimeInterval(LocalTime start, LocalTime end)
		{
			if (end < start)
			{
				throw new ArgumentOutOfRangeException(nameof(end),
					"The end parameter must be equal to or later than the start parameter");
			}

			this.start = start;
			this.end = end;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeInterval"/> struct from two nullable <see cref="LocalTime"/>
		/// values.
		/// </summary>
		/// <remarks>
		/// If the start is null, the TimeInterval is deemed to stretch to the start of time. If the end is null,
		/// the TimeInterval is deemed to stretch to the end of time.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>,
		/// if they are both non-null.</exception>
		/// <param name="start">The start <see cref="LocalTime"/> or null.</param>
		/// <param name="end">The end <see cref="LocalTime"/> or null.</param>
		public TimeInterval(LocalTime? start, LocalTime? end)
		{
			this.start = start;
			this.end = end;
			if (end < start)
			{
				throw new ArgumentOutOfRangeException(nameof(end),
					"The end parameter must be equal to or later than the start parameter");
			}
		}

		/// <summary>
		/// Gets the start LocalTime - the inclusive lower bound of the TimeInterval.
		/// </summary>
		/// <remarks>
		/// This will never be later than <see cref="End"/>, though it may be equal to it.
		/// </remarks>
		/// <value>The start <see cref="LocalTime"/>.</value>
		/// <exception cref="InvalidOperationException">The TimeInterval extends to the start of time.</exception>
		/// <seealso cref="HasStart"/>
		public LocalTime Start
		{
			get
			{
				Helpers.CheckState(start.HasValue, "TimeInterval extends to start of time");
				return start.GetValueOrDefault();
			}
		}

		/// <summary>
		/// Returns <c>true</c> if this TimeInterval has a fixed start point, or <c>false</c> if it
		/// extends to the start of time.
		/// </summary>
		/// <value><c>true</c> if this TimeInterval has a fixed start point, or <c>false</c> if it
		/// extends to the start of time.</value>
		public bool HasStart => start != null;

		/// <summary>
		/// Gets the end LocalTime - the exclusive upper bound of the TimeInterval.
		/// </summary>
		/// <value>The end <see cref="LocalTime"/>.</value>
		/// <exception cref="InvalidOperationException">The TimeInterval extends to the end of time.</exception>
		/// <seealso cref="HasEnd"/>
		public LocalTime End
		{
			get
			{
				Helpers.CheckState(end.HasValue, "TimeInterval extends to end of time");
				return end.GetValueOrDefault();
			}
		}

		/// <summary>
		/// Returns <c>true</c> if this TimeInterval has a fixed end point, or <c>false</c> if it
		/// extends to the end of time.
		/// </summary>
		/// <value><c>true</c> if this TimeInterval has a fixed end point, or <c>false</c> if it
		/// extends to the end of time.</value>
		public bool HasEnd => end.HasValue;

		/// <summary>
		/// Returns the Period of the TimeInterval.
		/// </summary>
		/// <remarks>
		/// This will always be a non-negative Period, though it may be zero.
		/// </remarks>
		/// <value>The Period of the TimeInterval.</value>
		/// <exception cref="InvalidOperationException">The TimeInterval extends to the start or end of time.</exception>
		public Period Period => End - Start;

		/// <summary>
		/// Returns the Duration of the TimeInterval.
		/// </summary>
		/// <remarks>
		/// This will always be a non-negative Duration, though it may be zero.
		/// </remarks>
		/// <value>The Duration of the TimeInterval.</value>
		/// <exception cref="InvalidOperationException">The TimeInterval extends to the start or end of time.</exception>

		public Duration Duration => Period.ToDuration();

		/// <summary>
		/// Returns whether or not this TimeInterval contains the given LocalTime.
		/// </summary>
		/// <param name="LocalTime">LocalTime to test.</param>
		/// <returns>True if this TimeInterval contains the given LocalTime; false otherwise.</returns>
		[Pure]
		public bool Contains(LocalTime LocalTime) => LocalTime >= start && LocalTime < end;

		/// <summary>
		/// Deconstruct this value into its components.
		/// </summary>
		/// <param name="_start">The start of the TimeInterval.</param>
		/// <param name="_end">The end of the TimeInterval.</param>
		[Pure]
		public void Deconstruct(out LocalTime? _start, out LocalTime? _end)
		{
			_start = start;
			_end = end;
		}

		#region Implementation of IEquatable<TimeInterval>

		/// <summary>
		/// Indicates whether the value of this TimeInterval is equal to the value of the specified TimeInterval.
		/// </summary>
		/// <param name="other">The value to compare with this instance.</param>
		/// <returns>
		/// true if the value of this LocalTime is equal to the value of the <paramref name="other" /> parameter;
		/// otherwise, false.
		/// </returns>
		public bool Equals(TimeInterval other) => start == other.start && end == other.end;

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
		public override bool Equals(object obj) => obj is TimeInterval other && Equals(other);

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() => Helpers.Hash(start, end);

		/// <summary>
		/// Returns a string representation of this TimeInterval, in extended ISO-8601 format: the format
		/// is "start/end" where each LocalTime uses a format of "uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF'Z'".
		/// If the start or end is infinite, the relevant part uses "StartOfTime" or "EndOfTime" to
		/// represent this.
		/// </summary>
		/// <returns>A string representation of this TimeInterval.</returns>
		public override string ToString()
		{
			var pattern = LocalTimePattern.ExtendedIso;
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
		public static bool operator ==(TimeInterval left, TimeInterval right) => left.Equals(right);

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(TimeInterval left, TimeInterval right) => !(left == right);

		#endregion

		public LocalInterval ToLocalInterval(LocalDate date)
			=> new LocalInterval(date.At(Start), date.At(End));

		public static TimeInterval Parse(string sInterval)
		{
			var separatorIndex = sInterval.IndexOf('/');
			if (separatorIndex < 5)
				throw new InvalidOperationException("It's not a TimeInterval value");
			var pattern = LocalTimePattern.ExtendedIso;
			var sStart = sInterval.Substring(0, separatorIndex);
			var sEnd = sInterval.Substring(separatorIndex + 1, sInterval.Length - separatorIndex - 1);
			LocalTime? start = null;
			if (sStart != "-") start = pattern.Parse(sStart).Value;
			LocalTime? end = null;
			if (sEnd != "-") end = pattern.Parse(sEnd).Value;
			return new TimeInterval(start, end);
		}
	}
}