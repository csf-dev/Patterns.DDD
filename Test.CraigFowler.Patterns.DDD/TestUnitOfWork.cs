using CraigFowler.Patterns.DDD.Entities;
using NUnit.Framework;
using CraigFowler.Patterns.DDD.Data;

namespace Test.CraigFowler.Patterns.DDD.Data
{
  [TestFixture]
  public class TestUnitOfWork
  {
    [Test]
    public void TestRegisterAndUnregister()
    {
      using(UnitOfWork tracker = new UnitOfWork(new MockRepositoryConnection()))
      {
        Entity<int> entity = new Entity<int>();
        
        entity.RegisterUnitOfWork(tracker);
      }
    }
  }
}

