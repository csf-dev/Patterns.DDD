//  
//  IRepository.cs
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

namespace CraigFowler.Patterns.DDD.Data
{
  /// <summary>
  /// <para>Interface for a single repository component.</para>
  /// </summary>
  public interface IRepository
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the <see cref="IRepositoryConnection"/> that is the backing for this repository.</para>
    /// </summary>
    IRepositoryConnection Connection { get; }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Creates the given <paramref name="entity"/> in the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/>
    /// </param>
    void Create(IEntity entity);
    
    /// <summary>
    /// <para>Retrieves a collection of <see cref="IEntity"/> from the repository.</para>
    /// </summary>
    /// <param name="criteria">
    /// A <see cref="System.Object"/> representing the criteria with which to use when selecting entities to retrieve.
    /// </param>
    /// <returns>
    /// A collection of <see cref="IEntity"/>
    /// </returns>
    IList<IEntity> ReadCollection(object criteria);
    
    /// <summary>
    /// <para>
    /// Retrieves a single <see cref="IEntity"/> from the repository using its unique <paramref name="identity"/>.
    /// </para>
    /// </summary>
    /// <param name="identity">
    /// An <see cref="IIdentity"/> that uniquely identifies the desired entity.
    /// </param>
    /// <returns>
    /// An <see cref="IEntity"/>
    /// </returns>
    IEntity Read(IIdentity identity);
    
    /// <summary>
    /// <para>Updates the given <paramref name="entity"/> in the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/>
    /// </param>
    void Update(IEntity entity);
    
    /// <summary>
    /// <para>Deletes the given <paramref name="entity"/> from the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/>
    /// </param>
    void Delete(IEntity entity);
    
    /// <summary>
    /// <para>Creates the given <paramref name="entity"/> in the repository or updates it if it already exists.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    void CreateOrUpdate(IEntity entity);
    
    #endregion
  }
}

