//  
//  EntityCacheBase.cs
//  
//  Author:
//       Craig Fowler <craig@craigfowler.me.uk>
// 
//  Copyright (c) 2012 CSF Software Limited
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using CSF.Patterns.DDD.Entities;

namespace CSF.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>Base class for an <see cref="IEntityCache"/>.</para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// The tasks that should be performed for implementing classes are:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// To decide whether or not to override the <see cref="IsCacheHit"/> method.  This is the core method that decides
  /// whether or not a request for any given entity should be served from cache or not.
  /// </item>
  /// <item>
  /// To implement whatever mechanism is required in order to call either the <see cref="PerformCleanup()"/> method or
  /// the <see cref="LockedCleanup()"/> method.  This base class does not call either under any circumstance.  Note
  /// that the locked version of the cleanup method is intended ONLY for calling within an established write lock.
  /// The non-locked version establishes this lock first and will throw an exception if the lock is already held.
  /// </item>
  /// <item>
  /// To perhaps override <see cref="GetCleanupRemovalCount()"/> and/or <see cref="GetCleanupRemovalCount()"/> with
  /// any additional logic as appropriate.  The implementations for these methods provided in this base type is a very
  /// simple demonstration functionality.
  /// </item>
  /// </list>
  /// </remarks>
  public abstract class EntityCacheBase : IEntityCache, IDisposable
  {
    #region constants
    
    /// <summary>
    /// <para>Read-only constant.  The proportion of entries that should remain after a cleanup operation.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Specifically, when a cleanup operation is initiated, the "best" <c>CLEANUP_FACTOR</c> entities are preserved
    /// in the cache.  Any others are removed.
    /// </para>
    /// </remarks>
    private const double CLEANUP_FACTOR = 0.8d;
    
    #endregion
    
    #region fields
    
    /// <summary>
    /// <para>A <see cref="ReaderWriterLockSlim"/> for controlling synchronised access to this instance.</para>
    /// </summary>
    protected ReaderWriterLockSlim SyncRoot;
    
    private ICacheBackingStore _backingStore;
    private IReplacementPolicy _replacementPolicy;
    private IItemPolicy _defaultItemPolicy;
    private bool _disposed;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the replacement policy (AKA caching algorithm) for this cache.</para>
    /// </summary>
    public IReplacementPolicy ReplacementPolicy
    {
      get {
        return _replacementPolicy;
      }
      protected set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        this.SyncRoot.EnterWriteLock();
        _replacementPolicy = value;
        this.SyncRoot.ExitWriteLock();
      }
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets the default item policy for newly-added items.  No policy is added if this property is null.
    /// </para>
    /// </summary>
    public IItemPolicy DefaultItemPolicy
    {
      get {
        return _defaultItemPolicy;
      }
      set {
        this.SyncRoot.EnterWriteLock();
        _defaultItemPolicy = value;
        this.SyncRoot.ExitWriteLock();
      }
    }

    /// <summary>
    /// <para>Gets and sets the backing store for this cache instance.</para>
    /// </summary>
    protected ICacheBackingStore BackingStore
    {
      get {
        return _backingStore;
      }
      set {
        this.SyncRoot.EnterWriteLock();
        _backingStore = value;
        this.SyncRoot.ExitWriteLock();
      }
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets a collection of all of the <see cref="IItemPolicy"/> stored within the current instance.
    /// </para>
    /// </summary>
    protected Dictionary<IIdentity, IItemPolicy> ItemPolicies
    {
      get;
      private set;
    }
    
    #endregion

    #region IEntityCache implementation

    /// <summary>
    /// <para>Read-only.  Gets the count of entities stored within this cache.</para>
    /// </summary>
    public int Count
    {
      get {
        int output;
        
        this.SyncRoot.EnterReadLock();
        output = (this.BackingStore != null)? this.BackingStore.Count : 0;
        this.SyncRoot.ExitReadLock();
        
        return output;
      }
    }

    /// <summary>
    /// <para>Overloaded.  Explicitly adds a single entity to the cache.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Add (IEntity entity)
    {
      this.Add(entity, this.DefaultItemPolicy);
    }

    /// <summary>
    /// <para>
    /// Overloaded.  Explicitly adds a single entity to the cache, along with an explicit policy, providing additional
    /// control over its lifetime in the cache.
    /// </para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    /// <param name="itemPolicy">
    /// A <see cref="IItemPolicy"/>
    /// </param>
    public void Add (IEntity entity, IItemPolicy itemPolicy)
    {
      if(entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      
      try
      {
        this.SyncRoot.EnterWriteLock();
        this.LockedAdd(entity, itemPolicy);
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
      }
    }

    /// <summary>
    /// <para>
    /// Gets whether or not an entitity (with the given <paramref name="identity"/>) is contained within this cache.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The usefulness of this method may be limited due to concurrency issues.  Many cache instances are used by
    /// multiple threads (perhaps processes) concurrently and thus this method is only true for the moment in time when
    /// it is executed.  If an entity is detected in the cache it could still be removed a fraction of a second
    /// afterwards by another thread (or by a scheduled purging operation).
    /// </para>
    /// </remarks>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    public bool Contains (IIdentity identity)
    {
      bool output;
      
      if(identity == null)
      {
        throw new ArgumentNullException("identity");
      }
      
      try
      {
        this.SyncRoot.EnterReadLock();
        output = (this.BackingStore != null)? this.BackingStore.Contains(identity) : false;
      }
      finally
      {
        if(this.SyncRoot.IsReadLockHeld)
        {
          this.SyncRoot.ExitReadLock();
        }
      }
      
      return output;
    }

    /// <summary>
    /// <para>
    /// Overloaded.  Attempts to read an entity from the cache.  If it is not found then the <paramref name="reader"/>
    /// is used to retrieve a fresh copy of the entity.
    /// </para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <param name="reader">
    /// A <see cref="EntityReader"/>
    /// </param>
    /// <returns>
    /// A <see cref="IEntity"/>
    /// </returns>
    public IEntity Read (IIdentity identity, EntityReader reader)
    {
      return this.Read(identity, this.DefaultItemPolicy, reader);
    }

    /// <summary>
    /// <para>
    /// Overloaded.  Attempts to read an entity from the cache.  If it is not found then the <paramref name="reader"/>
    /// is used to retrieve a fresh copy of the entity.  If  a fresh copy is retrieved, the given
    /// <paramref name="itemPolicy"/> is used to provide additional control over the entity's lifetime in the cache.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the core method that attempts to read entities from this cache instance and then either returns them
    /// from the <see cref="BackingStore"/> or reads them in afresh using the <paramref name="reader"/>.  When entities
    /// are retrieved using the reader they are implicitly added to the cache as part of the action.  This method can
    /// result in two core outcomes:
    /// </para>
    /// <list type="table">
    /// <item>
    /// <term>Cache hit</term>
    /// <description>
    /// The desired entity was found within the cache and is suitable to be returned directly from the cache.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Cache miss</term>
    /// <description>
    /// The desired entity was either not found within the cache or a policy deemed that is must not be returned from
    /// the cache.  The entity is fetched using the <paramref name="reader"/>, added to the cache and then returned.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <param name="itemPolicy">
    /// A <see cref="IItemPolicy"/>
    /// </param>
    /// <param name="reader">
    /// A <see cref="EntityReader"/>
    /// </param>
    /// <returns>
    /// A <see cref="IEntity"/>
    /// </returns>
    public IEntity Read (IIdentity identity, IItemPolicy itemPolicy, EntityReader reader)
    {
      IEntity output;
      
      if(identity == null)
      {
        throw new ArgumentNullException("identity");
      }
      else if(reader == null)
      {
        throw new ArgumentNullException("reader");
      }
      
      try
      {
        this.SyncRoot.EnterUpgradeableReadLock();
        
        if(this.IsCacheHit(identity))
        {
          // This is a cache hit, downgrade our lock immediately and return the entity from the cache
          this.SyncRoot.EnterReadLock();
          this.SyncRoot.ExitUpgradeableReadLock();
          
          this.OnCacheHit(identity);
          output = this.BackingStore.Read(identity);
        }
        else
        {
          // This is a cache miss, upgrade our lock and go get the entity from the reader
          this.SyncRoot.EnterWriteLock();
          
          this.OnCacheMiss(identity);
          output = reader(identity);
          this.LockedAdd(output, itemPolicy);
        }
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
        if(this.SyncRoot.IsUpgradeableReadLockHeld)
        {
          this.SyncRoot.ExitUpgradeableReadLock();
        }
        if(this.SyncRoot.IsReadLockHeld)
        {
          this.SyncRoot.ExitReadLock();
        }
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Reads all of the entities currently contained within the current cache instance.</para>
    /// </summary>
    /// <returns>
    /// A collection of <see cref="IEntity"/>, indexed by their <see cref="IIdentity"/>.
    /// </returns>
    public IDictionary<IIdentity, IEntity> ReadCache()
    {
      IDictionary<IIdentity, IEntity> output;
      
      try
      {
        this.SyncRoot.EnterReadLock();
        output = (this.BackingStore != null)? this.BackingStore.ReadAll() : new Dictionary<IIdentity, IEntity>();
      }
      finally
      {
        if(this.SyncRoot.IsReadLockHeld)
        {
          this.SyncRoot.ExitReadLock();
        }
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Overloaded.  Removes the entity with the given <paramref name="identity"/> from the cache.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    public void Remove (IIdentity identity)
    {
      this.Remove(new IIdentity[] { identity });
    }

    /// <summary>
    /// <para>
    /// Overloaded.  Removes all entities with with <paramref name="identities"/> in the given collection from the
    /// cache.
    /// </para>
    /// </summary>
    /// <param name="identities">
    /// A collection of <see cref="IIdentity"/>
    /// </param>
    public void Remove (IEnumerable<IIdentity> identities)
    {
      if(identities == null)
      {
        throw new ArgumentNullException("identities");
      }
      
      try
      {
        this.SyncRoot.EnterWriteLock();
        this.LockedRemove(identities);
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
      }
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Convenience method to remove entities matching the given <paramref name="identities"/> from the
    /// cache.
    /// </para>
    /// </summary>
    /// <param name="identities">
    /// A <see cref="IIdentity[]"/>
    /// </param>
    public void Remove(params IIdentity[] identities)
    {
      this.Remove(identities);
    }
    
    /// <summary>
    /// <para>Removes all entities from this cache, emptying it.</para>
    /// </summary>
    public void Purge()
    {
      try
      {
        IList<IIdentity> entitiesToRemove;
        
        this.SyncRoot.EnterWriteLock();
        entitiesToRemove = (this.BackingStore != null)? this.BackingStore.ReadAllIdentities() : new List<IIdentity>();
        this.LockedRemove(entitiesToRemove);
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
      }
    }
    
    /// <summary>
    /// <para>Performs a cleanup operation on this cache instance, removing stale entries from the cache.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method establishes a write lock on the cache and then performs the cleanup operation.  If you wish to
    /// perform cleanup but already have an established write lock then instead use <see cref="LockedCleanup()"/>.
    /// </para>
    /// <para>
    /// When overriding this method, call <see cref="LockedCleanup()"/> to perform the actual cleanup task itself.
    /// The only logic to add to this method should be to handle the establishing of the write lock and (if
    /// appropriate) logic relating to launching the cleanup in a separate thread, or performing a regular scheduled
    /// cleanup task.
    /// </para>
    /// </remarks>
    public virtual void PerformCleanup()
    {
      try
      {
        this.SyncRoot.EnterWriteLock();
        this.LockedCleanup();
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
      }
    }
    
    #endregion
    
    #region private and protected methods
    
    /// <summary>
    /// <para>Determines whether the given <paramref name="identity"/> is a cache hit or a miss.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method defines a cache hit as meeting all of the following criteria:
    /// </para>
    /// <list type="bullet">
    /// <item>The <see cref="BackingStore"/> is not null and also contains the requested entity.</item>
    /// <item>
    /// The <see cref="ItemPolicies"/> collection either does not contain an entry for the requested entity, or the
    /// entry does not indicate that the item should be removed on this cache hit.  IE:  The item policy does not
    /// explicitly override this cache hit.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    protected virtual bool IsCacheHit(IIdentity identity)
    {
      bool output;
      
      if(identity == null)
      {
        throw new ArgumentNullException("identity");
      }
      else if(!this.SyncRoot.IsReadLockHeld && !this.SyncRoot.IsUpgradeableReadLockHeld)
      {
        throw new InvalidOperationException("Cannot test for a cache hit unless an appropriate lock is held.");
      }
      
      if(this.BackingStore == null)
      {
        output = false;
      }
      else if(!this.BackingStore.Contains(identity))
      {
        output = false;
      }
      else if(this.ItemPolicies.ContainsKey(identity)
              && this.ItemPolicies[identity].GetAction(ItemLifecycleMilestones.OnCacheHit) == ItemAction.Remove)
      {
        output = false;
      }
      else
      {
        output = true;
      }

      return output;
    }
    
    /// <summary>
    /// <para>
    /// Gets a count of how many entities to remove from this cache instance during a given cleanup operation.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When overriding this method, be sure to check for the write lock and throw an exception if it is not held.
    /// </para>
    /// </remarks>
    /// <returns>
    /// A <see cref="System.Int32"/>
    /// </returns>
    protected virtual int GetCleanupRemovalCount()
    {
      if(!this.SyncRoot.IsWriteLockHeld)
      {
        throw new InvalidOperationException("Cannot get a removal count unless an appropriate lock is held.");
      }
      
      return (int) Math.Floor((double) this.Count * CLEANUP_FACTOR);
    }
    
    /// <summary>
    /// <para>Performs the cleanup process from within an established/held write lock.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The cleanup process proceeds as such:
    /// </para>
    /// <list type="number">
    /// <item>
    /// The <see cref="ItemPolicies"/> (where present) for all cached entities are checked to determine if any entities
    /// should be immediately removed from the cache regardless of the <see cref="ReplacementPolicy"/>.
    /// </item>
    /// <item>
    /// <see cref="GetCleanupRemovalCount"/> is used to determine how many entities should be removed by the
    /// replacement policy.
    /// </item>
    /// <item>
    /// The <see cref="ReplacementPolicy"/> is consulted to generate a list of the <see cref="IIdentity"/> of the
    /// entities that should be removed.
    /// </item>
    /// <item>
    /// For each of the entities to be removed, their <see cref="IItemPolicy"/> (if present) is checked.  If the policy
    /// does not indicate that the removal should be overridden then the entity is removed from the cache.  If the
    /// item policy overrides the removal then the entity remains within the cache and this step continues to the next
    /// entity.
    /// </item>
    /// </list>
    /// </remarks>
    protected void LockedCleanup()
    {
      if(!this.SyncRoot.IsWriteLockHeld)
      {
        throw new InvalidOperationException("Cannot perform cache cleanup unless an appropriate lock is held.");
      }
      
      foreach(IIdentity identity in this.ItemPolicies.Keys)
      {
        if(this.ItemPolicies[identity].GetAction(ItemLifecycleMilestones.OnCacheCleanup) == ItemAction.Remove)
        {
          this.LockedRemove(identity);
        }
      }
      
      this.LockedCleanup(this.GetCleanupRemovalCount());
    }
    
    /// <summary>
    /// <para>Performs removal of entities using the <see cref="ReplacementPolicy"/>.</para>
    /// <seealso cref="LockedCleanup()"/>
    /// </summary>
    /// <param name="howManyToRemove">
    /// A <see cref="System.Int32"/>
    /// </param>
    private void LockedCleanup(int howManyToRemove)
    {
      if(this.ReplacementPolicy != null)
      {
        IList<IIdentity> entitiesToRemove = this.ReplacementPolicy.GetEntitiesToRemove(howManyToRemove);
        
        foreach(IIdentity identity in entitiesToRemove)
        {
          if(this.ItemPolicies.ContainsKey(identity)
             && this.ItemPolicies[identity].GetAction(ItemLifecycleMilestones.OnReplacement) != ItemAction.Preserve)
          {
            this.LockedRemove(identity);
          }
        }
      }
    }
    
    /// <summary>
    /// <para>
    /// Performs the actual operations involved in adding an <paramref name="entity"/> to this instance.  This method
    /// must be called within a write lock.
    /// </para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    /// <param name="itemPolicy">
    /// A <see cref="IItemPolicy"/>
    /// </param>
    private void LockedAdd(IEntity entity, IItemPolicy itemPolicy)
    {
      IIdentity identity = entity.GetIdentity();
      
      if(!this.SyncRoot.IsWriteLockHeld)
      {
        throw new InvalidOperationException("Cannot add an entity to the current instance unless an appropriate " +
                                            "lock is held.");
      }
      else if(this.BackingStore == null)
      {
          throw new InvalidOperationException("The cache backing store must not be null.");
      }
      
      this.BackingStore.Add(entity);
      if(itemPolicy != null)
      {
        this.ItemPolicies.Add(identity, itemPolicy);
      }
      this.OnItemAdded(identity);
    }

    /// <summary>
    /// <para>
    /// Performs the actual operations involved in removing an entity from this instance.  This method
    /// must be called within a write lock.
    /// </para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    private void LockedRemove(IIdentity identity)
    {
      if(!this.SyncRoot.IsWriteLockHeld)
      {
        throw new InvalidOperationException("Cannot remove an entity from the current instance unless an appropriate " +
                                            "lock is held.");
      }
      else if(this.BackingStore == null)
      {
        throw new InvalidOperationException("The cache backing store must not be null.");
      }
      else if(identity == null)
      {
        throw new ArgumentException("Encountered null identity whilst removing entities", "identities");
      }
      
      this.BackingStore.Remove(identity);
      this.ItemPolicies.Remove(identity);
      this.OnItemRemoved(identity);
    }
    
    /// <summary>
    /// <para>
    /// Performs the actual operations involved in removing one or more entities from this instance.  This method
    /// must be called within a write lock.
    /// </para>
    /// </summary>
    /// <param name="identities">
    /// A collection of <see cref="IIdentity"/>
    /// </param>
    private void LockedRemove(IEnumerable<IIdentity> identities)
    {
      foreach(IIdentity identity in identities)
      {
        this.LockedRemove(identity);
      }
    }
    
    #endregion
    
    #region events and invokers
    
    /// <summary>
    /// <para>Occurs whenever an item is successfully read from the cache.</para>
    /// </summary>
    public event EventHandler CacheHit;
    
    /// <summary>
    /// <para>Occurs whenever a failed attempt is made to read an item from the cache.</para>
    /// </summary>
    public event EventHandler CacheMiss;
    
    /// <summary>
    /// <para>Occurs whenever an item is added to the cache.</para>
    /// </summary>
    public event EventHandler ItemAdded;
    
    /// <summary>
    /// <para>Occurs whenever an item is removed from the cache.</para>
    /// </summary>
    public event EventHandler ItemRemoved;
    
    /// <summary>
    /// <para>Invoker for the <see cref="CacheHit"/> event.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    protected virtual void OnCacheHit(IIdentity identity)
    {
      CachingEventArgs args = new CachingEventArgs(CachingEventType.CacheHit);
      args.EntityIdentity = identity;
      
      if(this.CacheHit != null)
      {
        this.CacheHit(this, args);
      }
    }
    
    /// <summary>
    /// <para>Invoker for the <see cref="CacheMiss"/> event.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    protected virtual void OnCacheMiss(IIdentity identity)
    {
      CachingEventArgs args = new CachingEventArgs(CachingEventType.CacheMiss);
      args.EntityIdentity = identity;
      
      if(this.CacheMiss != null)
      {
        this.CacheMiss(this, args);
      }
    }
    
    /// <summary>
    /// <para>Invoker for the <see cref="ItemAdded"/> event.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    protected virtual void OnItemAdded(IIdentity identity)
    {
      CachingEventArgs args = new CachingEventArgs(CachingEventType.ItemAdded);
      args.EntityIdentity = identity;
      
      if(this.ItemAdded != null)
      {
        this.ItemAdded(this, args);
      }
    }
    
    /// <summary>
    /// <para>Invoker for the <see cref="ItemRemoved"/> event.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    protected virtual void OnItemRemoved(IIdentity identity)
    {
      CachingEventArgs args = new CachingEventArgs(CachingEventType.ItemRemoved);
      args.EntityIdentity = identity;
      
      if(this.ItemRemoved != null)
      {
        this.ItemRemoved(this, args);
      }
    }
    
    #endregion
    
    #region IDisposable implementation
    
    /// <summary>
    /// <para>Performs explicit disposal of this instance.</para>
    /// </summary>
    public void Dispose ()
    {
      if(!_disposed)
      {
        this.PerformDisposal();
        _disposed = true;
      }
    }
    
    /// <summary>
    /// <para>Performs actual disposal-related tasks on this object instance.</para>
    /// </summary>
    protected virtual void PerformDisposal()
    {
      if(_disposed)
      {
        throw new ObjectDisposedException(this.GetType().FullName);
      }
      
      this.SyncRoot.Dispose();
      
      foreach(EventHandler handler in this.ItemAdded.GetInvocationList())
      {
        this.ItemAdded -= handler;
      }
      foreach(EventHandler handler in this.ItemRemoved.GetInvocationList())
      {
        this.ItemAdded -= handler;
      }
      foreach(EventHandler handler in this.CacheHit.GetInvocationList())
      {
        this.ItemAdded -= handler;
      }
      foreach(EventHandler handler in this.CacheMiss.GetInvocationList())
      {
        this.ItemAdded -= handler;
      }
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>Initialises this instance with empty components.</para>
    /// </summary>
    /// <param name="replacementPolicy">
    /// An <see cref="IReplacementPolicy"/>
    /// </param>
    public EntityCacheBase(IReplacementPolicy replacementPolicy) : this(replacementPolicy, null, null) {}
    
    /// <summary>
    /// <para>Initialises this instance with the given components.</para>
    /// </summary>
    /// <param name="replacementPolicy">
    /// An <see cref="IReplacementPolicy"/>
    /// </param>
    /// <param name="backingStore">
    /// An <see cref="ICacheBackingStore"/>
    /// </param>
    /// <param name="defaultItemPolicy">
    /// An <see cref="IItemPolicy"/>
    /// </param>
    public EntityCacheBase (IReplacementPolicy replacementPolicy,
                            ICacheBackingStore backingStore,
                            IItemPolicy defaultItemPolicy)
    {
      _disposed = false;
      
      this.SyncRoot = new ReaderWriterLockSlim();
      this.ItemPolicies = new Dictionary<IIdentity, IItemPolicy>();
      
      this.ReplacementPolicy = replacementPolicy;
      this.BackingStore = backingStore;
      this.DefaultItemPolicy = defaultItemPolicy;
      
      this.CacheHit += this.ReplacementPolicy.HandleCacheEvent;
      this.CacheMiss += this.ReplacementPolicy.HandleCacheEvent;
      this.ItemAdded += this.ReplacementPolicy.HandleCacheEvent;
      this.ItemRemoved += this.ReplacementPolicy.HandleCacheEvent;
    }
    
    /// <summary>
    /// <para>Performs implicit disposal of this instance.</para>
    /// </summary>
    ~EntityCacheBase()
    {
      this.Dispose();
    }
    
    #endregion
  }
}

