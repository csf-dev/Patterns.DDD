//  
//  EntityAlreadyRegisteredException.cs
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
using CraigFowler.Patterns.DDD.Entities;

namespace CraigFowler.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Represents an <see cref="Exception"/> that occurs when an <see cref="IEntity"/> is registered twice with the same
  /// <see cref="UnitOfWork"/>.
  /// </para>
  /// </summary>
  public class EntityAlreadyRegisteredException : Exception
  {
    #region constants
    
    private const string DEFAULT_MESSAGE = "An entity has already been registered with the unit of work, cannot " +
                                           "re-register.";
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the entity reference that was already registered with a unit of work.</para>
    /// </summary>
    public IEntity Entity
    {
      get;
      private set;
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this instance.</para>
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/>
    /// </param>
    public EntityAlreadyRegisteredException (IEntity entity) : base(DEFAULT_MESSAGE)
    {
      this.Entity = entity;
    }
    
    #endregion
  }
}

