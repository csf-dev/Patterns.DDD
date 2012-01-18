//  
//  NHibernateRepositoryBase.cs
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
using System.Collections.Generic;
using System.Linq;
using CSF.Patterns.DDD.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace CSF.Patterns.DDD.Data.NHibernate
{
  /// <summary>
  /// <para>A general/all-purpose repository type for an NHibernate-based repository.</para>
  /// </summary>
  /// <remarks>
  /// <para>
  /// For many types that you would create repositories for, this implementation will be adequate.  If you require
  /// more fine-tuning however or some specialised methods then you may choose to subclass this repository and implement
  /// additional functionality.
  /// </para>
  /// </remarks>
  public class NHibernateRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : IEntity
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the <see cref="NHibernateRepositoryConnection"/> that this repository uses.</para>
    /// </summary>
    protected NHibernateRepositoryConnection Connection
    {
      get;
      private set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Provides shortcut access to the <see cref="NHibernate.ISession"/> from the <see cref="Connection"/>.
    /// </para>
    /// </summary>
    protected ISession Session
    {
      get {
        return this.Connection.Session;
      }
    }
    
    #endregion
    
    #region extensions to IRepository[TEntity]
    
    /// <summary>
    /// <para>Overloaded.  Reads a collection of entities based on given criteria.</para>
    /// </summary>
    /// <param name="criteria">
    /// A collection of <see cref="ICriterion"/>
    /// </param>
    /// <returns>
    /// A collection of <see cref="IEntity"/>
    /// </returns>
    public IList<TEntity> ReadCollection (IEnumerable<ICriterion> criteria)
    {
      ICriteria nhCriteria = this.Session.CreateCriteria(typeof(TEntity));
      
      foreach(ICriterion criterion in criteria)
      {
        nhCriteria.Add(criterion);
      }
      
      return nhCriteria.List<TEntity>();
    }
    
    /// <summary>
    /// <para>Overloaded.  Reads a collection of entities based on given criteria.</para>
    /// </summary>
    /// <param name="criteria">
    /// A collection of <see cref="ICriterion"/>
    /// </param>
    /// <returns>
    /// A collection of <see cref="IEntity"/>
    /// </returns>
    public IList<TEntity> ReadCollection (params ICriterion[] criteria)
    {
      return this.ReadCollection((IEnumerable<ICriterion>) criteria);
    }
    
    #endregion
    
    #region IRepository[TEntity] implementation
    
    /// <summary>
    /// <para>Creates/stores an entity in the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/>
    /// </param>
    public void Create (TEntity entity)
    {
      this.Session.Save(entity);
    }

    /// <summary>
    /// <para>Overloaded.  Reads a collection of entities based on given criteria.</para>
    /// </summary>
    /// <param name="criteria">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <returns>
    /// A collection of <see cref="IEntity"/>
    /// </returns>
    public IList<TEntity> ReadCollection (object criteria)
    {
      IEnumerable<ICriterion> paramCriteria = (IEnumerable<ICriterion>) criteria;
      return this.ReadCollection(paramCriteria);
    }
    
    /// <summary>
    /// <para>Reads a single entity using its identity.</para>
    /// </summary>
    /// <param name="identity">
    /// A <see cref="IIdentity"/>
    /// </param>
    /// <returns>
    /// An <see cref="IEntity"/>
    /// </returns>
    public TEntity Read (IIdentity identity)
    {
      return this.Session.Get<TEntity>(identity.Value);
    }

    /// <summary>
    /// <para>Updates an entity in the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Update (TEntity entity)
    {
      this.Session.Update(entity);
    }

    /// <summary>
    /// <para>Deletes an entity from the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// A <see cref="IEntity"/>
    /// </param>
    public void Delete (TEntity entity)
    {
      this.Session.Delete(entity);
    }
    
    /// <summary>
    /// <para>Conditionally creates or updates an entity in the repository.</para>
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/>
    /// </param>
    public void CreateOrUpdate (TEntity entity)
    {
      this.Session.SaveOrUpdate(entity);
    }
    
    /// <summary>
    /// <para>Provides access to this repository as a queryable object.</para>
    /// </summary>
    /// <returns>
    /// A queryable object instance.
    /// </returns>
    public IQueryable<TEntity> AsQueryable()
    {
      return this.Session.Query<TEntity>();
    }
    
    #endregion

    #region explicit IRepository implementation
    
    void IRepository.Create (IEntity entity)
    {
      this.Create((TEntity) entity);
    }

    IList<IEntity> IRepository.ReadCollection (object criteria)
    {
      IList<TEntity> results = this.ReadCollection(criteria);
      IList<IEntity> output = new List<IEntity>();
      
      foreach(IEntity result in results)
      {
        output.Add(result);
      }
      
      return output;
    }

    IEntity IRepository.Read (IIdentity identity)
    {
      return this.Read(identity);
    }

    void IRepository.Update (IEntity entity)
    {
      this.Update((TEntity) entity);
    }

    void IRepository.Delete (IEntity entity)
    {
      this.Delete((TEntity) entity);
    }

    void IRepository.CreateOrUpdate (IEntity entity)
    {
      this.CreateOrUpdate((TEntity) entity);
    }

    IRepositoryConnection IRepository.Connection
    {
      get {
        return this.Connection;
      }
    }
    
    IQueryable IRepository.AsQueryable()
    {
      return this.AsQueryable();
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises the current instance.</para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="NHibernateRepositoryConnection"/>
    /// </param>
    public NHibernateRepositoryBase(NHibernateRepositoryConnection connection)
    {
      this.Connection = connection;
    }
    
    #endregion
  }
}

