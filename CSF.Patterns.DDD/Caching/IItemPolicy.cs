//  
//  IItemPolicy.cs
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
using CSF.Patterns.DDD.Entities;

namespace CSF.Patterns.DDD.Caching
{
  /// <summary>
  /// <para>
  /// Interface for a policy which provides additional/supplementary control over an item within an
  /// <see cref="IEntityCache"/>.
  /// </para>
  /// </summary>
  public interface IItemPolicy
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets a value that indicates when this policy should be consulted.</para>
    /// </summary>
    ItemLifecycleMilestones RelevantMilestones { get; }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>
    /// General event handler for a caching event that affects the <see cref="IEntity"/> that this policy governs.
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
    /// <para>Gets the action that should be applied to the entity that this policy governs.</para>
    /// </summary>
    /// <param name="milestone">
    /// A <see cref="ItemLifecycleMilestones"/> that indicates the current milestone in the cached entity's lifecycle
    /// </param>
    /// <returns>
    /// A <see cref="ItemAction"/> indicating how the cached entity should be handled.
    /// </returns>
    ItemAction GetAction(ItemLifecycleMilestones milestone);
    
    /// <summary>
    /// <para>
    /// Determines whether the given <paramref name="milestone"/> is relevant in the lifecycle of the governed
    /// entity.
    /// </para>
    /// </summary>
    /// <param name="milestone">
    /// A <see cref="ItemLifecycleMilestones"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    bool IsRelevant(ItemLifecycleMilestones milestone);
    
    #endregion
  }
}

