//  
//  IEntityCache.cs
//  
//  Author:
//       Craig Fowler <craig@craigfowler.me.uk>
// 
//  Copyright (c) 2011 Craig Fowler
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
using CraigFowler.Patterns.DDD.Entities;

namespace CraigFowler.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>
  /// Base interface for a cache designed for <see cref="IEntity"/> instances, accessed using the
  /// <see cref="IIdentity"/> associated with each.
  /// </para>
  /// </summary>
  public interface IEntityCache
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the count of entities stored within this cache.</para>
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// <para>Read-only.  Gets the replacement policy (AKA caching algorithm) for this cache.</para>
    /// </summary>
    IReplacementPolicy ReplacementPolicy { get; }
    
    /// <summary>
    /// <para>
    /// Gets and sets the default item policy for newly-added items.  No policy is added if this property is null.
    /// </para>
    /// </summary>
    IItemPolicy DefaultItemPolicy { get; set; }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Overloaded.  Explicitly adds a single entity to the cache.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    void Add(IEntity entity);
    
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
    void Add(IEntity entity, IItemPolicy itemPolicy);
    
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
    bool Contains(IIdentity identity);
    
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
    IEntity Read(IIdentity identity, EntityReader reader);
    
    /// <summary>
    /// <para>
    /// Overloaded.  Attempts to read an entity from the cache.  If it is not found then the <paramref name="reader"/>
    /// is used to retrieve a fresh copy of the entity.  If  a fresh copy is retrieved, the given
    /// <paramref name="itemPolicy"/> is used to provide additional control over the entity's lifetime in the cache.
    /// </para>
    /// </summary>
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
    IEntity Read(IIdentity identity, IItemPolicy itemPolicy, EntityReader reader);
    
    /// <summary>
    /// <para>Reads all of the entities currently contained within the current cache instance.</para>
    /// </summary>
    /// <returns>
    /// A collection of <see cref="IEntity"/>, indexed by their <see cref="IIdentity"/>.
    /// </returns>
    IDictionary<IIdentity, IEntity> ReadCache();
    
    /// <summary>
    /// <para>Overloaded.  Removes the entity with the given <paramref name="identity"/> from the cache.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    void Remove(IIdentity identity);
    
    /// <summary>
    /// <para>
    /// Overloaded.  Removes all entities with with <paramref name="identities"/> in the given collection from the
    /// cache.
    /// </para>
    /// </summary>
    /// <param name="identities">
    /// A collection of <see cref="IIdentity"/>
    /// </param>
    void Remove(IEnumerable<IIdentity> identities);

    /// <summary>
    /// <para>Removes all entities from this cache, emptying it.</para>
    /// </summary>
    void Purge();
    
    /// <summary>
    /// <para>Performs a cleanup operation on this cache instance, removing stale entries from the cache.</para>
    /// </summary>
    void PerformCleanup();
    
    #endregion
    
    #region events
    
    /// <summary>
    /// <para>Occurs whenever an item is successfully read from the cache.</para>
    /// </summary>
    event EventHandler CacheHit;
    
    /// <summary>
    /// <para>Occurs whenever a failed attempt is made to read an item from the cache.</para>
    /// </summary>
    event EventHandler CacheMiss;
    
    /// <summary>
    /// <para>Occurs whenever an item is added to the cache.</para>
    /// </summary>
    event EventHandler ItemAdded;
    
    /// <summary>
    /// <para>Occurs whenever an item is removed from the cache.</para>
    /// </summary>
    event EventHandler ItemRemoved;
    
    #endregion
  }
}

