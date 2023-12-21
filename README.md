# Cache Project
## Aim and requirements
The aim of this repo is to create a cache that:
- Uses Least Recently Used eviction policy.
- Stores arbitrary type of objects. (Which should be added and retrieved using a unique key)
    I have made the assumption that it is up to the user to ensure the key is unique, which could be done using a        hash function.
    I have also made the assumption that this unique key should also be of an arbitrary type, though I would
      expect the types used to be largely either strings or integers.
    I have further made the assumption that having arbitrary types means that in the same cache, the user should
      be able to add an item with (key, value) types of (string, double), then an item of (int, bool).
- Has configurable threshold for macimum number of items it can hold at any one time.
    I have implemented this as a settable value at anytime after the creation of the cache with a default
      capacity of 100 items.
- Can be used as a singleton.
- Is thread-safe.
- Allows the customer to know when items get evicted.

## Constraints notes
- I have used C# and .NET version 6.0
- I have used VisualStudio

## Timing
First 1-2 hours: I spent 1-2 hours deciding which data structures to use, creating a plan and building out an initial cache and writing some unittests without consideringas the singleton pattern, thread safety or a notification mechanism.

Next hour: I spent the next hour or so turning the cache into a singleton, ensuring thread-safety and adding some unit and stress tests fo rconcurrency.

Next hour: I spent the next hour or so creating more unittests and some stress tests which led to the adding of error messages and different functionality.

Next hour: I spent the next hour or so creating the consumer notification functionality, adding more unittests and testing the implementation manually.

I finnished by writing up this document you are reading and cleaning up the documentation a little.

## LRU data structure and algorithm choice
In building the cache there were several options of what data structures to use.
The decision as to which was primarily decided by the time and memory complexity of common methods and storage that would be needed.

### Functionality and storage needed:
- Get a value from the collection given a unique key.
- Insert new key, value pairs (position of insertion unimportant for requirements but order of most-least recently used must be conserved, or we have to store an additional data point with each key, value pair).
- Update key, value pair (either moving it if order is conserved, or updating a data point if recency is measured with additional data point).
- Extract the least recently used item.

### Possible data structures
There are many data structures to choose from. For this problem, search, retrieval, insertion and removal are all neccessary.

- Dictionary or Hash Map with Doubly Linked List (using dictionary / hash map to search and linked list to maintain order).
- Doubly Linked List and Array (this time with the array in place of a hash map for storing pointers to nodes).

The above implementations would provide O(1) time complexity for the regularly needed operations and O(n) space complexity.
All implementations work a similar way, all using 2 data structures increasing the code complexity a little.
All provide one data structure for ordering and fast insertion and extaction at the ends of their structure and another for holding the pointer to a node in an easily indexed location that can be found with a key.

Using an Array as a key holding structure with a linked list does not work well for non-integer keys, resizing the capacity of the array, or for sparse caches as empty spaces still take up memory. (the alternative would be inserting into the array for every new node rather than keeping a fixed array but this would drastically slow the speed of insertion at the front because all items in the array would need to be shifted, the same would be true for many items when removing from the middle).

Using a hash table as a key holding structure is easier to resize but still performs badly in terms of memory for sparce hash tables that don't dynamicly resize themselves.

Dictionaries are effectively dynamically resizing hash tables and perform efficiently.

We can then pair the Dictionary with either a Singly or Doubly linked list. A singly linked list would take up less memory, however, when updating a node in the middle of the list to move it to the front of the list, we'd need to loop through the whole linked list to set the next node of the previous node and so the operation would take O(n) time. Sacrificing the extra memory, a linked list performs this operation in O(1) time.
