//  
//  IReplacementPolicy.cs
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
  /// <para>Describes a replacement policy (AKA a caching algorithm) for an <see cref="IEntityCache"/>.</para>
  /// </summary>
  public interface IReplacementPolicy
  {
    #region methods
    
    /// <summary>
    /// <para>
    /// General event handler for a cache-related event that this replacement policy may need to know about.
    /// </para>
    /// </summary>
    /// <param name="sender">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="ev">
    /// A <see cref="EventArgs"/>
    /// </param>
    void HandleCacheEvent(object sender, EventArgs ev);
    
    /// <summary>
    /// <para>Gets a list of the entities to remove from the cache.</para>
    /// </summary>
    /// <param name="howMany">
    /// A <see cref="System.Int32"/> indicating how many entities to include in the output.
    /// </param>
    /// <returns>
    /// A collection of <see cref="IIdentity"/> that enumerates the entities that are to be removed from the cache.
    /// </returns>
    IList<IIdentity> GetEntitiesToRemove(int howMany);
    
    #endregion
  }
}

