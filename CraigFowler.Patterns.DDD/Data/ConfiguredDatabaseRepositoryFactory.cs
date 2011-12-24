//  
//  ConfiguredDatabaseRepositoryFactory.cs
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

namespace CraigFowler.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Repository factory type that takes its connection details from a configured connection string in the
  /// application configuration.
  /// </para>
  /// </summary>
  public abstract class ConfiguredDatabaseRepositoryFactory : DatabaseRepositoryFactory
  {
    #region fields
    
    private string _connectionStringName;
    
    #endregion
    
    #region properties
    
    /// <summary>
    /// <para>Gets and sets the name of the connection string (in the application configuration) to use.</para>
    /// </summary>
    public virtual string ConnectionStringName
    {
      get {
        return _connectionStringName;
      }
      set {
        _connectionStringName = value;
        
        ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
        
        if(settings != null)
        {
          this.ConnectionString = settings.ConnectionString;
          this.ProviderNamespace = settings.ProviderName;
        }
        else
        {
          this.ConnectionString = null;
          this.ProviderNamespace = null;
        }
      }
    }
    
    #endregion
  }
}

