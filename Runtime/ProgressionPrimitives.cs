using System;
using Deucarian.GameplayFoundation;

namespace Deucarian.Progression
{
    /// <summary>Identifier for a spendable or earnable currency.</summary>
    public readonly struct CurrencyId : IEquatable<CurrencyId>, IComparable<CurrencyId>
    {
        private readonly ContentId _value;

        /// <summary>Creates a currency identifier.</summary>
        public CurrencyId(string value)
        {
            _value = new ContentId(value);
        }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(CurrencyId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is CurrencyId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(CurrencyId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Identifier for cumulative progression such as experience or reputation.</summary>
    public readonly struct TrackId : IEquatable<TrackId>, IComparable<TrackId>
    {
        private readonly ContentId _value;

        /// <summary>Creates a track identifier.</summary>
        public TrackId(string value) { _value = new ContentId(value); }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(TrackId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is TrackId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(TrackId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Identifier for permanent content unlocks.</summary>
    public readonly struct UnlockId : IEquatable<UnlockId>, IComparable<UnlockId>
    {
        private readonly ContentId _value;

        /// <summary>Creates an unlock identifier.</summary>
        public UnlockId(string value) { _value = new ContentId(value); }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(UnlockId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is UnlockId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(UnlockId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Identifier for a ranked research or upgrade node.</summary>
    public readonly struct ResearchNodeId : IEquatable<ResearchNodeId>, IComparable<ResearchNodeId>
    {
        private readonly ContentId _value;

        /// <summary>Creates a research node identifier.</summary>
        public ResearchNodeId(string value) { _value = new ContentId(value); }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(ResearchNodeId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ResearchNodeId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(ResearchNodeId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Identifier for milestone definitions and claim records.</summary>
    public readonly struct MilestoneId : IEquatable<MilestoneId>, IComparable<MilestoneId>
    {
        private readonly ContentId _value;

        /// <summary>Creates a milestone identifier.</summary>
        public MilestoneId(string value) { _value = new ContentId(value); }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(MilestoneId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is MilestoneId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(MilestoneId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Identifier for a milestone metric such as kills, waves, or distance.</summary>
    public readonly struct MetricId : IEquatable<MetricId>, IComparable<MetricId>
    {
        private readonly ContentId _value;

        /// <summary>Creates a metric identifier.</summary>
        public MetricId(string value) { _value = new ContentId(value); }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(MetricId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is MetricId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(MetricId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Identifier used to make mutating operations idempotent.</summary>
    public readonly struct ProgressionOperationId : IEquatable<ProgressionOperationId>, IComparable<ProgressionOperationId>
    {
        private readonly ContentId _value;

        /// <summary>Creates an operation identifier.</summary>
        public ProgressionOperationId(string value) { _value = new ContentId(value); }

        /// <summary>Gets the stable identifier text.</summary>
        public string Value => _value.Value;

        /// <summary>Gets whether this identifier is unset.</summary>
        public bool IsEmpty => _value.IsEmpty;

        /// <inheritdoc />
        public bool Equals(ProgressionOperationId other) => _value.Equals(other._value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ProgressionOperationId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(ProgressionOperationId other) => _value.CompareTo(other._value);

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    /// <summary>Non-negative integral amount used by currencies, XP totals, ranks, and milestones.</summary>
    public readonly struct ProgressionAmount : IEquatable<ProgressionAmount>, IComparable<ProgressionAmount>
    {
        /// <summary>Represents zero.</summary>
        public static readonly ProgressionAmount Zero = new ProgressionAmount(0);

        /// <summary>Largest supported amount in version 0.1.0.</summary>
        public static readonly ProgressionAmount Max = new ProgressionAmount(long.MaxValue);

        /// <summary>Creates an amount after rejecting negative input.</summary>
        public ProgressionAmount(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Progression amounts cannot be negative.");
            }

            Value = value;
        }

        /// <summary>Gets the raw integral value.</summary>
        public long Value { get; }

        /// <inheritdoc />
        public bool Equals(ProgressionAmount other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ProgressionAmount other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(ProgressionAmount other) => Value.CompareTo(other.Value);

        /// <inheritdoc />
        public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>Attempts checked non-negative addition.</summary>
        public static bool TryAdd(long left, long right, out long value)
        {
            try
            {
                value = checked(left + right);
                return value >= 0;
            }
            catch (OverflowException)
            {
                value = 0;
                return false;
            }
        }

        /// <summary>Attempts checked non-negative subtraction.</summary>
        public static bool TrySubtract(long left, long right, out long value)
        {
            value = left - right;
            return right >= 0 && value >= 0 && value <= left;
        }
    }

    /// <summary>Result status for progression operations.</summary>
    public enum ProgressionStatus
    {
        /// <summary>The operation succeeded and changed state.</summary>
        Success = 0,

        /// <summary>The operation had already been applied and was ignored.</summary>
        DuplicateOperation = 1,

        /// <summary>A referenced definition was missing.</summary>
        UnknownDefinition = 2,

        /// <summary>An input amount was invalid.</summary>
        InvalidAmount = 3,

        /// <summary>The account or progression value could not afford the requested debit.</summary>
        InsufficientFunds = 4,

        /// <summary>The requested operation would overflow the configured numeric representation.</summary>
        Overflow = 5,

        /// <summary>The operation violated a definition or graph rule.</summary>
        InvalidDefinition = 6,

        /// <summary>The requested prerequisite was not met.</summary>
        MissingPrerequisite = 7,

        /// <summary>The ranked item is already at its maximum rank.</summary>
        MaxRankReached = 8,

        /// <summary>The milestone is not complete yet.</summary>
        NotCompleted = 9,

        /// <summary>The milestone reward was already claimed.</summary>
        AlreadyClaimed = 10,

        /// <summary>A transaction repeated a currency line in one atomic request.</summary>
        DuplicateLine = 11
    }
}
