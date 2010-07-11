
using System;
using System.Data;

namespace CraigFowler.Patterns.DDD
{
  public interface IRepositoryConfiguration
  {
    IDbConnection GetConnection();
  }
}
