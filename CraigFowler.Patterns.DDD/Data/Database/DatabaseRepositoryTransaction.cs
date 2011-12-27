//  
//  RepositoryDatabaseTransaction.cs
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

using System;
using System.Data;

namespace CraigFowler.Patterns.DDD.Data.Database
{
  /// <summary>
  /// <para>An <see cref="IRepositoryTransaction"/> that wraps <see cref="IDbTransaction"/>.</para>
  /// </summary>
  public class DatabaseRepositoryTransaction : IRepositoryTransaction
  {
    #region fields
    
    private IDbTransaction _dbTransaction;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the underlying database transaction.</para>
    /// </summary>
    public IDbTransaction DatabaseTransaction
    {
      get {
        return _dbTransaction;
      }
      private set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        _dbTransaction = value;
      }
    }
    
    #endregion
    
    #region IRepositoryTransaction implementation

    /// <summary>
    /// <para>Commits the underlying transaction.</para>
    /// </summary>
    public void Commit ()
    {
      this.DatabaseTransaction.Commit();
    }

    /// <summary>
    /// <para>Rolls the underlying transaction back.</para>
    /// </summary>
    public void Rollback ()
    {
      this.DatabaseTransaction.Rollback();
    }
    
    #endregion

    #region IDisposable implementation
    
    /// <summary>
    /// <para>Disposes the underlying transaction.</para>
    /// </summary>
    public void Dispose ()
    {
      this.DatabaseTransaction.Dispose();
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this instance with a new <see cref="IDbTransaction"/>.</para>
    /// </summary>
    /// <param name="transaction">
    /// A <see cref="IDbTransaction"/>
    /// </param>
    public DatabaseRepositoryTransaction (IDbTransaction transaction)
    {
      this.DatabaseTransaction = transaction;
    }
    
    #endregion
  }
}

