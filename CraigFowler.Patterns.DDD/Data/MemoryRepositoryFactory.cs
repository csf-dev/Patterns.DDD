//  
//  MemoryRepositoryFactory.cs
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

namespace CraigFowler.Patterns.DDD.Data
{
  /// <summary>
  /// <para>
  /// Factory for a simple in-memory repository factory.  Storage persistance is limited to the lifetime of the process.
  /// </para>
  /// </summary>
  public class MemoryRepositoryFactory : RepositoryFactoryBase
  {
    #region fields
    
    private object _syncRoot;
    private Dictionary<Type, IRepository> _repositories;
    
    #endregion
    
    #region implemented abstract members of CraigFowler.Patterns.DDD.Data.RepositoryFactoryBase
    
    /// <summary>
    /// <para>Gets a new <see cref="MemoryRepositoryConnection"/> instance.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    public override IRepositoryConnection GetConnection ()
    {
      return new MemoryRepositoryConnection(this);
    }

    /// <summary>
    /// <para>Gets an <see cref="IRepository"/> suitable for the generic entity type.</para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    /// <returns>
    /// An <see cref="IRepository"/>
    /// </returns>
    public override IRepository<TEntity> GetRepository<TEntity> (IRepositoryConnection connection)
    {
      Type repositoryType = typeof(TEntity);
      IRepository<TEntity> output;
      
      lock(_syncRoot)
      {
        if(_repositories.ContainsKey(repositoryType))
        {
          output = (IRepository<TEntity>) _repositories[repositoryType];
        }
        else
        {
          output = new MemoryRepository<TEntity>(connection);
          _repositories.Add(repositoryType, output);
        }
      }
      
      return output;
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Clears all data in the current instance and re-initialises in an empty state.</para>
    /// </summary>
    public void Clear()
    {
      lock(_syncRoot)
      {
        _repositories = new Dictionary<Type, IRepository>();
      }
    }
    
    #endregion
    
    #region constructor
    
    /// <summary>
    /// <para>Initialises the current instance.</para>
    /// </summary>
    public MemoryRepositoryFactory ()
    {
      _repositories = new Dictionary<Type, IRepository>();
      _syncRoot = new object();
    }
    
    #endregion
  }
}

