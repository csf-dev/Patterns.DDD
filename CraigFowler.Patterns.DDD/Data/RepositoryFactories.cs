//  
//  RepositoryFactories.cs
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
using System.Collections;
using System.Configuration;

namespace CraigFowler.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Collection type for storing <see cref="IRepositoryFactory"/> instances, indexed by <see cref="System.String"/>.
  /// </para>
  /// </summary>
  public static class RepositoryFactories
  {
    #region constants
    
    /// <summary>
    /// <para>Read-only constant.  Gets the name of the default repository factory.</para>
    /// <seealso cref="Default"/>
    /// </summary>
    public static readonly string DefaultFactoryName = "Default";
    
    #endregion
    
    #region fields
    
    private static Dictionary<string, IRepositoryFactory> _factories;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the default <see cref="IRepositoryFactory"/>.</para>
    /// </summary>
    public static IRepositoryFactory Default
    {
      get {
        IRepositoryFactory output;
        
        try
        {
          output = Factory(DefaultFactoryName);
        }
        catch(IndexOutOfRangeException)
        {
          output = null;
        }
        
        return output;
      }
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>
    /// Overloaded.  Gets the <see cref="IRepositoryFactory"/> at the given <paramref name="key"/>.
    /// </para>
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.String"/>
    /// </param>
    public static IRepositoryFactory Factory(string key)
    {
      if(String.IsNullOrEmpty(key))
      {
        throw new ArgumentException("Repository name may not be null or an empty string.", "key");
      }
      else if(!_factories.ContainsKey(key))
      {
        throw new IndexOutOfRangeException(String.Format("No repository factory was found with the name '{0}'", key));
      }
      
      return _factories[key];
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Gets the <see cref="IRepositoryFactory"/> at the given <paramref name="key"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This indexer is intended for use with an enumerated type, the <paramref name="key"/> is converted to a
    /// <see cref="System.String"/> using its <see cref="Object.ToString"/> method before being passed to the other
    /// 'overload' of this indexer that takes a string parameter.  In short it provides a way of accessing the indexer
    /// using a type-safe manner.
    /// </para>
    /// </remarks>
    /// <param name="key">
    /// A <see cref="System.Object"/>
    /// </param>
    public static IRepositoryFactory Factory(object key)
    {
      return Factory(key.ToString());
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>Initialises the singleton instance of this type.</para>
    /// </summary>
    static RepositoryFactories()
    {
      RepositoryFactoryConfiguration config;
      
      config = (RepositoryFactoryConfiguration) ConfigurationManager.GetSection(RepositoryFactoryConfiguration.LogicalPath);
      
      _factories =  config.GetAllFactories();
    }
    
    #endregion
  }
}

