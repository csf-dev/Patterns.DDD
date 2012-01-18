//  
//  TestUnitOfWork.cs
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
using NUnit.Framework;
using CSF.Patterns.DDD.Mocks.Data;
using CSF.Patterns.DDD.Data;
using CSF.Patterns.DDD.Mocks.Entities;

namespace Test.CSF.Patterns.DDD.Data
{
  [TestFixture]
  public class TestUnitOfWork
  {
    #region unit tests
    
    [Test]
    public void TestConstructor()
    {
      using(DummyRepositoryConnection connection = new DummyRepositoryConnection())
      {
        UnitOfWork work = new UnitOfWork(connection);
        work.PerformWork();
      }
    }
    
    [Test]
    public void TestRegister()
    {
      using(DummyRepositoryConnection connection = new DummyRepositoryConnection())
      {
        UnitOfWork work = new UnitOfWork(connection);
        
        Person person = new Person();
        work.Register(person);
        work.PerformWork();
        
        Assert.AreEqual("Create entity '[CSF.Patterns.DDD.Mocks.Entities.Person: no identity]'\n",
                        connection.Log.ToString(),
                        "Correct log message");
      }
    }
    
    #endregion
    
    #region integration tests
    
    [Test]
    [Category("Integration")]
    public void TestPerformWork()
    {
      using(DummyRepositoryConnection connection = new DummyRepositoryConnection())
      {
        UnitOfWork work = new UnitOfWork(connection);
        
        Person personOne = new Person();
        personOne.RegisterUnitOfWork(work);
        personOne.Name = "Joe";
        personOne.TelephoneNumber = "98765";
        
        Person personTwo = new Person(5);
        personTwo.RegisterUnitOfWork(work);
        personTwo.Name = "Bob";
        personTwo.TelephoneNumber = "12345";
        
        work.PerformWork();
        
        Assert.AreEqual("Create entity '[CSF.Patterns.DDD.Mocks.Entities.Person: no identity]'\n" +
                        "Update entity '[CSF.Patterns.DDD.Mocks.Entities.Person: 5]'\n",
                        connection.Log.ToString(),
                        "Correct log message");
      }
    }
    
    #endregion
  }
}

