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
            string dbPath = Untils.Constants.DatabasePath;/*指定数据库路径 */
            if (!File.Exists(dbPath))
            {
                //创建数据库
                SQLiteConnection.CreateFile(dbPath);
            }

            using (SQLiteConnection conn = new SQLiteConnection("Data Source =" + dbPath))
            {
                conn.Open();

                for (var i = 0; i < tableNames.Length; i++)
                {
                    if (!await conn.ExistsTable(tableNames[i]))
                    {
                        await conn.CreateTable(sqlQueries[i]);
                    }
                }
            }
        }

        private static async Task<bool> ExistsTable(this SQLiteConnection conn, string tableName)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';";
            object results = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(results) > 0;
        }

        private static Task<int> CreateTable(this SQLiteConnection conn, string sqlQuery)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sqlQuery;
            return cmd.ExecuteNonQueryAsync();
        }

        private static string[] tableNames = new string[] {
            "Actor",
            "Group" ,
            "GroupMember",
            "Image",
            "Media",
            "Seen",
            "Tag"
        };

        private static string[] sqlQueries = new string[] {
            @"CREATE TABLE Actor (
                ID         INTEGER        PRIMARY KEY AUTOINCREMENT
                                          NOT NULL,
                Name       NVARCHAR (255) COLLATE NOCASE,
                FormerName NVARCHAR (255) COLLATE NOCASE,
                Summary    TEXT           COLLATE NOCASE,
                BirthDay   DATETIME,
                DebutDay   DATETIME,
                Introduce  TEXT           COLLATE NOCASE
            );",

            @"CREATE TABLE [Group] (
                ID   INTEGER       PRIMARY KEY AUTOINCREMENT
                                   NOT NULL,
                Name VARCHAR (100) NOT NULL
            );",

            @"CREATE TABLE GroupMember (
                ID      INTEGER NOT NULL
                                PRIMARY KEY AUTOINCREMENT,
                GroupID INTEGER NOT NULL,
                MediaID INTEGER NOT NULL,
                IsTop   BOOLEAN
            );",

            @"CREATE TABLE Image (
                ID           INTEGER       PRIMARY KEY AUTOINCREMENT
                                           NOT NULL,
                FileName     VARCHAR (100) NOT NULL
                                           COLLATE NOCASE,
                Type         VARCHAR (10)  NOT NULL
                                           COLLATE NOCASE,
                TypeSourceID INTEGER       NOT NULL,
                Path         TEXT          COLLATE NOCASE
            );",

            @"CREATE TABLE Media (
                ID              [INTEGER]       PRIMARY KEY AUTOINCREMENT
                                                NOT NULL,
                Title           [VARCHAR] (200),
                MediaType       INTEGER         DEFAULT (0),
                MD5             [VARCHAR] (100),
                Cover           VARCHAR (100),
                Code            [VARCHAR] (50),
                MediaSource     [TEXT],
                MediaSourceType INTEGER,
                Actors          [VARCHAR] (100),
                Directors       [VARCHAR] (100),
                PubDate         [DATETIME],
                Year            [INTEGER],
                Languages       [VARCHAR] (100),
                Duration        BIGINT          DEFAULT (0) 
                                                NOT NULL,
                Countries       [VARCHAR] (100),
                Summary         [TEXT],
                Introduction    [TEXT],
                Rating          [INTEGER]       DEFAULT (0),
                IsFavorite      BOOLEAN         DEFAULT (0),
                EnterDate       [DATETIME]      DEFAULT (Datetime('now') ),
                UpdateDate      [DATETIME]      DEFAULT (Datetime('now') ) 
            );",

            @"CREATE TABLE Seen (
                ID          INTEGER       PRIMARY KEY AUTOINCREMENT
                                          NOT NULL,
                SeenDate    DATETIME      NOT NULL,
                Title       VARCHAR (200) NOT NULL,
                MediaSource TEXT          NOT NULL,
                MediaID     INTEGER       NOT NULL
            );",

            @"CREATE TABLE Tag (
                ID           INTEGER      PRIMARY KEY AUTOINCREMENT
                                          NOT NULL,
                Keyword      VARCHAR (20) NOT NULL
                                          COLLATE NOCASE,
                Type         VARCHAR (10) NOT NULL
                                          COLLATE NOCASE,
                TypeSourceID INTEGER      NOT NULL
            );"

        };
    }
}
