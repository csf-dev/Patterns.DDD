//  
//  Entity.cs
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
using CSF.Patterns.DDD.Data;

namespace CSF.Patterns.DDD.Entities
{
  /// <summary>
  /// <para>
  /// Generic type to store entity references that are serialisable to a persistent data source such as a database.
  /// </para>
  /// </summary>
  /// <typeparam name="T">
  /// Describes the underlying type of the reference to be stored within the current object instance.
  /// </typeparam>
  /// <remarks>
  /// <para>This type supports access to the reference ID within that repository.</para>
  /// </remarks>
  public class Entity<T> : IEntity<T>
  {
    #region fields
    
    private Identity<T>? _identity;
    private UnitOfWork _unitOfWork;
    
    #endregion
    
    #region IEntity[T] implementation

    /// <summary>
    /// <para>Read-only.  Gets whether or not the current instance holds a valid reference or not.</para>
    /// </summary>
    public virtual bool HasIdentity
    {
      get {
        return (_identity.HasValue);
      }
    }
    
    /// <summary>
    /// <para>Gets and sets the unique identifier component of this entity's generic identity.</para>
    /// <para>
    /// Avoid using this property in favour of <see cref="GetIdentity"/> and <see cref="SetIdentity"/>, see the
    /// remarks for more information.
    /// </para>
    /// <seealso cref="GetIdentity"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Ideally, avoid using this property unless absolutely neccesary.  It exists primarily for providing access to
    /// database-mapping layers that need a property with which to reference the unique identifier.
    /// </para>
    /// </remarks>
    public virtual T Id
    {
      get {
        T output = default(T);
        
        if(_identity.HasValue)
        {
          output = _identity.Value.Value;
        }
        
        return output;
      }
      set {
        _identity = this.CreateIdentity(value);
      }
    }
    
    /// <summary>
    /// <para>Gets the reference stored within the current object instance.</para>
    /// </summary>
    /// <returns>
    /// An instance of the underlying reference for the current object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If the underlying reference stored within the current object instance is not valid.
    /// </exception>
    public virtual IIdentity<T> GetIdentity()
    {
      if(!_identity.HasValue)
      {
        throw new InvalidOperationException("This entity instance does not have an identity.");
      }
      
      return _identity.Value;
    }

    /// <summary>
    /// <para>Sets the reference stored within the current object instance.</para>
    /// </summary>
    /// <param name="identityValue">
    /// An instance of the underlying reference type for the current object.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="identityValue"/> is not a valid reference.
    /// </exception>
    public virtual void SetIdentity(T identityValue)
    {
      if(_identity.HasValue)
      {
        throw new InvalidOperationException("The current entity already has an identity.  Identity may not be " +
                                            "changed once set without first calling ClearIdentity().");
      }
      
      _identity = this.CreateIdentity(identityValue);
    }

    /// <summary>
    /// <para>Clears the underlying reference stored within the current instance.</para>
    /// </summary>
    public virtual void ClearIdentity()
    {
      _identity = null;
    }
    
    /// <summary>
    /// <para>
    /// Determines whether or not a given <paramref name="identityValue"/> is valid for storing within the current
    /// object instance.
    /// </para>
    /// </summary>
    /// <param name="identityValue">
    /// An instance of the underlying reference type for the current object.
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    public virtual bool ValidateIdentity(T identityValue)
    {
      return (!identityValue.Equals(default(T)));
    }
    
    /// <summary>
    /// <para>Registers this entity instance with the given <paramref name="UnitOfWork"/>.</para>
    /// </summary>
    /// <param name="unitOfWork">
    /// A <see cref="UnitOfWork"/>
    /// </param>
    public virtual void RegisterUnitOfWork(UnitOfWork unitOfWork)
    {
      if(unitOfWork == null)
      {
        throw new ArgumentNullException("tracker");
      }
      else if(_unitOfWork != null)
      {
        throw new InvalidOperationException("This entity is already registered with a unit of work.");
      }
      
      _unitOfWork = unitOfWork;
      _unitOfWork.Register(this);
    }
    
