using Bitretsmah.Core.Interfaces;
using Bitretsmah.Core.Models;
using Bitretsmah.Data.LiteDB;
using Bitretsmah.Data.LiteDB.Internal;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using Bitretsmah.Tests.Unit;

namespace Bitretsmah.Tests.Integration.Data.LiteDB
{
    [TestFixture]
    public class AccountRepositoryShould
    {
        private IAccountRepository _accountRepository;

        private Account _firstAccount;
        private Account _secondAccount;

        [SetUp]
        public void SetUp()
        {
            _firstAccount = new Account(new NetworkCredential("user1@server.com", "Password-01"), new Quota(10, 3));
            _secondAccount = new Account(new NetworkCredential("user2@server.com", "Password-02"), new Quota(50, 40));

            using (var db = DbFactory.Create())
            {
                db.DropCollection(Db.AccountsCollectionName);
                db.Accounts.Insert(_firstAccount);
                db.Accounts.Insert(_secondAccount);
            }

            _accountRepository = new AccountRepository();
        }

        [Test]
        public async Task GetAllAccounts()
        {
            var accounts = await _accountRepository.GetAll();
            accounts.Count.Should().Be(2);
            accounts[0].ShouldSerializeSameAs(_firstAccount);
            accounts[1].ShouldSerializeSameAs(_secondAccount);
        }

        [Test]
        public async Task AddNewAccountWithProvidedCredential()
        {
            var newCredential = new NetworkCredential("user3@server.com", "Password-03");
            await _accountRepository.AddOrUpdate(newCredential);

            using (var db = DbFactory.Create())
            {
                var account = db.Accounts.FindOne(x => x.Credential.UserName.Equals(newCredential.UserName));
                account.Should().NotBeNull();
                account.Credential.ShouldSerializeSameAs(newCredential);
                account.Quota.ShouldSerializeSameAs(new Quota(0, 0));
            }
        }

        [Test]
        public async Task UpdateCredentialInExistingAccount()
        {
            var updatedCredential = new NetworkCredential("user1@server.com", "New_Password");
            await _accountRepository.AddOrUpdate(updatedCredential);

            using (var db = DbFactory.Create())
            {
                var account = db.Accounts.FindOne(x => x.Credential.UserName.Equals(updatedCredential.UserName));
                account.Should().NotBeNull();
                account.Credential.ShouldSerializeSameAs(updatedCredential);
                account.Quota.ShouldSerializeSameAs(new Quota(10, 3));
            }
        }
    }
}