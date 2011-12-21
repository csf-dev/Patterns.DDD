//  
//  TestRepositoryFactories.cs
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

namespace Test.CraigFowler.Patterns.DDD.Data
{
  [TestFixture]
  [Category("Requires configuration")]
  [Description("The tests in this fixture depend upon a working repository factory configuration containing " +
               "compatible settings.")]
  public class TestRepositoryFactories
  {
    #region set up
    
    [SetUp]
    public void Setup()
    {
#pragma warning disable 219
      /* Intentionally disable CS219 - we are touching this type in the test setup so that its assembly is definitely
       * loaded into the current AppDomain.  Otherwise the code that we are testing might not be able to find this
       * type.
       */
      Type typeLoader = typeof(global::CraigFowler.Patterns.DDD.Mocks.Data.DummyRepositoryFactory);
#pragma warning restore 219
    }
    
    #endregion
    
    #region tests
    
    [Test]
    public void TestDefault()
    {
      IRepositoryFactory factory;
      
      Assert.IsNotNull(RepositoryFactories.ConfiguredFactories.Default, "Default factory is not null");
      Assert.IsInstanceOfType(typeof(IRepositoryFactory),
                              RepositoryFactories.ConfiguredFactories.Default,
                              "Correct type");
      
      factory = RepositoryFactories.ConfiguredFactories.Default;
      
      Assert.AreEqual("Server=localhost;User=root",
                      ((DatabaseRepositoryFactory) factory).ConnectionString,
                      "Correct connection string");
    }
    
    #endregion
  }
}

