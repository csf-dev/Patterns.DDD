//  
//  IRepositoryFactory.cs
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
using CSF.Patterns.DDD.Data;
using CSF.Patterns.DDD.Entities;

namespace CSF.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Interface for a repository factory, a type that is capable of creating <see cref="IRepository"/> instances.
  /// </para>
  /// </summary>
  public interface IRepositoryFactory
  {
    #region methods
    
    /// <summary>
    /// <para>Gets a connection to the repository backend provided by the current instance.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    IRepositoryConnection GetConnection();
    
    /// <summary>
    /// <para>
    /// Overloaded.  Gets a repository for the given <paramref name="entityType"/>, using an explicitly-created
    /// <paramref name="connection"/> to the repository backend.
    /// </para>
    /// </summary>
    /// <param name="entityType">
    /// A <see cref="Type"/>
    /// </param>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    /// <returns>
    /// A repository instance.
    /// </returns>
    IRepository GetRepository(Type entityType, IRepositoryConnection connection);
    
    /// <summary>
    /// <para>
    /// Overloaded.  Gets a repository for a generic <see cref="IEntity"/> type, using an explicitly-created
    /// <paramref name="connection"/> to the repository backend.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    /// <returns>
    /// A repository instance.
    /// </returns>
    IRepository<TEntity> GetRepository<TEntity>(IRepositoryConnection connection) where TEntity : IEntity;
    
    #endregion
  }
}

