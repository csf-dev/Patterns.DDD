//  
//  MemoryRepositoryConnection.cs
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
  /// <para>Represents a connection to an in-memory repository.</para>
  /// </summary>
  public class MemoryRepositoryConnection : IRepositoryConnection
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the parent repository factory for this connection.</para>
    /// </summary>
    public MemoryRepositoryFactory Factory
    {
      get;
      private set;
    }
    
    #endregion
    
    #region IRepositoryConnection implementation
    
    /// <summary>
    /// <para>Overloaded.  Gets an <see cref="IRepository"/> instance from this connection.</para>
    /// </summary>
    /// <param name="entityType">
    /// A <see cref="Type"/>
    /// </param>
    /// <returns>
    /// A <see cref="IRepository"/>
    /// </returns>
    public IRepository GetRepository (Type entityType)
    {
      return this.Factory.GetRepository(entityType, this);
    }

    /// <summary>
    /// <para>Overloaded.  Gets an <see cref="IRepository"/> instance from this connection.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepository"/>
    /// </returns>
    public IRepository<TEntity> GetRepository<TEntity> () where TEntity : IEntity
    {
      return this.Factory.GetRepository<TEntity>(this);
    }

    /// <summary>
    /// <para>Creates a transaction from the current instance.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryTransaction"/>
    /// </returns>
    public IRepositoryTransaction CreateTransaction ()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// <para>Read-only.  Gets whether the current instance has transaction support.</para>
    /// </summary>
    public bool HasTransactionSupport
    {
      get {
        return false;
      }
    }
    
    #endregion

    #region IDisposable implementation
    
    /// <summary>
    /// <para>Disposes of the current instance</para>
    /// </summary>
    public void Dispose ()
    {
      // Intentional no-operation
    }
    
    #endregion
    
    #region constructor

    /// <summary>
    /// <para>Initialises the current instance.</para>
    /// </summary>
    /// <param name="factory">
    /// A <see cref="MemoryRepositoryFactory"/>
    /// </param>
    public MemoryRepositoryConnection (MemoryRepositoryFactory factory)
    {
      this.Factory = factory;
    }
    
    #endregion
  }
}

