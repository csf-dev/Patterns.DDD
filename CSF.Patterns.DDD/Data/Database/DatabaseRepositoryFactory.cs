//  
//  DatabaseRepositoryFactory.cs
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

namespace CSF.Patterns.DDD.Data.Database
{
  /// <summary>
  /// <para>Base class for repository factories that work from a database.</para>
  /// </summary>
  public abstract class DatabaseRepositoryFactory : RepositoryFactoryBase
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets the connection string that will be used to connect to the database provider.</para>
    /// </summary>
    public virtual string ConnectionString
    {
      get;
      set;
    }
    
    /// <summary>
    /// <para>
    /// Read-only.  Gets the namespace where a <see cref="Type"/> implementing <see cref="System.Data.IDbConnection"/>
    /// (that will act as the database backend) is located.
    /// </para>
    /// </summary>
    public virtual string ProviderNamespace
    {
      get;
      set;
    }
    
    #endregion
    
    #region IRepositoryFactory implementation
    
    /// <summary>
    /// <para>Creates a new repository connection.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    public override IRepositoryConnection GetConnection ()
    {
      return new DatabaseRepositoryConnection(this.ConnectionString, this.ProviderNamespace, this);
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises this instance.</para>
    /// </summary>
    public DatabaseRepositoryFactory () : base() {}
    
    #endregion
  }
}

