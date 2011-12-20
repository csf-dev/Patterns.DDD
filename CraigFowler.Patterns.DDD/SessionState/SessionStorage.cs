//  
//  SessionStorage.cs
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
using System.Threading;
using System.Configuration;

namespace CraigFowler.Patterns.DDD.SessionState
{
  /// <summary>
  /// <para>Base class for a session storage back-end.</para>
  /// </summary>
  public abstract class SessionStorage
  {
    #region fields
    
    /// <summary>
    /// <para>A <see cref="ReaderWriterLockSlim"/> for controlling synchronised access to this instance.</para>
    /// </summary>
    protected ReaderWriterLockSlim SyncRoot = new ReaderWriterLockSlim();
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Gets a value from the session storage by its <paramref name="key"/>.</para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.String"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Object"/>
    /// </returns>
    public abstract object GetValue(string key);
    
    /// <summary>
    /// <para>Stores a value in the session storage by its <paramref name="key"/>.</para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.String"/>
    /// </param>
    /// <param name="value">
    /// A <see cref="System.Object"/>
    /// </param>
    public abstract void StoreValue(string key, object value);
    
    /// <summary>
    /// <para>Clears a value from the session storage by its <paramref name="key"/>.</para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.String"/>
    /// </param>
    public abstract void ClearValue(string key);
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises the singleton instance for session storage.</para>
    /// </summary>
    static SessionStorage ()
    {
      SessionStorageConfiguration config;
      
      config = (SessionStorageConfiguration) ConfigurationManager.GetSection(SessionStorageConfiguration.LogicalPath);
      Current = config.GetSessionStorage();
    }
    
    #endregion
    
    #region static properties
    
    /// <summary>
    /// <para>Read-only.  Gets a singleton instance of a session storage backend.</para>
    /// </summary>
    public static SessionStorage Current
    {
      get;
      private set;
    }
    
    #endregion
  }
}

