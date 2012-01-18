//  
//  DummyRepositoryConnection.cs
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
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace CSF.Patterns.DDD.Mocks.Data
{
  public class DummyRepositoryConnection : IRepositoryConnection
  {
    #region log
    
    public StringBuilder Log = new StringBuilder();
    
    #endregion
    
    #region IRepositoryConnection implementation
    
    public void Dispose ()
    {
      // Intentional no-op
    }
    
    public IRepository GetRepository (Type entityType)
    {
      if(entityType == null)
      {
        throw new ArgumentNullException("entityType");
      }
      else if(entityType.GetInterface(typeof(IEntity).FullName) == null)
      {
        throw new ArgumentException("The type must implement IEntity", "entityType");
      }
      
      MethodInfo method = typeof(DummyRepositoryConnection).GetMethod("GetRepository", new Type[] {});
      return (IRepository) method.MakeGenericMethod(entityType).Invoke(this, null);
    }
    
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : IEntity
    {
      return new LoggingRepository<TEntity>(this.Log);
    }
    
    public bool HasTransactionSupport
    {
      get {
        return false;
      }
    }
    
    public IRepositoryTransaction CreateTransaction()
    {
      return new DummyRepositoryTransaction();
    }
    
    #endregion
    
    #region contained types
    
    public class LoggingRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
      #region log
      
      private StringBuilder _log;
      
      public LoggingRepository (StringBuilder log)
      {
        _log = log;
      }
      
      #endregion

      #region IRepository[TEntity] implementation
      
      public void Create (TEntity entity)
      {
        _log.AppendFormat("Create entity '{0}'\n", entity);
      }

      public IList<TEntity> ReadCollection (object criteria)
      {
        _log.AppendFormat("Read collection of entities using criteria '{0}'\n", criteria);
        return new List<TEntity>();
      }

      public TEntity Read (IIdentity identity)
      {
        _log.AppendFormat("Read entity with identity '{0}'\n", identity);
        return default(TEntity);
      }

      public void Update (TEntity entity)
      {
        _log.AppendFormat("Update entity '{0}'\n", entity);
      }

      public void Delete (TEntity entity)
      {
        _log.AppendFormat("Delete entity '{0}'\n", entity);
      }
      
      public void CreateOrUpdate (TEntity entity)
      {
        _log.AppendFormat("Create/epdate entity '{0}'\n", entity);
      }
      
      #endregion

      #region IRepository implementation
      
      public void Create (IEntity entity)
      {
        this.Create((TEntity) entity);
      }

      IList<IEntity> IRepository.ReadCollection (object criteria)
      {
        List<IEntity> output = new List<IEntity>();
        IList<TEntity> results = this.ReadCollection(criteria);
        
        foreach(IEntity entity in results)
        {
          output.Add(entity);
        }
        
        return output;
      }

      IEntity IRepository.Read (IIdentity identity)
      {
        return this.Read(identity);
      }

      public void Update (IEntity entity)
      {
        this.Update((TEntity) entity);
      }

      public void Delete (IEntity entity)
      {
        this.Delete((TEntity) entity);
      }

      public IRepositoryConnection Connection
      {
        get {
          throw new NotImplementedException ();
        }
      }

      public void CreateOrUpdate (IEntity entity)
      {
        this.CreateOrUpdate((TEntity) entity);
      }
      
      #endregion
    }
    
    #endregion
  }
}
