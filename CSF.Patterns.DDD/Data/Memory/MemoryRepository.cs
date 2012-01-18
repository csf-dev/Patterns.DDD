//  
//  MemoryRepository.cs
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
using System.Collections.Generic;
using System.Linq;

namespace CSF.Patterns.DDD.Data.Memory
{
  /// <summary>
  /// <para>An <see cref="IRepository"/> implementation that uses a simple in-memory backing store.</para>
  /// </summary>
  public class MemoryRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
  {
    #region fields
    
    private Dictionary<IIdentity, TEntity> _backingStore;
    private uint _autoIncrement;
    
    #endregion
    
    #region IRepository[TEntity] implementation
    
    /// <summary>
    /// <para>Creates a new entity within the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Create (TEntity entity)
    {
      if(entity.HasIdentity)
      {
        throw new ArgumentException("Entity already has an identity, cannot create.", "entity");
      }
      
      entity.SetIdentity(++_autoIncrement);
      _backingStore.Add(entity.GetIdentity(), entity);
    }

    /// <summary>
    /// <para>Reads a collection of <see cref="IEntity"/> from the current instance.</para>
    /// </summary>
    /// <param name="criteria">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <returns>
    /// A collection of <see cref="IEntity"/>
    /// </returns>
    public IList<TEntity> ReadCollection (object criteria)
    {
      throw new NotImplementedException ();
    }
    
    /// <summary>
    /// <para>Gets an <see cref="IQueryable"/> that may be used to query the repository.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IQueryable"/>
    /// </returns>
    public IQueryable<TEntity> AsQueryable()
    {
      return _backingStore.Values.AsQueryable();
    }

    /// <summary>
    /// <para>Reads an <see cref="IEntity"/> from this instance using its identity.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <returns>
    /// A <see cref="IEntity"/>
    /// </returns>
    public TEntity Read (IIdentity identity)
    {
      return _backingStore[identity];
    }

    /// <summary>
    /// <para>Updates an <see cref="IEntity"/> in the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Update (TEntity entity)
    {
      if(!entity.HasIdentity)
      {
        throw new ArgumentException("Entity does not have an identity, cannot update.", "entity");
      }
      
      // Intentional no-operation
    }

    /// <summary>
    /// <para>Removes a <see cref="IEntity"/> from the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Delete (TEntity entity)
    {
      _backingStore.Remove(entity.GetIdentity());
    }

    /// <summary>
    /// <para>Conditionally creates or updates a <see cref="IEntity"/> in the current instance</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void CreateOrUpdate (TEntity entity)
    {
      if(_backingStore.ContainsKey(entity.GetIdentity()))
      {
        _backingStore.Remove(entity.GetIdentity());
      }
    }
    #endregion

    #region IRepository implementation
    
    /// <summary>
    /// <para>Creates a <see cref="IEntity"/> in the current instance.s</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Create (IEntity entity)
    {
      this.Create((TEntity) entity);
    }

    IList<IEntity> IRepository.ReadCollection (object criteria)
    {
      throw new NotImplementedException ();
    }

    IQueryable IRepository.AsQueryable ()
    {
      return this.AsQueryable();
    }

    IEntity IRepository.Read (IIdentity identity)
    {
      return this.Read(identity);
    }

    /// <summary>
    /// <para>Updates an <see cref="IEntity"/> in the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Update (IEntity entity)
    {
      this.Update((TEntity) entity);
    }

    /// <summary>
    /// <para>Removes a <see cref="IEntity"/> from the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Delete (IEntity entity)
    {
      this.Delete((TEntity) entity);
    }

    /// <summary>
    /// <para>Conditionally creates or updates a <see cref="IEntity"/> in the current instance</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void CreateOrUpdate (IEntity entity)
    {
      this.CreateOrUpdate((TEntity) entity);
    }
  
    /// <summary>
    /// <para>Read-only.  Gets the connection associated with the current instance.</para>
    /// </summary>
    public IRepositoryConnection Connection
    {
      get;
      private set;
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises the current instance.</para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    public MemoryRepository(IRepositoryConnection connection)
    {
      this.Connection = connection;
      _backingStore = new Dictionary<IIdentity, TEntity>();
      _autoIncrement = 0;
    }
    
    #endregion
  }
}

