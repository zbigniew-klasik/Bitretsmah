﻿using Bitretsmah.Core.Models;
using LiteDB;
using System.Net;
using System.Runtime.CompilerServices;

namespace Bitretsmah.Data.LiteDB.Internal
{
    internal class Db : LiteDatabase
    {
        private const string ConnecionString = @"bitretsmah.db";

        static Db()
        {
            Configure();
        }

        public Db()
            : base(ConnecionString)
        {
        }

        public const string AccountsCollectionName = "accounts";
        public const string BackupsCollectionName = "backups";
        public const string TargetsCollectionName = "targets";

        public LiteCollection<Account> Accounts => GetCollection<Account>(AccountsCollectionName);
        public LiteCollection<Backup> Backups => GetCollection<Backup>(BackupsCollectionName);
        public LiteCollection<Target> Targets => GetCollection<Target>(TargetsCollectionName);

        private static void Configure()
        {
            BsonMapper.Global.Entity<NetworkCredential>()
                .Index(x => x.UserName, true)
                .Field(x => x.Password, "Password")
                .Ignore(x => x.SecurePassword)
                .Ignore(x => x.Domain);
        }

        // TODO: Creat drop database feature, and use it in the TestCleanUpHelper
    }
}