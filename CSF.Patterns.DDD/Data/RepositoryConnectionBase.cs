//  
//  RepositoryConnectionBase.cs
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
  /// <para>
  /// Base class for types that will implement <see cref="IRepositoryConnection"/>.  Provides re-usable code for
  /// these types.
  /// </para>
  /// </summary>
  public abstract class RepositoryConnectionBase : IRepositoryConnection
  {
    #region fields
    
    private bool _disposed;
    private IRepositoryFactory _factory;
    
    #endregion
    
    #region properties

    /// <summary>
    /// <para>
    /// Read-only.  Gets a reference to the <see cref="IRepositoryFactory"/> that this connection was created from.
    /// </para>
    /// </summary>
    public virtual IRepositoryFactory Factory
    {
      get {
        return _factory;
      }
      private set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        _factory = value;
      }
    }
    
    #endregion
    
    #region IRepositoryConnection implementation

    /// <summary>
    /// <para>Read-only.  Gets whether or not this repository connection type supports transactions.</para>
    /// </summary>
    public abstract bool HasTransactionSupport { get; }
    
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
    public virtual IRepository GetRepository(Type entityType)
    {
      return this.Factory.GetRepository(entityType, this);
    }

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
    public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : IEntity
    {
      return this.Factory.GetRepository<TEntity>(this);
    }

    /// <summary>
    /// <para>Creates and returns a new <see cref="IRepositoryTransaction"/>.</para>
    /// </summary>
    /// <returns>
    /// An <see cref="IRepositoryTransaction"/>
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// <para>If the underlying repository backend does not support transactions.</para>
    /// <seealso cref="HasTransactionSupport"/>
    /// </exception>
    public abstract IRepositoryTransaction CreateTransaction();
    
    #endregion

    #region IDisposable implementation
    
    /// <summary>
    /// <para>Disposes this instance.</para>
    /// </summary>
    public void Dispose ()
    {
      if(!_disposed)
      {
        this.PerformDisposal();
      }
    }
    
    /// <summary>
    /// <para>Performs the actual disposal tasks relating to this instance.</para>
    /// </summary>
    protected virtual void PerformDisposal()
    {
      if(_disposed)
      {
        throw new ObjectDisposedException(this.GetType().FullName);
      }
      
      _disposed = true;
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>
    /// Initialises this instance with information about the <see cref="IRepositoryFactory"/> that created it.
    /// </para>
    /// </summary>
    /// <param name="factory">
    /// A <see cref="IRepositoryFactory"/>
    /// </param>
    public RepositoryConnectionBase(IRepositoryFactory factory)
    {
      _disposed = false;
      
      this.Factory = factory;
    }
    
    /// <summary>
    /// <para>Implicit destructor for this type, only used if disposal is forgotten.</para>
    /// </summary>
    ~RepositoryConnectionBase()
    {
      this.Dispose();
    }
    
    #endregion
  }
}

