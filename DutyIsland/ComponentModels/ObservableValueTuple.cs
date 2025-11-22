using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.ComponentModels;

internal interface IValueTupleInternal : ITuple
{
    int GetHashCode(IEqualityComparer comparer);
    string ToStringEnd();
}

public class ObservableValueTuple<T1, T2> : ObservableRecipient, IEquatable<ObservableValueTuple<T1, T2>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ObservableValueTuple<T1, T2>>, IValueTupleInternal
{
    public T1 Item1;
    public T2 Item2;

    public T1 First
    {
        get => Item1;
        set
        {
            Item1 = value;
            OnPropertyChanged();
        }
    }

    public T2 Second
    {
        get => Item2;
        set
        {
            Item2 = value;
            OnPropertyChanged();
        }
    }

    public ObservableValueTuple(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ObservableValueTuple<T1, T2> tuple && Equals(tuple);
    }

    public bool Equals(ObservableValueTuple<T1, T2>? other)
    {
        return other != null
               && EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
               && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
    }

    bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer) =>
        other is ObservableValueTuple<T1, T2> vt &&
        comparer.Equals(Item1, vt.Item1) &&
        comparer.Equals(Item2, vt.Item2);

    int IComparable.CompareTo(object? other)
    {
        return other switch
        {
            null => 1,
            ObservableValueTuple<T1, T2> objTuple => CompareTo(objTuple),
            _ => throw new ArgumentException()
        };
    }

    public int CompareTo(ObservableValueTuple<T1, T2>? other)
    {
        if (other == null) return -1;
        var c = Comparer<T1>.Default.Compare(Item1, other.Item1);
        return c != 0 ? c : Comparer<T2>.Default.Compare(Item2, other.Item2);

    }

    int IStructuralComparable.CompareTo(object? other, IComparer comparer)
    {
        if (other is null) return 1;
        if (other is not ObservableValueTuple<T1, T2> objTuple) throw new ArgumentException();
        var c = comparer.Compare(Item1, objTuple.Item1);
        return c != 0 ? c : comparer.Compare(Item2, objTuple.Item2);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Item1?.GetHashCode() ?? 0,
                                Item2?.GetHashCode() ?? 0);
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        return GetHashCodeCore(comparer);
    }

    private int GetHashCodeCore(IEqualityComparer comparer)
    {
        return HashCode.Combine(comparer.GetHashCode(Item1!),
                                comparer.GetHashCode(Item2!));
    }

    int IValueTupleInternal.GetHashCode(IEqualityComparer comparer)
    {
        return GetHashCodeCore(comparer);
    }

    public override string ToString()
    {
        return "(" + Item1 + ", " + Item2 + ")";
    }
    
    string IValueTupleInternal.ToStringEnd()
    {
        return Item1 + ", " + Item2 + ")";
    }

    int ITuple.Length => 2;
    object? ITuple.this[int index] =>
        index switch
        {
            0 => Item1,
            1 => Item2,
            _ => throw new IndexOutOfRangeException(),
        };
    
    public static implicit operator ValueTuple<T1, T2>(ObservableValueTuple<T1, T2> pair)
    {
        return ValueTuple.Create(pair.Item1, pair.Item2);
    }
    
    public static implicit operator ObservableValueTuple<T1, T2>(ValueTuple<T1, T2> pair)
    {
        return new ObservableValueTuple<T1, T2>(pair.Item1, pair.Item2);
    }
}
