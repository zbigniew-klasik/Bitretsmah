using Bitretsmah.Data.Mega;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace Bitretsmah.Tests.Integration.Data.Mega
{
    public class MegaCredentialVerifierShould
    {
        [Test]
        public async Task ReturnTrueForCorrectCredential()
        {
            var correctCredential = AppConfigHelper.GetTestMegaCredential();
            var verifier = new MegaCredentialVerifier();
            var result = await verifier.Verify(correctCredential);
            result.Should().BeTrue();
        }

        [Test]
        public async Task ReturnFalseForCorrectCredential()
        {
            var correctCredential = new NetworkCredential("fake.user@server.com", "Fake_Pa$$word");
            var verifier = new MegaCredentialVerifier();
            var result = await verifier.Verify(correctCredential);
            result.Should().BeFalse();
        }
    }
}