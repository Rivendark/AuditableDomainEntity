using AuditableDomainEntity.Events.CollectionEvents.ListEvents;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuditableDomainEntity.Collections.Lists;

public partial class AuditableList<T> : IList<T>, IList, IReadOnlyList<T>
{
    public bool IsSynchronized => false;
    public object SyncRoot => this;

    private T[] _items;
    
    private int _size;
    private int _version;
    private bool _isReadOnly;
    
#pragma warning disable CA1825 // avoid the extra generic instantiation for Array.Empty<T>()
    private static readonly T[] SEmptyArray = [];
#pragma warning restore CA1825
    
    private const int DefaultCapacity = 4;
    
    public AuditableList()
    {
        AddHydrateMethods();
        _items = SEmptyArray;
        
        AddDomainEvent(new AuditableListInitialized<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            [],
            DateTimeOffset.UtcNow));
    }
    
    public AuditableList(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative");
        }
        
        AddHydrateMethods();
        _items = capacity == 0 ? SEmptyArray : new T[capacity];
        
        AddDomainEvent(new AuditableListInitialized<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            [],
            DateTimeOffset.UtcNow));
    }
    
    public AuditableList(IEnumerable<T> collection)
    {
        AddHydrateMethods();
        switch (collection)
        {
            case null:
                throw new ArgumentNullException(nameof(collection));
            case ICollection<T> c:
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = SEmptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
                
                AddDomainEvent(new AuditableListInitialized<T>(
                    Ulid.NewUlid(),
                    _entityId,
                    _fieldId,
                    _fieldName,
                    ++_eventVersion,
                    _items,
                    DateTimeOffset.UtcNow));

                break;
            }
            default:
            {
                _size = 0;
                _items = SEmptyArray;
            
                foreach (var item in collection)
                {
                    Add(item);
                }
                
                AddDomainEvent(new AuditableListInitialized<T>(
                    Ulid.NewUlid(),
                    _entityId,
                    _fieldId,
                    _fieldName,
                    ++_eventVersion,
                    [],
                    DateTimeOffset.UtcNow));

                break;
            }
        }
    }
    
    public static implicit operator AuditableList<T>(List<T> l) => new (l);
    
    public int Count => _size;
    
    // Gets and sets the capacity of this list.  The capacity is the size of
    // the internal array used to hold items.  When set, the internal
    // array of the list is reallocated to the given capacity.
    //
    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _size)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Capacity cannot be less than the size of the list");
            }

            if (value != _items.Length)
            {
                if (value > 0)
                {
                    T[] newItems = new T[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, newItems, _size);
                    }
                    _items = newItems;
                }
                else
                {
                    _items = SEmptyArray;
                }
            }
        }
    }
    
    internal void SetAsReadOnly()
    {
        _isReadOnly = true;
    }
    
    public void ForEach(Action<T> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        int version = _version;

        for (int i = 0; i < _size; i++)
        {
            if (version != _version)
            {
                break;
            }
            action(_items[i]);
        }

        if (version != _version)
            throw new InvalidOperationException("The list was modified.");
    }

    public Enumerator GetEnumerator() => new (this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        Count == 0 ? SzGenericArrayEnumerator<T>.Empty :
            GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    
    public ReadOnlyCollection<T> AsReadOnly()
        => new (this);

    // Adds the given object to the end of this list. The size of the list is
    // increased by one. If required, the capacity of the list is doubled
    // before adding the new element.
    //
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        _version++;
        T[] array = _items;
        int size = _size;
        if ((uint)size < (uint)array.Length)
        {
            _size = size + 1;
            array[size] = item;
        }
        else
        {
            AddWithResize(item);
        }
        AddDomainEvent(new AuditableListItemAdded<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            item,
            DateTimeOffset.UtcNow));
    }
    
    public void AddRange(IEnumerable<T> collection)
    {
        switch (collection)
        {
            case null:
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
            case ICollection<T> c:
            {
                var count = c.Count;
                if (count > 0)
                {
                    if (_items.Length - _size < count)
                    {
                        Grow(checked(_size + count));
                    }

                    c.CopyTo(_items, _size);
                    _size += count;
                    _version++;
                }

                break;
            }
            default:
            {
                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }

                break;
            }
        }
    }

    int IList.Add(object? value)
    {
        if (value == null && default(T) != null)
        {
            throw new ArgumentNullException(nameof(value), "Value cannot be null");
        }

        try
        {
            Add((T)value!);
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Value is not of the correct type. Given {value?.GetType()}, AuditableList type: {typeof(T)}", nameof(value));
        }

        return Count - 1;
    }
    
    // Non-inline from List.Add to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        Debug.Assert(_size == _items.Length);
        int size = _size;
        Grow(size + 1);
        _size = size + 1;
        _items[size] = item;
    }

    void IList.Clear()
    {
        _version++;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            int size = _size;
            _size = 0;
            if (size > 0)
            {
                Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
            }
        }
        else
        {
            _size = 0;
        }
        AddDomainEvent(new AuditableListCleared<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            DateTimeOffset.UtcNow));
    }
    
    // Contains returns true if the specified element is in the List.
    // It does a linear, O(n) search.  Equality is determined by calling
    // EqualityComparer<T>.Default.Equals().
    //
    public bool Contains(T item)
    {
        // PERF: IndexOf calls Array.IndexOf, which internally
        // calls EqualityComparer<T>.Default.IndexOf, which
        // is specialized for different types. This
        // boosts performance since instead of making a
        // virtual method call each iteration of the loop,
        // via EqualityComparer<T>.Default.Equals, we
        // only make one virtual call to EqualityComparer.IndexOf.

        return _size != 0 && IndexOf(item) >= 0;
    }

    bool IList.Contains(object? value)
    {
        if (IsCompatibleObject(value))
        {
            return Contains((T)value!);
        }
        return false;
    }
    
    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards from beginning to end.
    // The elements of the list are compared to the given value using the
    // Object.Equals method.
    //
    // This method uses the Array.IndexOf method to perform the
    // search.
    //
    public int IndexOf(T item)
        => Array.IndexOf(_items, item, 0, _size);

    int IList.IndexOf(object? value)
    {
        if (IsCompatibleObject(value))
        {
            return IndexOf((T)value!);
        }
        return -1;
    }
    
    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards, starting at index
    // index and ending at count number of elements. The
    // elements of the list are compared to the given value using the
    // Object.Equals method.
    //
    // This method uses the Array.IndexOf method to perform the
    // search.
    //
    public int IndexOf(T item, int index)
    {
        if (index > _size)
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");
        return Array.IndexOf(_items, item, index, _size - index);
    }

    // Returns the index of the first occurrence of a given value in a range of
    // this list. The list is searched forwards, starting at index
    // index and upto count number of elements. The
    // elements of the list are compared to the given value using the
    // Object.Equals method.
    //
    // This method uses the Array.IndexOf method to perform the
    // search.
    //
    public int IndexOf(T item, int index, int count)
    {
        if (index > _size)
            throw new IndexOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");

        if (count < 0 || index > _size - count)
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");

        return Array.IndexOf(_items, item, index, count);
    }

    // Inserts an element into this list at a given index. The size of the list
    // is increased by one. If required, the capacity of the list is doubled
    // before inserting the new element.
    //
    public void Insert(int index, T item)
    {
        // Note that insertions at the end are legal.
        if ((uint)index > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if (_size == _items.Length) Grow(_size + 1);
        if (index < _size)
        {
            Array.Copy(_items, index, _items, index + 1, _size - index);
        }
        _items[index] = item;
        _size++;
        _version++;
        
        AddDomainEvent(new AuditableListItemInserted<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            item,
            index,
            DateTimeOffset.UtcNow));
    }
    
    void IList.Insert(int index, object? value)
    {
        if ((uint)index > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        try
        {
            Insert(index, (T)value!);
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Value is not of the correct type. Given {value?.GetType()}, AuditableList type: {typeof(T)}", nameof(value));
        }
    }
    
    // Inserts the elements of the given collection at a given index. If
    // required, the capacity of the list is increased to twice the previous
    // capacity or the new size, whichever is larger.  Ranges may be added
    // to the end of the list by setting index to the List's size.
    //
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
        }

        if ((uint)index > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count > 0)
            {
                if (_items.Length - _size < count)
                {
                    Grow(checked(_size + count));
                }
                if (index < _size)
                {
                    Array.Copy(_items, index, _items, index + count, _size - index);
                }

                // If we're inserting a List into itself, we want to be able to deal with that.
                if (Equals(this, c))
                {
                    // Copy first part of _items to insert location
                    Array.Copy(_items, 0, _items, index, index);
                    // Copy last part of _items back to inserted location
                    Array.Copy(_items, index + count, _items, index * 2, _size - index);
                }
                else
                {
                    c.CopyTo(_items, index);
                }
                _size += count;
                _version++;
                
                AddDomainEvent(new AuditableListRangeInserted<T>(
                    Ulid.NewUlid(),
                    _entityId,
                    _fieldId,
                    _fieldName,
                    ++_eventVersion,
                    c,
                    index,
                    DateTimeOffset.UtcNow));
            }
        }
        else
        {
            using IEnumerator<T> en = collection.GetEnumerator();
            while (en.MoveNext())
            {
                Insert(index++, en.Current);
            }
        }
    }
    
    // Removes the first occurrence of the given element, if found.
    // The size of the list is decreased by one if successful.
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    void IList.Remove(object? value)
    {
        if (IsCompatibleObject(value))
        {
            Remove((T)value!);
        }
    }
    
    // Removes a range of elements from this list.
    public void RemoveRange(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
        }

        if (_size - index < count)
            throw new AggregateException("Invalid offset length");

        if (count > 0)
        {
            _size -= count;
            if (index < _size)
            {
                Array.Copy(_items, index + count, _items, index, _size - index);
            }

            _version++;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_items, _size, count);
            }
            
            AddDomainEvent(new AuditableListRangeRemoved<T>(
                Ulid.NewUlid(),
                _entityId,
                _fieldId,
                _fieldName,
                ++_eventVersion,
                index,
                count,
                DateTimeOffset.UtcNow));
        }
    }

    
    // Removes the element at the given index. The size of the list is
    // decreased by one.
    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        _size--;
        if (index < _size)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _items[_size] = default!;
        }
        _version++;
        
        AddDomainEvent(new AuditableListRemoveAt<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            index,
            DateTimeOffset.UtcNow));
    }
    
    public List<T> Slice(int start, int length) => GetRange(start, length);
    
    public List<T> GetRange(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
        }

        if (_size - index < count)
        {
            throw new AggregateException("Invalid offset length");
        }

        var list = new T[count];
        Array.Copy(_items, index, list, 0, count);
        return [..list];
    }

    public bool IsFixedSize => false;

    bool IList.IsReadOnly => _isReadOnly;

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    public void Clear()
    {
        _version++;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            int size = _size;
            _size = 0;
            if (size > 0)
            {
                Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
            }
        }
        else
        {
            _size = 0;
        }
        
        AddDomainEvent(new AuditableListCleared<T>(
            Ulid.NewUlid(),
            _entityId,
            _fieldId,
            _fieldName,
            ++_eventVersion,
            DateTimeOffset.UtcNow));
    }

    // Copies this List into array, which must be of a
    // compatible array type.
    public void CopyTo(T[] array)
        => CopyTo(array, 0);

    // Copies this List into array, which must be of a
    // compatible array type.
    void ICollection.CopyTo(Array array, int arrayIndex)
    {
        if (array != null && array.Rank != 1)
        {
            throw new AggregateException("Only single dimensional arrays are supported for the requested action.");
        }

        try
        {
            // Array.Copy will check for NULL.
            Array.Copy(_items, 0, array!, arrayIndex, _size);
        }
        catch (ArrayTypeMismatchException)
        {
            throw new AggregateException("Invalid array type");
        }
    }

    // Copies a section of this list to the given array at the given index.
    //
    // The method uses the Array.Copy method to copy the elements.
    //
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        if (_size - index < count)
        {
            throw new AggregateException("Invalid offset length");
        }

        // Delegate rest of error checking to Array.Copy.
        Array.Copy(_items, index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        // Delegate rest of error checking to Array.Copy.
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    int ICollection.Count => _size;

    int ICollection<T>.Count => _size;

    bool ICollection<T>.IsReadOnly => _isReadOnly;

    public T this[int index]
    {
        get
        {
            // Following trick can reduce the range check by one
            if ((uint)index >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
            return _items[index];
        }

        set
        {
            if ((uint)index >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
            _items[index] = value;
            _version++;
        }
    }
    
    public int EnsureCapacity(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative");
        }
        if (_items.Length < capacity)
        {
            Grow(capacity);
        }

        return _items.Length;
    }

    int IReadOnlyCollection<T>.Count => _size;
    
    public bool Exists(Predicate<T> match)
        => FindIndex(match) != -1;

    public T? Find(Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        for (int i = 0; i < _size; i++)
        {
            if (match(_items[i]))
            {
                return _items[i];
            }
        }
        return default;
    }

    public List<T> FindAll(Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        List<T> list = new List<T>();
        for (int i = 0; i < _size; i++)
        {
            if (match(_items[i]))
            {
                list.Add(_items[i]);
            }
        }
        return list;
    }
    
     public int FindIndex(Predicate<T> match)
            => FindIndex(0, _size, match);

    public int FindIndex(int startIndex, Predicate<T> match)
        => FindIndex(startIndex, _size - startIndex, match);

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        if ((uint)startIndex > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        if (count < 0 || startIndex > _size - count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
        }

        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        int endIndex = startIndex + count;
        for (int i = startIndex; i < endIndex; i++)
        {
            if (match(_items[i])) return i;
        }
        return -1;
    }

    public T? FindLast(Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        for (int i = _size - 1; i >= 0; i--)
        {
            if (match(_items[i]))
            {
                return _items[i];
            }
        }
        return default;
    }

    public int FindLastIndex(Predicate<T> match)
        => FindLastIndex(_size - 1, _size, match);

    public int FindLastIndex(int startIndex, Predicate<T> match)
        => FindLastIndex(startIndex, startIndex + 1, match);

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        if (_size == 0)
        {
            // Special case for 0 length List
            if (startIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
        }
        else
        {
            // Make sure we're not out of range
            if ((uint)startIndex >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
        }

        // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
        if (count < 0 || startIndex - count + 1 < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");
        }

        int endIndex = startIndex - count;
        for (int i = startIndex; i > endIndex; i--)
        {
            if (match(_items[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private void Grow(int capacity)
    {
        Debug.Assert(_items.Length < capacity);

        var newCapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

        // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
        // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
        if ((uint)newCapacity > Array.MaxLength) newCapacity = Array.MaxLength;

        // If the computed capacity is still less than specified, set to the original argument.
        // Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newCapacity < capacity) newCapacity = capacity;

        Capacity = newCapacity;
    }
    
    private static bool IsCompatibleObject(object? value)
    {
        // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        return (value is T) || (value == null && default(T) == null);
    }
    
    public struct Enumerator : IEnumerator<T>
    {
        private readonly AuditableList<T> _list;
        private int _index;
        private readonly int _version;
        private T? _current;

        internal Enumerator(AuditableList<T> list)
        {
            _list = list;
            _index = 0;
            _version = list._version;
            _current = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            AuditableList<T> localList = _list;

            if (_version == localList._version && ((uint)_index < (uint)localList._size))
            {
                _current = localList._items[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("The list was modified.");
            }

            _index = _list._size + 1;
            _current = default;
            return false;
        }

        public T Current => _current!;

        object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _list._size + 1)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }
                return Current;
            }
        }

        void IEnumerator.Reset()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("The list was modified.");
            }

            _index = 0;
            _current = default;
        }
    }
    
    internal abstract class SzGenericArrayEnumeratorBase : IDisposable
    {
        protected int Index;
        protected readonly int EndIndex;

        protected SzGenericArrayEnumeratorBase(int endIndex)
        {
            Index = -1;
            EndIndex = endIndex;
        }

        public bool MoveNext()
        {
            int index = Index + 1;
            if ((uint)index < (uint)EndIndex)
            {
                Index = index;
                return true;
            }
            Index = EndIndex;
            return false;
        }

        public void Reset() => Index = -1;

        public void Dispose()
        {
        }
    }
    
    internal sealed class SzGenericArrayEnumerator<TEnumerator> : SzGenericArrayEnumeratorBase, IEnumerator<TEnumerator>
    {
        private readonly TEnumerator[]? _array;

        /// <summary>Provides an empty enumerator singleton.</summary>
        /// <remarks>
        /// If the consumer is using SZGenericArrayEnumerator elsewhere or is otherwise likely
        /// to be using T[] elsewhere, this singleton should be used.  Otherwise, GenericEmptyEnumerator's
        /// singleton should be used instead, as it doesn't reference T[] in order to reduce footprint.
        /// </remarks>
        internal static readonly SzGenericArrayEnumerator<TEnumerator> Empty = new (null, 0);

        internal SzGenericArrayEnumerator(TEnumerator[]? array, int endIndex)
            : base(endIndex)
        {
            Debug.Assert(array == null || endIndex == array.Length);
            _array = array;
        }

        public TEnumerator Current
        {
            get
            {
                if ((uint)Index >= (uint)EndIndex)
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                return _array![Index];
            }
        }

        object? IEnumerator.Current => Current;
    }
}