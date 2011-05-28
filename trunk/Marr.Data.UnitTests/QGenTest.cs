﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marr.Data.QGen;
using Marr.Data.UnitTests.Entities;
using Rhino.Mocks;
using Marr.Data.Mapping;
using Marr.Data;
using System.Data.Common;
using Marr.Data.QGen.Dialects;
using Marr.Data.Tests.Entities;

namespace Marr.Data.UnitTests
{
    /// <summary>
    /// Summary description for QGenTest
    /// </summary>
    [TestClass]
    public class QGenTest : TestBase
    {
        [TestMethod]
        public void SqlServerUpdateQuery_ShouldGenQuery()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            mappingHelper.CreateParameters<Person>(person, columns, true);

            int idValue = 7;
            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.ID == person.ID || p.ID == idValue || p.Name == person.Name && p.Name == "Bob", false);

            IQuery query = new UpdateQuery(new SqlServerDialect(), columns, command, "dbo.People", where.ToString());

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.IsTrue(queryText.Contains("UPDATE [dbo].[People]"));
            Assert.IsTrue(queryText.Contains("[Name]"));
            Assert.IsTrue(queryText.Contains("[Age]"));
            Assert.IsTrue(queryText.Contains("[IsHappy]"));
            Assert.IsTrue(queryText.Contains("[BirthDate]"));
            Assert.IsTrue(queryText.Contains("[ID] = @P4"));
            Assert.IsTrue(queryText.Contains("[ID] = @P5"));
            Assert.IsTrue(queryText.Contains("[Name] = @P6"));
            Assert.IsTrue(queryText.Contains("[Name] = @P7"));
            Assert.AreEqual(command.Parameters["@P4"].Value, 1);
            Assert.AreEqual(command.Parameters["@P5"].Value, 7);
            Assert.AreEqual(command.Parameters["@P6"].Value, "Jordan");
            Assert.AreEqual(command.Parameters["@P7"].Value, "Bob");
        }

        [TestMethod]
        public void SqlServerInsertQuery_ShouldGenQuery()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            mappingHelper.CreateParameters<Person>(person, columns, true);

            IQuery query = new InsertQuery(new SqlServerDialect(), columns, command, "dbo.People");

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.IsTrue(queryText.Contains("INSERT INTO [dbo].[People]"));
            Assert.IsFalse(queryText.Contains("@ID"), "Should not contain ID column since it is marked as AutoIncrement");
            Assert.IsTrue(queryText.Contains("[Name]"), "Should contain the name column");
        }

        [TestMethod]
        public void SqlServerDeleteQuery_ShouldGenQuery()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.ID == 5, false);
            IQuery query = new DeleteQuery("dbo.People", where.ToString());

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.IsTrue(queryText.Contains("DELETE FROM dbo.People"));
            Assert.IsTrue(queryText.Contains("WHERE (([ID] = @P0))"));
            Assert.AreEqual(command.Parameters["@P0"].Value, 5);
        }

        [TestMethod]
        public void SqlServerSelectQuery_ShouldGenQuery()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name == "John" && p.Age > 15 || p.Age < 5 && p.Age > 1, false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.AreEqual(command.Parameters["@P1"].Value, 15);
            Assert.AreEqual(command.Parameters["@P2"].Value, 5);
            Assert.AreEqual(command.Parameters["@P3"].Value, 1);
            Assert.IsTrue(queryText.Contains("([Name] = @P0) AND ([Age] > @P1))"));
            Assert.IsTrue(queryText.Contains("([Age] < @P2) AND ([Age] > @P3))"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_Contains_Constant()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.Contains("John"), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Name] LIKE '%' + @P0 + '%'"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_Contains_Variable()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            string john = "John";

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.Contains(john), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Name] LIKE '%' + @P0 + '%'"));
        }


        [TestMethod]
        public void SqlServerSelectQuery_StartsWith_Constant()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.StartsWith("John"), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Name] LIKE @P0 + '%'"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_StartsWith_Variable()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            string john = "John";

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.StartsWith(john), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Name] LIKE @P0 + '%'"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_EndsWith_Constant()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.EndsWith("John"), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Name] LIKE '%' + @P0"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_EndsWith_Variable()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            string john = "John";

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.EndsWith(john), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Name] LIKE '%' + @P0"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_BinaryExpression_MethodExpression()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Age > 5 && p.Name.Contains("John"), false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, 5);
            Assert.AreEqual(command.Parameters["@P1"].Value, "John");
            Assert.IsTrue(queryText.Contains("[Age] > @P0"));
            Assert.IsTrue(queryText.Contains("[Name] LIKE '%' + @P1 + '%'"));
        }

        [TestMethod]
        public void SqlServerSelectQuery_MethodExpression_BinaryExpression()
        {
            // Arrange
            var command = new System.Data.SqlClient.SqlCommand();
            ColumnMapCollection columns = MapRepository.Instance.GetColumns(typeof(Person));
            MappingHelper mappingHelper = new MappingHelper(command);

            Person person = new Person();
            person.ID = 1;
            person.Name = "Jordan";
            person.Age = 33;
            person.IsHappy = true;
            person.BirthDate = new DateTime(1977, 1, 22);

            List<Person> list = new List<Person>();

            var where = new WhereBuilder<Person>(command, new SqlServerDialect(), p => p.Name.Contains("John") && p.Age > 5, false);
            IQuery query = new SelectQuery(new SqlServerDialect(), columns, "dbo.People", where.ToString(), "", false);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsNotNull(queryText);
            Assert.AreEqual(command.Parameters["@P0"].Value, "John");
            Assert.AreEqual(command.Parameters["@P1"].Value, 5);
            Assert.IsTrue(queryText.Contains("[Name] LIKE '%' + @P0 + '%'"));
            Assert.IsTrue(queryText.Contains("[Age] > @P1"));            
        }

        /// <summary>
        /// Ref: bug fix for work item #34 submitted by vitidev
        /// </summary>
        [TestMethod]
        public void WhenColumnNameDiffersFromProperty_InsertQueryShouldUseColumnName()
        {
            // Arrange
            Person2 person = new Person2 { Name = "Bob", Age = 40, BirthDate = DateTime.Now };
            Dialect dialect = new SqlServerDialect();
            ColumnMapCollection mappings = MapRepository.Instance.GetColumns(typeof(Person2));
            DbCommand command = new System.Data.SqlClient.SqlCommand();
            var mappingHelper = new MappingHelper(command);
            mappingHelper.CreateParameters<Person2>(person, mappings, true);
            string targetTable = "PersonTable";
            InsertQuery query = new InsertQuery(dialect, mappings, command, targetTable);

            // Act
            string queryText = query.Generate();

            // Assert
            Assert.IsTrue(queryText.Contains("[PersonName]"), "Query should contain column name");
            Assert.IsTrue(queryText.Contains("[PersonAge]"), "Query should contain column name");
            Assert.IsTrue(queryText.Contains("[BirthDate]"), "Query should contain property name");
            Assert.IsTrue(queryText.Contains("[IsHappy]"), "Query should contain property name");
        }
    }
}
