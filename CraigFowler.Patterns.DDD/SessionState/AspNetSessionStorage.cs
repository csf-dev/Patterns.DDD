//  
//  AspNetSessionStorage.cs
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
using System.Web;
using System.Web.SessionState;

namespace CraigFowler.Patterns.DDD.SessionState
{
  /// <summary>
  /// <para>Implementation of <see cref="SessionStorage"/> using the ASP.NET session state as the backend.</para>
  /// </summary>
  public class AspNetSessionStorage : SessionStorage
  {
    #region SessionStorage implementation
    
    /// <summary>
    /// <para>Gets a value from the session storage by its <paramref name="key"/>.</para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Object"/>
    /// </returns>
    public override object GetValue(string key)
    {
      object output = null;
      
      if(key == null)
      {
        throw new ArgumentNullException("key");
      }
      
      try
      {
        this.SyncRoot.EnterReadLock();
        output = this.GetStorageBackend()[key];
      }
      finally
      {
        if(this.SyncRoot.IsReadLockHeld)
        {
          this.SyncRoot.ExitReadLock();
        }
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Stores a value in the session storage by its <paramref name="key"/>.</para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.Object"/>
    /// </param>
    /// <param name="value">
    /// A <see cref="System.Object"/>
    /// </param>
    public override void StoreValue(string key, object value)
    {
      if(key == null)
      {
        throw new ArgumentNullException("key");
      }
      
      try
      {
        this.SyncRoot.EnterWriteLock();
        HttpSessionState backend = this.GetStorageBackend();
        backend[key] = value;
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
      }
    }
    
    /// <summary>
    /// <para>Clears a value from the session storage by its <paramref name="key"/>.</para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.Object"/>
    /// </param>
    public override void ClearValue(string key)
    {
      if(key == null)
      {
        throw new ArgumentNullException("key");
      }
      
      try
      {
        this.SyncRoot.EnterWriteLock();
        HttpSessionState backend = this.GetStorageBackend();
        backend.Remove(key);
      }
      finally
      {
        if(this.SyncRoot.IsWriteLockHeld)
        {
          this.SyncRoot.ExitWriteLock();
        }
      }
    }
    
    #endregion
    
    #region access to the session storage
    
    /// <summary>
    /// <para>Gets the ASP.NET session storage backend.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="HttpSessionState"/>
    /// </returns>
    protected HttpSessionState GetStorageBackend()
    {
      if(HttpContext.Current == null)
      {
        throw new InvalidOperationException("There is no current HttpContext.");
      }
      else if(HttpContext.Current.Session == null)
      {
        throw new InvalidOperationException("The current HttpContext does not expose a session state.");
      }
      
      return HttpContext.Current.Session;
    }
    
    #endregion
  }
}

