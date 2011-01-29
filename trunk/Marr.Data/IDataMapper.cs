﻿/*  Copyright (C) 2008 - 2010 Jordan Marr

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>. */

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Marr.Data.Parameters;

namespace Marr.Data
{
    public interface IDataMapper : IDisposable
    {
        ParameterChainMethods AddParameter(string name, object value);
        IDbDataParameter AddParameter(IDbDataParameter parameter);
        void BeginTransaction();
        DbCommand Command { get; }
        void Commit();
        int DeleteDataTable(DataTable dt, string deleteSP);
        int ExecuteNonQuery(string sql);
        object ExecuteScalar(string sql);
        DataSet GetDataSet(string sql);
        DataSet GetDataSet(string sql, DataSet ds, string tableName);
        DataTable GetDataTable(string sql, DataTable dt, string tableName);
        DataTable GetDataTable(string sql);
        int Insert<T>(T entity, string sql);
        int InsertDataTable(DataTable table, string insertSP);
        int InsertDataTable(DataTable table, string insertSP, UpdateRowSource updateRowSource);
        DbParameterCollection Parameters { get; }
        void Find(string sql);
        T Find<T>(string sql);
        T Find<T>(string sql, T ent);
        List<T> Query<T>(string sql);
        void Query(string sql);
        ICollection<T> Query<T>(string sql, ICollection<T> entityList);
        void RollBack();
        SqlModes SqlMode { get; set; }
        int Update<T>(T entity, string sql);
        int UpdateDataSet(DataSet ds, string updateSP);
        List<T> QueryViewToObjectGraph<T>(string sql);
        ICollection<T> QueryViewToObjectGraph<T>(string sql, ICollection<T> entityList);
        event EventHandler<LoadEntityEventArgs> LoadEntity;
        event EventHandler OpeningConnection;
    }
}