﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Marr.Data.QGen
{
    internal class QueryFactory
    {
        public static IQuery CreateUpdateQuery(Mapping.ColumnMapCollection columns, DbParameterCollection parameters, string schema, string target)
        {
            if (parameters.Count == 0)
                throw new Exception("Must contain at least one parameter.");

            string paramType = parameters[0].GetType().Name.ToLower();
            if (paramType.Contains("sqlparameter"))
            {
                return new SqlServerUpdateQuery(columns, parameters, schema, target);
            }
            else
            {
                throw new NotImplementedException("An IQuery class has not yet been implemented for this database provider.");
            }
        }

        public static IQuery CreateInsertQuery(Mapping.ColumnMapCollection columns, DbParameterCollection parameters, string schema, string target)
        {
            if (parameters.Count == 0)
                throw new Exception("Must contain at least one parameter.");

            string paramType = parameters[0].GetType().Name.ToLower();
            if (paramType.Contains("sqlparameter"))
            {
                return new SqlServerInsertQuery(columns, parameters, schema, target);
            }
            else
            {
                throw new NotImplementedException("An IQuery class has not yet been implemented for this database provider.");
            }
        }

        public static IQuery CreateDeleteQuery(Mapping.ColumnMapCollection columns, DbParameterCollection parameters, string schema, string target)
        {
            if (parameters.Count == 0)
                throw new Exception("Must contain at least one parameter.");

            string paramType = parameters[0].GetType().Name.ToLower();
            if (paramType.Contains("sqlparameter"))
            {
                return new SqlServerDeleteQuery(columns, parameters, schema, target);
            }
            else
            {
                throw new NotImplementedException("An IQuery class has not yet been implemented for this database provider.");
            }
        }

        public static IQuery CreateSelectQuery(Mapping.ColumnMapCollection columns, DbParameterCollection parameters, string schema, string target, string where)
        {
            if (parameters.Count == 0)
                throw new Exception("Must contain at least one parameter.");

            string paramType = parameters[0].GetType().Name.ToLower();
            if (paramType.Contains("sqlparameter"))
            {
                return new SqlServerSelectQuery(columns, parameters, schema, target, where);
            }
            else
            {
                throw new NotImplementedException("An IQuery class has not yet been implemented for this database provider.");
            }
        }
    }
}
