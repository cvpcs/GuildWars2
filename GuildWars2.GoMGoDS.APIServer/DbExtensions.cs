using System;
using System.Data;

namespace GuildWars2.GoMGoDS.APIServer
{
    public static class DbExtensions
    {
        public static void AddParameter(this IDbCommand command, string key, object value)
        {
            IDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = key;
            parameter.Value = value;

            if (command.Parameters.Contains(key))
                command.Parameters.RemoveAt(key);

            command.Parameters.Add(parameter);
        }
    }
}
