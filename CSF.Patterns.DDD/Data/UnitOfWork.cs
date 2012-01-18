//  
//  UnitOfWork.cs
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
using System.Collections.Generic;
using CSF.Patterns.DDD.Entities;

namespace CSF.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Tracks changes to <see cref="IEntity"/> domain entities, ready for writing them to an <see cref="IRepository"/>
  /// data store.
  /// </para>
  /// </summary>
  public class UnitOfWork : IDisposable
  {
    #region fields
    
    private IRepositoryConnection _connection;
    private bool _disposed;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets the <see cref="IRepositoryConnection"/> that this <see cref="UnitOfWork"/> will perform tasks
    /// against.
    /// </para>
    /// </summary>
    protected IRepositoryConnection Connection
    {
      get {
        return _connection;
      }
      private set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        _connection = value;
      }
    }
    
    #endregion
    
    #region entity lists
    
    /// <summary>
    /// <para>
    /// Read-only.  Stores a collection of the new entities to be created when this tracker commits its data.
    /// </para>
    /// </summary>
    protected List<IEntity> NewEntities
    {
      get;
      private set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Stores a collection of the entities to be deleted when this tracker commits its data.
    /// </para>
    /// </summary>
    protected List<IEntity> DeletedEntities
    {
      get;
      private set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Stores a collection of the entities that are dirty and should be updated when this tracker commits
    /// its data.
    /// </para>
    /// </summary>
    protected List<IEntity> DirtyEntities
    {
      get;
      private set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Stores a collection of the entities that have not been modified and should be left untouched when
    /// this tracker commits its data.
    /// </para>
    /// </summary>
    protected List<IEntity> CleanEntities
    {
      get;
      private set;
    }
    
    #endregion
    
    #region entity registration
    
    /// <summary>
    /// <para>Registers an entity with this tracker for the first time.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Register(IEntity entity)
    {
      if(entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      
      this.CheckNotAlreadyRegistered(entity,
                                     this.CleanEntities,
                                     this.NewEntities,
                                     this.DirtyEntities,
                                     this.DeletedEntities);
      
      if(entity.HasIdentity)
      {
        this.CleanEntities.Add(entity);
      }
      else
      {
        this.NewEntities.Add(entity);
      }
      
      entity.Deleted += HandleDeleted;
      entity.Dirty += HandleDirty;
    }
    
    /// <summary>
    /// <para>Registers an entity with this tracker as dirty (needing update).</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void RegisterDirty(IEntity entity)
    {
      if(entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      
      this.CheckNotAlreadyRegistered(entity,
                                     this.NewEntities, this.DirtyEntities, this.DeletedEntities);
      
      this.CleanEntities.Remove(entity);
      this.DirtyEntities.Add(entity);
    }
    
    /// <summary>
    /// <para>Registers an entity with this tracker as deleted.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void RegisterDeleted(IEntity entity)
    {
      if(entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      
      this.CheckNotAlreadyRegistered(entity,
                                     this.NewEntities, this.DeletedEntities);
      
      this.CleanEntities.Remove(entity);
      this.DirtyEntities.Remove(entity);
      this.DeletedEntities.Add(entity);
    }
    
    /// <summary>
    /// <para>Gets the state of an <paramref name="entity"/> within the current instance.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    /// <returns>
    /// A <see cref="EntityLifecycleState"/>
    /// </returns>
    public EntityLifecycleState GetState(IEntity entity)
    {
      EntityLifecycleState output;
      
      if(this.CleanEntities.Contains(entity))
      {
        output = EntityLifecycleState.Clean;
      }
      else if(this.NewEntities.Contains(entity))
      {
        output = EntityLifecycleState.New;
      }
      else if(this.DirtyEntities.Contains(entity))
      {
        output = EntityLifecycleState.Dirty;
      }
      else if(this.DeletedEntities.Contains(entity))
      {
        output = EntityLifecycleState.Deleted;
      }
      else
      {
        output = EntityLifecycleState.NotRegistered;
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Retrieves a collection of <see cref="IEntity"/> from the repository.</para>
    /// </summary>
    /// <param name="criteria">
    /// A <see cref="System.Object"/> representing the criteria with which to use when selecting entities to retrieve.
    /// </param>
    /// <returns>
    /// A collection of <see cref="IEntity"/>
    /// </returns>
    public IList<TEntity> ReadCollection<TEntity>(object criteria) where TEntity : IEntity
    {
      if(_disposed)
      {
        throw new ObjectDisposedException(this.GetType().FullName);
      }
      
      IRepository<TEntity> repository = this.Connection.GetRepository<TEntity>();
      IList<TEntity> output = repository.ReadCollection(criteria);
      
      foreach(IEntity entity in output)
      {
        entity.RegisterUnitOfWork(this);
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>
    /// Retrieves a <see cref="IEntity"/> from the repository, typically by using a unique identifier of some sort.
    /// </para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/> that uniquely identifies the desired entity.
    /// </param>
    /// <returns>
    /// An <see cref="IEntity"/>
    /// </returns>
    public TEntity Read<TEntity>(IIdentity identity) where TEntity : IEntity
    {
      if(_disposed)
      {
        throw new ObjectDisposedException(this.GetType().FullName);
      }
      
      IRepository<TEntity> repository = this.Connection.GetRepository<TEntity>();
      TEntity output = repository.Read(identity);
      
      output.RegisterUnitOfWork(this);
      
      return output;
    }
    
    /// <summary>
    /// <para>
    /// Checks that an <paramref name="entity"/> is not already registered with the given
    /// <paramref name="entityCollections"/> and throws an exception if it is.
    /// </para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    /// <param name="entityCollections">
    /// A collection-of-collections of <see cref="IEntity"/>
    /// </param>
    private void CheckNotAlreadyRegistered(IEntity entity, params IList<IEntity>[] entityCollections)
    {
      if(this.IsAlreadyRegistered(entity, entityCollections))
      {
        throw new EntityAlreadyRegisteredException(entity);
      }
    }
    
    /// <summary>
    /// <para>
    /// Determines whether an <paramref name="entity"/> is already registered with the given
    /// <paramref name="entityCollections"/>.
    /// </para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    /// <param name="entityCollections">
    /// A collection-of-collections of <see cref="IEntity"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    private bool IsAlreadyRegistered(IEntity entity, params IList<IEntity>[] entityCollections)
    {
      bool output = false;
      
      if(entity == null)
      {
        throw new ArgumentNullException("entity");
      }
      else if(entityCollections == null)
      {
        throw new ArgumentNullException("entityCollections");
      }
      
      foreach(IList<IEntity> entityCollection in entityCollections)
      {
        if(entityCollection.Contains(entity))
        {
          output = true;
          break;
        }
      }
      
      return output;
    }

    
    #endregion
    
    #region entity tracker lifecycle
    
    /// <summary>
    /// <para>Overloaded.  Commits this entity tracker's workload to the repository.</para>
    /// </summary>
    public virtual void PerformWork()
    {
      using(IRepositoryTransaction transaction = this.Connection.CreateTransaction())
      {
        this.PerformWork(transaction);
        transaction.Commit();
      }
    }
    
    /// <summary>
    /// <para>Overloaded.  Commits this entity tracker's workload to the repository.</para>
    /// </summary>
    /// <param name="transaction">
    /// A <see cref="IRepositoryTransaction"/>
    /// </param>
    public virtual void PerformWork(IRepositoryTransaction transaction)
    {
      if(_disposed)
      {
        throw new ObjectDisposedException(this.GetType().FullName, "This entity tracker has already been disposed.");
      }
      
      foreach(IEntity entity in this.NewEntities)
      {
        entity.Create(this.Connection);
      }
      
      foreach(IEntity entity in this.DirtyEntities)
      {
        entity.Update(this.Connection);
      }
      
      foreach(IEntity entity in this.DeletedEntities)
      {
        entity.Delete(this.Connection);
      }
    }
    
    /// <summary>
    /// <para>Performs explicit disposal of this object instance.</para>
    /// </summary>
    public void Dispose()
    {
      if(!_disposed)
      {
        this.PerformDisposal();
      }
    }
    
    /// <summary>
    /// <para>Performs disposal tasks on this instance.</para>
    /// </summary>
    protected virtual void PerformDisposal()
    {
      this.UnregisterEventHandlers(this.NewEntities, this.CleanEntities, this.DeletedEntities, this.DirtyEntities);
      _disposed = true;
    }
    
    /// <summary>
    /// <para>
    /// Unregisters events from all of the <see cref="IEntity"/> instances within the given
    /// <paramref name="entityCollections"/>.
    /// </para>
    /// </summary>
    /// <param name="entityCollections">
    /// A collection-of-collections of <see cref="IEntity"/>
    /// </param>
    private void UnregisterEventHandlers(params IList<IEntity>[] entityCollections)
    {
      foreach(IList<IEntity> collection in entityCollections)
      {
        foreach(IEntity entity in collection)
        {
          entity.Deleted -= this.HandleDeleted;
          entity.Dirty -= this.HandleDirty;
        }
      }
    }
    
    #endregion
    
    #region event handlers
    
    /// <summary>
    /// <para>Event handler for an entity requesting deletion.</para>
    /// </summary>
    /// <param name="sender">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="ev">
    /// A <see cref="EventArgs"/>
    /// </param>
    private void HandleDeleted(object sender, EventArgs ev)
    {
      if(sender is IEntity && !this.IsAlreadyRegistered((IEntity) sender,
                                                        this.NewEntities, this.DeletedEntities))
      {
        this.RegisterDeleted((IEntity) sender);
      }
    }
    
    /// <summary>
    /// <para>Event hander for an entity marking itself as dirty.</para>
    /// </summary>
    /// <param name="sender">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="ev">
    /// A <see cref="EventArgs"/>
    /// </param>
    private void HandleDirty(object sender, EventArgs ev)
    {
      if(sender is IEntity && !this.IsAlreadyRegistered((IEntity) sender,
                                                        this.NewEntities, this.DirtyEntities, this.DeletedEntities))
      {
        this.RegisterDirty((IEntity) sender);
      }
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this unit of work to create connections to the default repository.</para>
    /// </summary>
    public UnitOfWork(IRepositoryConnection connection)
    {
      this.Connection = connection;
      
      _disposed = false;
      
      this.NewEntities = new List<IEntity>();
      this.DeletedEntities = new List<IEntity>();
      this.DirtyEntities = new List<IEntity>();
      this.CleanEntities = new List<IEntity>();
    }
    
    /// <summary>
    /// <para>
    /// Initialises this unit of work using a newly-created connection from the given <see cref="IRepositoryFactory"/>.
    /// </para>
    /// </summary>
    /// <param name="repositoryFactory">
    /// A <see cref="IRepositoryFactory"/>
    /// </param>
    public UnitOfWork (IRepositoryFactory repositoryFactory) : this(repositoryFactory.GetConnection()) {}
    
    #endregion
  }
}

