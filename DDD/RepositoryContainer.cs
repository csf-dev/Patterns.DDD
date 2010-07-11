
using System;
using System.Data;

namespace CraigFowler.Patterns.DDD
{
  public abstract class RepositoryContainer : IDisposable
  {
    #region fields
    
    private IDbConnection dbConnection;
    private bool disposed;
    
    #endregion
    
    #region properties
    
    protected IDbConnection DbConnection
    {
      get {
        return dbConnection;
      }
      set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        dbConnection = value;
      }
    }
    
    public bool Disposed
    {
      get {
        return disposed;
      }
      private set {
        disposed = value;
      }
    }
    
    #endregion
    
    #region methods
    
    public virtual void Dispose()
    {
      if(this.Disposed)
      {
        throw new ObjectDisposedException("this");
      }
      
      this.DbConnection.Dispose();
      this.Disposed = true;
    }
    
    #endregion
    
    #region constructor and destructor
    
    public RepositoryContainer(IDbConnection db)
    {
      this.DbConnection = db;
      this.Disposed = false;
    }
    
    ~RepositoryContainer()
    {
      if(!Disposed)
      {
        Dispose();
      }
    }
    
    #endregion
  }
}
