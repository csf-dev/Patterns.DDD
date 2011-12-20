//  
//  ItemLifecycleMilestones.cs
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

namespace CraigFowler.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>
  /// Enumerates when an <see cref="IItemPolicy"/> is considered.
  /// </para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="IItemPolicy"/> instances may be used to provide additional/supplementary control over how entities are
  /// stored in an <see cref="IEntityCache"/>.  These policies may be checked at various points in time to determine
  /// whether the entity may remain in the cache.  If an item policy does not indicate that it should be considered at
  /// a given phase in the lifecycle of the item and the cache then the policy is not consulted at that point and the
  /// cache behaves as if no policy were present.
  /// </para>
  /// </remarks>
  [Flags]
  public enum ItemLifecycleMilestones : int
  {
    /// <summary>
    /// <para>
    /// The item policy will never be consulted.  Item policies with this <see cref="IItemPolicy.RelevantMilestones"/>
    /// are equivalent to missing item policies, since they will never be used.
    /// </para>
    /// </summary>
    Never                     = 0,
    
    /// <summary>
    /// <para>
    /// The item policy will be consulted any time that a cache hit occurs.  This means that the item policy may turn
    /// a cache hit into a cache miss.
    /// </para>
    /// </summary>
    OnCacheHit                = 1,
    
    /// <summary>
    /// <para>
    /// The item policy will be consulted any time that a 'cache-wide cleanup' operation takes place.  In this scenario,
    /// item policies are consulted before (and independently of) the cache's <see cref="IReplacementPolicy"/>.  Thus,
    /// items may be marked for removal from the cache by their item policy even when the cache-wide replacement policy
    /// would not have removed them.
    /// </para>
    /// </summary>
    OnCacheCleanup            = 2,
    
    /// <summary>
    /// <para>
    /// The item policy will be consulted whenever the entity would be removed from the cache.  In this scenario the
    /// policy is consulted when the entity would be removed from the cache by the cache-wide
    /// <see cref="IReplacementPolicy"/>.  Thus, the entity's item policy may cancel the replacement/removal of the
    /// entity from the cache.  Use this type of consideration with care as it can lead to caches that keep items
    /// for longer than they need to.
    /// </para>
    /// </summary>
    OnReplacement             = 4
  }
}

