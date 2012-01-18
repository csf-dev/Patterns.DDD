//  
//  EntityLifecycleState.cs
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

namespace CSF.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Enumerates the states that an <see cref="IEntity"/> can be in during its lifecyle registered to a
  /// <see cref="UnitOfWork"/>.
  /// </para>
  /// </summary>
  public enum EntityLifecycleState : int
  {
    /// <summary>
    /// <para>The entity is unrecognised to the unit of work.</para>
    /// </summary>
    NotRegistered = 0,
    
    /// <summary>
    /// <para>The entity is registered with the unit of work but has not been modified.</para>
    /// </summary>
    Clean,
    
    /// <summary>
    /// <para>
    /// The entity is new and requires creation within the persistant storage when the unit of work's workload is
    /// committed.
    /// </para>
    /// </summary>
    New,
    
    /// <summary>
    /// <para>
    /// The entity is dirty and requires update within the persistant storage when the unit of work's workload is
    /// committed.
    /// </para>
    /// </summary>
    Dirty,
    
    /// <summary>
    /// <para>
    /// The entity is to be deleted within the persistant storage when the unit of work's workload is committed.
    /// </para>
    /// </summary>
    Deleted
  }
}

