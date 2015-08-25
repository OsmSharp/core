using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE

// ReSharper disable CheckNamespace
namespace OsmSharp
// ReSharper restore CheckNamespace
{
    // Summary:
    //     Provides the base interface for the abstraction of sets.
    //
    // Type parameters:
    //   T:
    //     The type of elements in the set.
    public interface ISet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        //
        // Summary:
        //     Removes all elements in the specified collection from the current set.
        //
        // Parameters:
        //   other:
        //     The collection of items to remove from the set.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        void ExceptWith(IEnumerable<T> other);
        //
        // Summary:
        //     Modifies the current set so that it contains only elements that are also
        //     in a specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        void IntersectWith(IEnumerable<T> other);
        //
        // Summary:
        //     Determines whether the current set is a property (strict) subset of a specified
        //     collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Returns:
        //     true if the current set is a correct subset of other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        bool IsProperSubsetOf(IEnumerable<T> other);
        //
        // Summary:
        //     Determines whether the current set is a correct superset of a specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Returns:
        //     true if the System.Collections.Generic.ISet<T> object is a correct superset
        //     of other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        bool IsProperSupersetOf(IEnumerable<T> other);
        //
        // Summary:
        //     Determines whether a set is a subset of a specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Returns:
        //     true if the current set is a subset of other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        bool IsSubsetOf(IEnumerable<T> other);
        //
        // Summary:
        //     Determines whether the current set is a superset of a specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Returns:
        //     true if the current set is a superset of other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        bool IsSupersetOf(IEnumerable<T> other);
        //
        // Summary:
        //     Determines whether the current set overlaps with the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Returns:
        //     true if the current set and other share at least one common element; otherwise,
        //     false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        bool Overlaps(IEnumerable<T> other);
        //
        // Summary:
        //     Determines whether the current set and the specified collection contain the
        //     same elements.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Returns:
        //     true if the current set is equal to other; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        bool SetEquals(IEnumerable<T> other);
        //
        // Summary:
        //     Modifies the current set so that it contains only elements that are present
        //     either in the current set or in the specified collection, but not both.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        void SymmetricExceptWith(IEnumerable<T> other);
        //
        // Summary:
        //     Modifies the current set so that it contains all elements that are present
        //     in both the current set and in the specified collection.
        //
        // Parameters:
        //   other:
        //     The collection to compare to the current set.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     other is null.
        void UnionWith(IEnumerable<T> other);
    }
}
#endif