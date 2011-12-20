//  
//  SessionStorageConfiguration.cs
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
using System.Configuration;
using System.Reflection;

namespace CraigFowler.Patterns.DDD.SessionState
{
  /// <summary>
  /// <para>
  /// Base class for a <see cref="ConfigurationSection"/> that describes how <see cref="SessionStorage.Current"/>
  /// should be populated.
  /// </para>
  /// </summary>
  public class SessionStorageConfiguration : ConfigurationSection
  {
    #region constants
    
    /// <summary>
    /// <para>
    /// Read-only constant gets the default storage backend if no storage backend is specified in
    /// <see cref="TypeName"/>.
    /// </para>
    /// </summary>
    public static readonly Type DefaultStorageType = typeof(MemorySessionStorage);
    
    /// <summary>
    /// <para>
    /// Read-only constant gets the path to where this configration should be found within the application configuration.
    /// </para>
    /// </summary>
    public static readonly string LogicalPath = "CraigFowlerPatterns/SessionState";
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Gets and sets the fully-qualified type name to the class that provides the session storage backend.</para>
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="Type"/> described by this property must implement <see cref="SessionStorage"/>.</para>
    /// </remarks>
    [ConfigurationProperty("TypeName", DefaultValue = "", IsRequired = false)]
    public string TypeName
    {
      get;
      set;
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>
    /// Factory method for an instance of a type that implements <see cref="SessionStorage"/>.
    /// </para>
    /// </summary>
    /// <returns>
    /// A <see cref="SessionStorage"/>
    /// </returns>
    public virtual SessionStorage GetSessionStorage()
    {
      Type storageType = String.IsNullOrEmpty(this.TypeName)? DefaultStorageType : Type.GetType(this.TypeName, true);
      ConstructorInfo constructor = storageType.GetConstructor(new Type[0]);
      return (SessionStorage) constructor.Invoke(null);
    }
    
    #endregion
  }
}

