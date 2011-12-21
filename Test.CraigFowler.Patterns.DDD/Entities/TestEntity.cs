//  
//  TestEntity.cs
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
using CraigFowler.Patterns.DDD.Entities;
using NUnit.Framework;
using CraigFowler.Patterns.DDD.Mocks.Entities;

namespace Test.CraigFowler.Patterns.DDD.Entities
{
  [TestFixture]
  public class TestEntity
  {
    #region tests
    
    [Test]
    public void TestHasIdentity()
    {
      Entity<uint> entity = new Entity<uint>();
      
      Assert.IsFalse(entity.HasIdentity, "Entity has no identity");
      
      entity.SetIdentity(5);
      Assert.IsTrue(entity.HasIdentity, "Entity has an identity");
    }
    
    [Test]
    public void TestGetIdentity()
    {
      Person entity = new Person(5);
      IIdentity identity = entity.GetIdentity();
      Assert.AreEqual("[CraigFowler.Patterns.DDD.Mocks.Entities.Person: 5]", identity.ToString(), "Correct identity");
    }
    
    [Test]
    public void TestSetIdentity()
    {
      Person entity = new Person();
      IIdentity identity;
      
      entity.SetIdentity(5);
      identity = entity.GetIdentity();
      Assert.AreEqual("[CraigFowler.Patterns.DDD.Mocks.Entities.Person: 5]", identity.ToString(), "Correct identity");
    }
    
    [Test]
    [ExpectedException(ExceptionType = typeof(ArgumentException), ExpectedMessage = "Invalid identity value")]
    public void TestSetIdentityInvalid()
    {
      Entity<uint> entity = new Entity<uint>();
      entity.SetIdentity(0);
      Assert.Fail("Test should never reach this point");
    }
    
    [Test]
    public void TestClearIdentity()
    {
      Person entity = new Person(5);
      
      Assert.IsTrue(entity.HasIdentity, "Has identity");
      entity.ClearIdentity();
      Assert.IsFalse(entity.HasIdentity, "Identity removed");
    }
    
    [Test]
    public void TestValidateIdentity()
    {
      Person entity = new Person();
      Assert.IsTrue(entity.ValidateIdentity(5), "Valid identity");
      Assert.IsFalse(entity.ValidateIdentity(0), "Invalid identity");
    }
    
    [Test]
    public void TestEquals()
    {
      string stringTest = "foo bar";
      uint numericTest = 3; 
      Person three = new Person(3);
      Person four = new Person(4);
      Person threeAgain = new Person(3);
      Product threeProduct = new Product(3);
      
      Assert.IsFalse(three.Equals(stringTest), "Entity does not equal a string");
      Assert.IsFalse(three.Equals(numericTest), "Entity does not equal a uint");
      Assert.IsFalse(three.Equals(four), "Non-matching identities not equal");
      
      Assert.IsTrue(three.Equals(three), "Copies of the same object are equal");
      Assert.IsTrue(three.Equals(threeAgain), "Identical identities are equal");
      
      Assert.IsFalse(three.Equals(threeProduct), "Non-matching types not equal");
    }
    
    [Test]
    public void TestToString()
    {
      Person entity = new Person(5);
      
      Assert.AreEqual("[CraigFowler.Patterns.DDD.Mocks.Entities.Person: 5]", entity.ToString(), "Correct identity");
      
      entity.ClearIdentity();
      Assert.AreEqual("[CraigFowler.Patterns.DDD.Mocks.Entities.Person: no identity]",
                      entity.ToString(),
                      "Cleared identity");
    }
    
    [Test]
    public void TestOperatorEquality()
    {
      Person three = new Person(3);
      Person four = new Person(4);
      Person threeAgain = new Person(3);
      Product threeProduct = new Product(3);
      
      Assert.IsFalse(three == four, "Non-matching identities not equal");
#pragma warning disable 1718
      // Disabling CS1718 - the point of this test is to compare the object to itself
      Assert.IsTrue(three == three, "Copies of the same object are equal");
#pragma warning restore 1718
      Assert.IsTrue(three == threeAgain, "Identical instances are equal");
      Assert.IsFalse(three == threeProduct, "Non-matching types not equal");
    }
    
    [Test]
    public void TestOperatorInequality()
    {
      Person three = new Person(3);
      Person four = new Person(4);
      Person threeAgain = new Person(3);
      Product threeProduct = new Product(3);
      
      Assert.IsTrue(three != four, "Non-matching identities not equal");
#pragma warning disable 1718
      // Disabling CS1718 - the point of this test is to compare the object to itself
      Assert.IsFalse(three != three, "Copies of the same object are equal");
#pragma warning restore 1718
      Assert.IsFalse(three != threeAgain, "Identical instances are equal");
      Assert.IsTrue(three != threeProduct, "Non-matching types not equal");
    }
    
    #endregion
  }
}

