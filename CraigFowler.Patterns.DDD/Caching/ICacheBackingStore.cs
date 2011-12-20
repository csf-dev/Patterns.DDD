//  
//  ICacheBackingStore.cs
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

using System.Collections.Generic;
using CraigFowler.Patterns.DDD.Entities;

namespace CraigFowler.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>Interface for a data-store that may be used by an <see cref="IEntityCache"/>.</para>
  /// </summary>
  public interface ICacheBackingStore
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the count of entities in this backing store.</para>
    /// </summary>
    int Count { get; }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Adds an entity to the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    void Add(IEntity entity);
    
    /// <summary>
    /// <para>
    /// Gets whether or not the current instance contains a cached entity matching the given
    /// <paramref name="identity"/>.
    /// </para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    bool Contains(IIdentity identity);
    
    /// <summary>
    /// <para>
    /// Reads an entity from the current instance.  If the entity is not contained within this backing store instance
    /// then a null reference will be returned.
    /// </para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <returns>
    /// A <see cref="IEntity"/>
    /// </returns>
    IEntity Read(IIdentity identity);
    
    /// <summary>
    /// <para>Reads all of the entities from the backing store.</para>
    /// </summary>
    /// <returns>
    /// A collection of <see cref="IEntity"/>, indexed by their <see cref="IIdentity"/>.
    /// </returns>
    IDictionary<IIdentity, IEntity> ReadAll();

    /// <summary>
    /// <para>Reads all of the entity identities contained within this backing store.</para>
    /// </summary>
    /// <returns>
    /// A collection of <see cref="IIdentity"/>
    /// </returns>
    IList<IIdentity> ReadAllIdentities();
    
    /// <summary>
    /// <para>Removes aan entity from the current instance.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    void Remove(IIdentity identity);
    
    #endregion
  }
}

