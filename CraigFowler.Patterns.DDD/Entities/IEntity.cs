//  
//  IEntity.cs
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
using CraigFowler.Patterns.DDD.Data;

namespace CraigFowler.Patterns.DDD.Entities
{
  /// <summary>
  /// <para>Base non-generic interface for domain entities.</para>
  /// </summary>
  public interface IEntity
  {
    #region properties
    
    /// <summary>
    /// <para>Read-only.  Gets whether or not the current instance holds a valid reference or not.</para>
    /// </summary>
    bool HasIdentity { get; }
    
    /// <summary>
    /// <para>Gets and sets the unique identifier component of this entity's <see cref="IIdentity"/>.</para>
    /// <seealso cref="IIdentity.Value"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Ideally, avoid using this property unless absolutely neccesary.  It exists primarily for providing access to
    /// database-mapping layers that need a property with which to reference the unique identifier.
    /// </para>
    /// </remarks>
    object Id { get; set; }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>Gets the identity of the current instance.</para>
    /// </summary>
    /// <returns>
    /// An <see cref="IIdentity"/> instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If the current instance does not yet have an identity.  <see cref="HasIdentity"/>.
    /// </exception>
    IIdentity GetIdentity();
    
    /// <summary>
    /// <para>Sets the reference stored within the current object instance.</para>
    /// </summary>
    /// <param name="identityValue">
    /// An instance of the underlying reference type for the current object.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="identityValue"/> is not a valid reference.
    /// </exception>
    void SetIdentity(object identityValue);
    
    /// <summary>
    /// <para>
    /// Determines whether or not a given <paramref name="identityValue"/> is valid for storing within the current
    /// object instance.
    /// </para>
    /// </summary>
    /// <param name="identityValue">
    /// An instance of the underlying identity type for the current object.
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    bool ValidateIdentity(object identityValue);
    
    /// <summary>
    /// <para>Clears the identity stored within the current instance.</para>
    /// </summary>
    void ClearIdentity();
    
    /// <summary>
    /// <para>Registers this entity instance with a given <see cref="UnitOfWork"/></para>
    /// </summary>
    /// <param name="unitOfWork">
    /// A <see cref="UnitOfWork"/>
    /// </param>
    void RegisterUnitOfWork(UnitOfWork unitOfWork);
    
    /// <summary>
    /// <para>Deletes the association between this instance and a registered <see cref="UnitOfWork"/>.</para>
    /// </summary>
    void UnRegisterUnitOfWork();
    
    /// <summary>
    /// <para>
    /// Overloaded.  Deletes this entity within the default repository, or if this entity is registered with a
    /// <see cref="UnitOfWork"/>, marks this entity for deletion.
    /// </para>
    /// </summary>
    void Delete();
    
    /// <summary>
    /// <para>
    /// Overloaded.  Deletes this entity within the repository exposed by the given <paramref name="connection"/>.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    void Delete(IRepositoryConnection connection);
    
    /// <summary>
    /// <para>
    /// Overloaded.  Updates this entity within the default repository, or if this entity is registered with a
    /// <see cref="UnitOfWork"/>, marks this entity for update.
    /// </para>
    /// </summary>
    void Update();
    
    /// <summary>
    /// <para>
    /// Overloaded.  Updates this entity within the repository exposed by the given <paramref name="connection"/>.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    void Update(IRepositoryConnection connection);
    
    /// <summary>
    /// <para>
    /// Overloaded.  Creates this entity within the default repository, or if this entity is registered with a
    /// <see cref="UnitOfWork"/>, marks this entity for creation.
    /// </para>
    /// </summary>
    void Create();
    
    /// <summary>
    /// <para>
    /// Overloaded.  Creates this entity within the repository exposed by the given <paramref name="connection"/>.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    void Create(IRepositoryConnection connection);
    
    #endregion
    
    #region events
    
    /// <summary>
    /// <para>
    /// This event is invoked when the current instance becomes dirty (requires update in a presistent data store).
    /// </para>
    /// </summary>
    event EventHandler Dirty;
    
    /// <summary>
    /// <para>This event is invoked when the current instance is to deleted in a persistent data store.</para>
    /// </summary>
    event EventHandler Deleted;
    
    /// <summary>
    /// <para>
    /// This event is invoked when the current instance is created (stored within the repository for the first time).
    /// </para>
    /// </summary>
    event EventHandler Created;
    
    #endregion
  }
}

