//  
//  IRepositoryConnection.cs
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
  /// <para>Interface for types that are capable of generating <see cref="IRepository"/> instances.</para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// Types that implement <see cref="IRepositoryConnection"/> should always be immutable wrappers around 'connections'
  /// to repositories.  This may not actually involve the use of an actual connection
  /// </para>
  /// </remarks>
  public interface IRepositoryConnection : IDisposable
  {
    #region properties
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets whether or not the connection represented by the current instance supports transactions.
    /// </para>
    /// </summary>
    bool HasTransactionSupport { get; }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>
    /// Overloaded.  Convenience method gets an <see cref="IRepository"/> appropriate for use with a given
    /// <see cref="Type"/> that implements <see cref="IEntity"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides a convenient way to call
    /// <see cref="IRepositoryFactory.GetRepository(Type,IRepositoryConnection)"/> without needing to reference the
    /// repository factory again directly.
    /// </para>
    /// </remarks>
    /// <param name="entityType">
    /// A <see cref="Type"/> that implement <see cref="IEntity"/>
    /// </param>
    /// <returns>
    /// An <see cref="IRepository"/>
    /// </returns>
    IRepository GetRepository(Type entityType);
    
    /// <summary>
    /// <para>
    /// Overloaded.  Convenience method gets an appropriate <see cref="IRepository"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides a convenient way to call the generic version of 
    /// <see cref="IRepositoryFactory.GetRepository(Type,IRepositoryConnection)"/> without needing to reference the
    /// repository factory again directly.
    /// </para>
    /// </remarks>
    /// <returns>
    /// An <see cref="IRepository"/> appropriate to the type of <see cref="IEntity"/> specified as the generic
    /// parameter to this method.
    /// </returns>
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : IEntity;
    
    /// <summary>
    /// <para>
    /// Begins a <see cref="IRepositoryTransaction"/> so that calls to this repository (and child repositories) may be
    /// made in a consistent manner and rolled back or comitted atomically.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryTransaction"/>
    /// </returns>
    IRepositoryTransaction CreateTransaction();
    
    #endregion
  }
}

