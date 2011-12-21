//  
//  MemorySessionStorage.cs
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
using System.Collections.Generic;
using System.Threading;

namespace CraigFowler.Patterns.DDD.SessionState
{
  /// <summary>
  /// <para>
  /// Provides an implementation of <see cref="SessionStorage"/> that uses a simple in-memory collection of keys and
  /// values.
  /// </para>
  /// </summary>
  public class MemorySessionStorage : SessionStorage
  {
    #region properties
    
    [ThreadStatic]
    private static Dictionary<string, object> _storage = new Dictionary<string, object>();
    
    #endregion

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
        output = _storage.ContainsKey(key)? _storage[key] : null;
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
        _storage[key] = value;
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
        _storage.Remove(key);
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
    /// <para>Abandons the current session in the backend and removes all associations with the current session.</para>
    /// </summary>
    public override void Abandon()
    {
      try
      {
        this.SyncRoot.EnterWriteLock();
        _storage = new Dictionary<string, object>();
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
  }
}
