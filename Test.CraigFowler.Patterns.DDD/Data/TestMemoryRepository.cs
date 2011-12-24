//  
//  TestMemoryRepository.cs
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
using NUnit.Framework;
using CraigFowler.Patterns.DDD.Data;
using CraigFowler.Patterns.DDD.Mocks.Entities;
using CraigFowler.Patterns.DDD.Entities;

namespace Test.CraigFowler.Patterns.DDD.Data
{
  [TestFixture]
  public class TestMemoryRepository
  {
    #region properties
    
    public MemoryRepositoryFactory Factory
    {
      get;
      set;
    }
    
    #endregion
    
    #region setup
    
    [SetUp]
    public void Setup()
    {
      this.Factory = new MemoryRepositoryFactory();
    }
    
    #endregion
    
    #region tests
    
    [Test]
    public void TestCreate()
    {
      uint id;
      
      using (IRepositoryConnection connection = this.Factory.GetConnection())
      {
        IRepository<Person> repository = connection.GetRepository<Person>();
        
        Person person = new Person() {Name = "John", TelephoneNumber = "12345"};
        Assert.IsFalse(person.HasIdentity, "Person does not yet have an identity");
        
        repository.Create(person);
        Assert.IsTrue(person.HasIdentity, "Person now has an identity");
        
        id = person.GetIdentity().Value;
      }
      
      using (IRepositoryConnection connection = this.Factory.GetConnection())
      {
        IRepository<Person> repository = connection.GetRepository<Person>();
        Person person = repository.Read(new Identity<uint>(typeof(Person), id));
        Assert.AreEqual("John", person.Name, "Correct name");
      }
    }
    
    [Test]
    [ExpectedException(ExceptionType = typeof(ArgumentException),
                       ExpectedMessage = "Entity already has an identity, cannot create.\nParameter name: entity")]
    public void TestCreateWithIdentity()
    {
      using (IRepositoryConnection connection = this.Factory.GetConnection())
      {
        IRepository<Person> repository = connection.GetRepository<Person>();
        
        Person person = new Person() {Name = "John", TelephoneNumber = "12345"};
        Assert.IsFalse(person.HasIdentity, "Person does not yet have an identity");
        person.SetIdentity(1);
        
        repository.Create(person);
        Assert.Fail("Test should not reach this point");
      }
    }
    
    #endregion
  }
}

