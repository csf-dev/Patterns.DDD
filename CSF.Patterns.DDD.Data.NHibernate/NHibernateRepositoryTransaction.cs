//  
//  NHibernateRepositoryTransaction.cs
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

namespace CSF.Patterns.DDD.Data.NHibernate
{
  /// <summary>
  /// <para>Provides a <see cref="IRepositoryTransaction"/> that wraps an NHibernate transaction.</para>
  /// </summary>
  public class NHibernateRepositoryTransaction : IRepositoryTransaction
  {
    #region fields
    
    private global::NHibernate.ITransaction _nhTransaction;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the underlying NHibernate transaction.</para>
    /// </summary>
    public global::NHibernate.ITransaction NHibernateTransaction
    {
      get {
        return _nhTransaction;
      }
      private set {
        if(value == null)
        {
          throw new ArgumentNullException("value");
        }
        
        _nhTransaction = value;
      }
    }
    
    #endregion
    
    #region IRepositoryTransaction implementation

    /// <summary>
    /// <para>Commits the underlying transaction.</para>
    /// </summary>
    public void Commit ()
    {
      this.NHibernateTransaction.Commit();
    }

    /// <summary>
    /// <para>Rolls the underlying transaction back.</para>
    /// </summary>
    public void Rollback ()
    {
      this.NHibernateTransaction.Rollback();
    }
    
    #endregion

    #region IDisposable implementation
    
    /// <summary>
    /// <para>Disposes the underlying transaction.</para>
    /// </summary>
    public void Dispose ()
    {
      this.NHibernateTransaction.Dispose();
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this instance with a new <see cref="NHibernate.ITransaction"/>.</para>
    /// </summary>
    /// <param name="transaction">
    /// A <see cref="NHibernate.ITransaction"/>
    /// </param>
    public NHibernateRepositoryTransaction (global::NHibernate.ITransaction transaction)
    {
      this.NHibernateTransaction = transaction;
    }
    
    #endregion
  }
}

