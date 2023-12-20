# CacheProject
## Aim and Requirements
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

# Constraints notes
