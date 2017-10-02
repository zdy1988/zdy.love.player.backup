using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhu.Untils
{
    public static class SQLiteDatabase
    {
        public static async Task Initialize()
        {
            string dbPath = Environment.CurrentDirectory + "/WantCha.db";/*指定数据库路径 */
            if (!File.Exists(dbPath))
            {
                //创建数据库
                SQLiteConnection.CreateFile(dbPath);
            }

            //创建数据库表
            using (SQLiteConnection conn = new SQLiteConnection("Data Source =" + dbPath))
            {
                conn.Open();

                //if (!await conn.ExistsTable("Movie"))
                //{
                //    await conn.CreateTableMovie();
                //}

                //if (!await conn.ExistsTable("NetTV"))
                //{
                //    await conn.CreateTableNetTV();
                //}
            }
        }

        private static async Task<bool> ExistsTable(this SQLiteConnection conn, string tableName)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';";
            object results = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(results) > 0;
        }

        private static Task<int> CreateTableMovie(this SQLiteConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                          CREATE TABLE Movie (
                            ID         [INTEGER] PRIMARY KEY AUTOINCREMENT NOT NULL,
                            MD5        [VARCHAR] (100)   ,
                            Code       [VARCHAR] (50)    ,
                            SubType    [VARCHAR] (10)    ,
                            Title      [VARCHAR] (200)   ,
                            FilePath   [TEXT]            ,
                            StreamNetworkAddress  [TEXT] ,
                            Actors     [VARCHAR] (100)   ,
                            Directors  [VARCHAR] (100)   ,
                            Pubdates   [DATETIME]        ,
                            Year       [INTEGER]         ,
                            Languages  [VARCHAR] (100)   ,
                            Durations  [VARCHAR] (20)    ,
                            Countries  [VARCHAR] (100)   ,
                            Summary    [TEXT]            ,
                            Introduction [TEXT]          ,
                            Rating     [INTEGER]  DEFAULT (0),
                            IsFavorite [BOOLEAN]  DEFAULT (0),
                            EnterDate  [DATETIME] DEFAULT (Datetime('now')),
                            UpdateDate [DATETIME] DEFAULT (Datetime('now'))       
                          )";
            return cmd.ExecuteNonQueryAsync();
        }

        private static Task<int> CreateTableNetTV(this SQLiteConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                          CREATE TABLE NetTV (
                            ID         [INTEGER] PRIMARY KEY AUTOINCREMENT NOT NULL,
                            Code       [VARCHAR] (50)    ,
                            SubType    [VARCHAR] (10)    ,
                            Title      [VARCHAR] (200)   ,
                            StreamNetworkAddress  [TEXT] ,
                            Actors     [VARCHAR] (100)   ,
                            Languages  [VARCHAR] (100)   ,
                            Countries  [VARCHAR] (100)   ,
                            Summary    [TEXT]            ,
                            Introduction [TEXT]          ,
                            Rating     [INTEGER]  DEFAULT (0),
                            IsFavorite [BOOLEAN]  DEFAULT (0),
                            EnterDate  [DATETIME] DEFAULT (Datetime('now')),
                            UpdateDate [DATETIME] DEFAULT (Datetime('now'))  
                          )";
            return cmd.ExecuteNonQueryAsync();
        }
    }
}