    /// <summary>
    /// <para>Deletes the association between this instance and a registered <see cref="UnitOfWork"/>.</para>
    /// </summary>
    public virtual void UnRegisterUnitOfWork()
    {
      _unitOfWork = null;
    }
    
    #endregion
    
    #region explicit IEntity implementation
    
    object IEntity.Id {
      get {
        return this.Id;
      }
      set {
        this.Id = (T) value;
      }
    }
    
    IIdentity IEntity.GetIdentity()
    {
      return this.GetIdentity();
    }
    
    void IEntity.SetIdentity(object identityValue)
    {
      this.SetIdentity((T) identityValue);
    }
    
    bool IEntity.ValidateIdentity(object identityValue)
    {
      return this.ValidateIdentity((T) identityValue);
    }
    
    #endregion
    
    #region methods
    
    /// <summary>
    /// <para>
    /// Overridden from <see cref="Object"/>, overloaded.  Determines whether the current instance is equal to the
    /// specified <paramref name="obj"/>.
    /// </para>
    /// </summary>
    /// <param name="obj">
    /// A <see cref="System.Object"/> to compare against.
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/> indicating whether the <paramref name="obj"/> is equal to the current instance.
    /// </returns>
    public override bool Equals(object obj)
    {
      bool output;
      
      if(Object.ReferenceEquals(this, obj))
      {
        output = true;
      }
      else if(obj is IEntity)
      {
        output = this.Equals((IEntity) obj);
      }
      else
      {
        output = false;
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Determines whether the current instance is equal to the specified entity reference.
    /// </para>
    /// </summary>
    /// <param name="compareTo">
    /// An <see cref="IEntity"/> to compare against
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/> indicating whether the entity to <paramref name="compareTo"/> is equal to
    /// the current instance.
    /// </returns>
    public virtual bool Equals(IEntity compareTo)
    {
      bool output;
      
      if(Object.ReferenceEquals(this, compareTo))
      {
        output = true;
      }
      else if((object) compareTo == null)
      {
        output = false;
      }
      else if(!this.HasIdentity || !compareTo.HasIdentity)
      {
        output = false;
      }
      else
      {
        output = ((Identity<T>) this.GetIdentity() == compareTo.GetIdentity());
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Overridden.  Gets a hash code for the current object instance.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="System.Int32"/>
    /// </returns>
    public override int GetHashCode()
    {
      int output;
      
      if(this.HasIdentity)
      {
        output = _identity.GetHashCode();
      }
      else
      {
        output = base.GetHashCode();
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>Creates a human-readable string representation of the current instance.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/>
    /// </returns>
    public override string ToString()
    {
      string output;
      
      if(this.HasIdentity)
      {
        output = this.GetIdentity().ToString();
      }
      else
      {
        output = String.Format("[{0}: no identity]", this.GetIdentityType().FullName);
      }
      
      return output;
    }
    
    /// <summary>
    /// <para>
    /// Factory method creates a new <see cref="IIdentity"/> instance that uniquely identifies the current instance.
    /// </para>
    /// </summary>
    /// <param name="identityValue">
    /// An identity value of the generic type appropriate to this entity
    /// </param>
    /// <returns>
    /// A generic identity instance
    /// </returns>
    protected virtual Identity<T> CreateIdentity(T identityValue)
    {
      if(!this.ValidateIdentity(identityValue))
      {
        throw new ArgumentException("Invalid identity value");
      }
      
      return new Identity<T>(this.GetIdentityType(), identityValue);
    }
    
    /// <summary>
    /// <para>Gets the <see cref="System.Type"/> that this entity instance uses to identify itself.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="Type"/>
    /// </returns>
    protected virtual Type GetIdentityType()
    {
      return this.GetType();
    }
    
    /// <summary>
    /// <para>Overloaded.  Gets a <see cref="IRepositoryConnection"/> to the default repository.</para>
    /// </summary>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    protected IRepositoryConnection GetRepositoryConnection()
    {
      return this.GetRepositoryConnection(null);
    }
    
    /// <summary>
    /// <para>Overloaded.  Gets a <see cref="IRepositoryConnection"/> to the named repository.</para>
    /// </summary>
    /// <param name="repositoryIdentifier">
    /// A <see cref="System.String"/>
    /// </param>
    /// <returns>
    /// A <see cref="IRepositoryConnection"/>
    /// </returns>
    protected IRepositoryConnection GetRepositoryConnection(object repositoryIdentifier)
    {
      IRepositoryFactory factory = RepositoryFactories.Factory(repositoryIdentifier);
      
      return factory.GetConnection();
    }
    
    #endregion
    
    #region repository manipulation methods
    
    /// <summary>
    /// <para>
    /// Overloaded.  Deletes this entity within the default repository, or if this entity is registered with a
    /// <see cref="UnitOfWork"/>, marks this entity for deletion.
    /// </para>
    /// </summary>
    public virtual void Delete()
    {
      if(_unitOfWork != null)
      {
        this.OnDelete();
      }
      else
      {
        IRepositoryConnection connection = RepositoryFactories.Default.GetConnection();
        this.Delete(connection);
      }
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Deletes this entity within the repository exposed by the given <paramref name="connection"/>.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    public virtual void Delete(IRepositoryConnection connection)
    {
      if(connection == null)
      {
        throw new ArgumentNullException("connection");
      }
      
      IRepository repository = connection.GetRepository(this.GetType());
      repository.Delete(this);
      
      this.OnDelete();
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Updates this entity within the default repository, or if this entity is registered with a
    /// <see cref="UnitOfWork"/>, marks this entity for update.
    /// </para>
    /// </summary>
    public virtual void Update()
    {
      if(_unitOfWork != null)
      {
        this.OnDirty();
      }
      else
      {
        IRepositoryConnection connection = RepositoryFactories.Default.GetConnection();
        this.Update(connection);
      }
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Updates this entity within the repository exposed by the given <paramref name="connection"/>.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    public virtual void Update(IRepositoryConnection connection)
    {
      if(connection == null)
      {
        throw new ArgumentNullException("connection");
      }
      
      IRepository repository = connection.GetRepository(this.GetType());
      repository.Update(this);
      
      this.OnDirty();
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Creates this entity within the default repository, or if this entity is registered with a
    /// <see cref="UnitOfWork"/>, marks this entity for creation.
    /// </para>
    /// </summary>
    public virtual void Create()
    {
      if(_unitOfWork != null)
      {
        this.OnCreated();
      }
      else
      {
        IRepositoryConnection connection = RepositoryFactories.Default.GetConnection();
        this.Create(connection);
      }
    }
    
    /// <summary>
    /// <para>
    /// Overloaded.  Creates this entity within the repository exposed by the given <paramref name="connection"/>.
    /// </para>
    /// </summary>
    /// <param name="connection">
    /// A <see cref="IRepositoryConnection"/>
    /// </param>
    public virtual void Create(IRepositoryConnection connection)
    {
      if(connection == null)
      {
        throw new ArgumentNullException("connection");
      }
      
      IRepository repository = connection.GetRepository(this.GetType());
      repository.Create(this);
      
      this.OnCreated();
    }
    
    #endregion
    
    #region events and invokers
    
    /// <summary>
    /// <para>
    /// This event is invoked when the current instance becomes dirty (requires update in a presistent data store).
    /// </para>
    /// </summary>
    public virtual event EventHandler Dirty;
    
    /// <summary>
    /// <para>This event is invoked when the current instance is to deleted in a persistent data store.</para>
    /// </summary>
    public virtual event EventHandler Deleted;
    
    /// <summary>
    /// <para>
    /// This event is invoked when the current instance is created (stored within the repository for the first time).
    /// </para>
    /// </summary>
    public virtual event EventHandler Created;
    
    /// <summary>
    /// <para>Event invoker for when this instance becomes dirty.</para>
    /// </summary>
    protected virtual void OnDirty()
    {
      EventArgs ev = new EventArgs();
      
      if(this.Dirty != null)
      {
        this.Dirty(this, ev);
      }
    }
    
    /// <summary>
    /// <para>Event invoker for when this instance is to be deleted.</para>
    /// </summary>
    protected virtual void OnDelete()
    {
      EventArgs ev = new EventArgs();
      
      if(this.Deleted != null)
      {
        this.Deleted(this, ev);
      }
    }
    
    /// <summary>
    /// <para>
    /// Event invoker for when this instance is to be created (stored in the repository for the first time).
    /// </para>
    /// </summary>
    protected virtual void OnCreated()
    {
      EventArgs ev = new EventArgs();
      
      if(this.Created != null)
      {
        this.Created(this, ev);
      }
    }
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// <para>Initialises this instance with an empty/default reference.</para>
    /// </summary>
    public Entity() : this(default(T), null) {}
    
    /// <summary>
    /// <para>Initialises this instance with a given <paramref name="initialReference"/>.</para>
    /// </summary>
    /// <remarks>
    /// <para>This constructor does not check whether the <paramref name="initialReference"/> is valid or not.</para>
    /// </remarks>
    /// <param name="initialReference">
    /// An instance of the underlying reference type for the current object.
    /// </param>
    public Entity(T initialReference) : this(initialReference, null) {}
    
    /// <summary>
    /// <para>
    /// Initialises this instance with an empty/default reference and registers it with an <see cref="UnitOfWork"/>.
    /// </para>
    /// </summary>
    /// <param name="tracker">
    /// A <see cref="UnitOfWork"/>
    /// </param>
    public Entity(UnitOfWork tracker) : this(default(T), tracker) {}
    
    /// <summary>
    /// <para>
    /// Initialises this instance with a given <paramref name="initialReference"/> and registers it with an
    /// <see cref="UnitOfWork"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>This constructor does not check whether the <paramref name="initialReference"/> is valid or not.</para>
    /// </remarks>
    /// <param name="initialReference">
    /// An instance of the underlying reference type for the current object.
    /// </param>
    /// <param name="tracker">
    /// A <see cref="UnitOfWork"/>
    /// </param>
    public Entity(T initialReference, UnitOfWork tracker)
    {
      if(this.ValidateIdentity(initialReference))
      {
        _identity = this.CreateIdentity(initialReference);
      }
      else
      {
        _identity = null;
      }
      
      if(tracker != null)
      {
        this.RegisterUnitOfWork(tracker);
      }
    }
    
    #endregion
    
    #region static operator overloads
    
    /// <summary>
    /// <para>Performs equality testing between two entity instances.</para>
    /// </summary>
    /// <remarks>
    /// <para>This equality test does not require type equality between the objects to be compared.</para>
    /// </remarks>
    /// <param name="entity">
    /// An entity instance.
    /// </param>
    /// <param name="obj">
    /// An <see cref="IEntity"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    public static bool operator ==(Entity<T> entity, IEntity obj)
    {
      bool output;
      
      if((object) entity == null)
      {
        output = (obj == null);
      }
      else
      {
        output = entity.Equals(obj);
      }
      
      return output;
    }

    /// <summary>
    /// <para>Performs inequality testing between two entity instances.</para>
    /// </summary>
    /// <remarks>
    /// <para>This equality test does not require type equality between the objects to be compared.</para>
    /// </remarks>
    /// <param name="entity">
    /// An entity instance.
    /// </param>
    /// <param name="obj">
    /// An <see cref="IEntity"/>
    /// </param>
    /// <returns>
    /// A <see cref="System.Boolean"/>
    /// </returns>
    public static bool operator !=(Entity<T> entity, IEntity obj)
    {
      return !(entity == obj);
    }
    
    #endregion
  }
}

