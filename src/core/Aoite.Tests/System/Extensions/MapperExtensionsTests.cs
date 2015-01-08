using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Extensions
{
    public class MapperExtensionsTests
    {
        public enum UserStatus
        {
            Unactived,
            Actived,
            Disabled
        }
        public class User
        {
            public long Id { get; set; }
            public string Username { get; set; }
            [Ignore]
            public string IgnoreP1 { get; set; }
            public string IgnoreP2 { get; set; }
            public UserStatus Status { get; set; }
        }
        private DataTable CreateTable(bool statusIsInt32 = true)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", Types.Int64);
            table.Columns.Add("Username", Types.String);
            table.Columns.Add("Status", statusIsInt32 ? Types.Int32 : Types.String);
            return table;
        }

        [Fact()]
        public void MapFromDataRowTest()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            var table = CreateTable();
            var id = GA.CreateMockValue<long>();
            var username = GA.CreateMockValue<string>();
            var status = GA.CreateMockValue<UserStatus>();
            table.Rows.Add(id, username, status);

            var u = mapper.From(table.Rows[0]).To(new User());
            Assert.Equal(id, u.Id);
            Assert.Equal(username, u.Username);
            Assert.Equal(status, u.Status);
        }
        [Fact()]
        public void MapFromDataReaderTest()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            var table = CreateTable();
            var id = GA.CreateMockValue<long>();
            var username = GA.CreateMockValue<string>();
            var status = GA.CreateMockValue<UserStatus>();
            table.Rows.Add(id, username, status);

            var reader = table.CreateDataReader();
            reader.Read();
            var u = mapper.From(reader).To(new User());
            Assert.Equal(id, u.Id);
            Assert.Equal(username, u.Username);
            Assert.Equal(status, u.Status);
        }

        [Fact()]
        public void MapFromEntityTest()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            var table = CreateTable();
            var id = GA.CreateMockValue<long>();
            var username = GA.CreateMockValue<string>();
            var status = GA.CreateMockValue<UserStatus>();
            table.Rows.Add(id, username, status);
            var row = table.Rows[0];
            var u = mapper.From(row).To(new User());
            Assert.Equal(id, u.Id);
            Assert.Equal(username, u.Username);
            Assert.Equal(status, u.Status);

            u.Id = 5;
            u.Username = "abcdefg";
            u.Status = UserStatus.Actived;

            mapper.From(u).To(row);
            Assert.Equal(row["Id"], u.Id);
            Assert.Equal(row["Username"], u.Username);
            Assert.Equal(row["Status"], (int)u.Status);
        }

        [Fact()]
        public void EnumTest1()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            DataTable table = new DataTable();
            table.Columns.Add("Status", Types.String);
            table.Rows.Add("ACTIVED");
            User u = new User();
            mapper.From(table.Rows[0]).To(u);

            Assert.Equal(u.Status, UserStatus.Actived);
        }

        [Fact()]
        public void EnumTest2()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            DataTable table = new DataTable();
            table.Columns.Add("Status", Types.Int32);
            table.Rows.Add(1);
            User u = new User();
            mapper.From(table.Rows[0]).To(u);

            Assert.Equal(u.Status, UserStatus.Actived);
        }

        [Fact()]
        public void EnumTest3()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            DataTable table = new DataTable();
            table.Columns.Add("Status", Types.String);
            table.Rows.Add("ACTIVED");

            User u = new User();
            mapper.From(table.Rows[0]).To(u);
            Assert.Equal(u.Status, UserStatus.Actived);

            u.Status = UserStatus.Disabled;
            mapper.From(u).To(table.Rows[0]);
            Assert.Equal(table.Rows[0][0], "Disabled");
        }

        [Fact()]
        public void EnumTest4()
        {
            var mapper = TypeMapper.Instance<User>.Mapper;
            DataTable table = new DataTable();
            table.Columns.Add("Status", Types.Int32);
            table.Rows.Add(1);
            User u = new User();
            mapper.From(table.Rows[0]).To(u);
            Assert.Equal(u.Status, UserStatus.Actived);

            u.Status = UserStatus.Disabled;
            mapper.From(u).To(table.Rows[0]);
            Assert.Equal(table.Rows[0][0], 2);
        }
    }
}
