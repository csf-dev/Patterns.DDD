using System;
using CraigFowler.Patterns.DDD.Data;
using CraigFowler.Patterns.DDD.Entities;

namespace Test.CraigFowler.Patterns.DDD.Data
{
  public class MockRepositoryConnection : IRepositoryConnection
  {
    public void Dispose ()
    {
      
    }
    
    public IRepository GetRepository (Type entityType)
    {
      return null;
    }
    
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : IEntity
    {
      return null;
    }
    
    public bool HasTransactionSupport
    {
      get {
        return false;
      }
    }
    
    public IRepositoryTransaction CreateTransaction()
    {
      throw new NotImplementedException ();
    }
  }
}

