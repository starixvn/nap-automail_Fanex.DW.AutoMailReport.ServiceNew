using System;
using System.Collections.Generic;
using Fanex.MailDeliver.Model;
using NegativeBalance.Business.Models;
using NegativeBalance.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services BalanceNotificationServiceUnitTests")]
    public class BalanceNotificationServiceUnitTests
    {
        private RepositoryBalanceService repository;
        private DataService dataService;
        private FileService fileService;
        private RecipientService recipientService;
        private MailDeliverService mailDeliverService;

        private void Initialize()
        {
            repository = Substitute.For<RepositoryBalanceService>();
            dataService = Substitute.For<DataService>();
            fileService = Substitute.For<FileService>();
            recipientService = Substitute.For<RecipientService>();
            mailDeliverService = Substitute.For<MailDeliverService>();
        }

        [Test]
        public void Execute_QueryMatchesBySiteNoData_MethodSendShouldNotBeCalled()
        {
            // Arrange
            Initialize();
            repository.QuerySites(false).Returns(new List<Site> { new Site { LicenseeName = "12bet" } });
            dataService.QueryMatchesBySite(Arg.Any<Site>()).Returns(new Tuple<List<Match>, string>(new List<Match>(), "12bet"));

            // Act
            var service = new BalanceNotificationServiceImp(repository, dataService, fileService, recipientService, mailDeliverService);
            service.Execute(false);

            // Assert
            fileService.DidNotReceive().ReadContent(Arg.Any<List<Match>>());
            recipientService.DidNotReceive().CreateMail(Arg.Any<Site>(), Arg.Any<string>());
            mailDeliverService.DidNotReceive().Send(Arg.Any<MailRecord>());
            repository.DidNotReceive().UpdateMatchSent(Arg.Any<string>());
        }

        [Test]
        public void Execute_SendMailFail_MethodUpdateMatchSentShouldNotBeCalled()
        {
            // Arrange
            Initialize();
            repository.QuerySites(false).Returns(new List<Site> { new Site { LicenseeName = "12bet" } });
            var data = TestLoader.DataLoader<Tuple<List<Match>, string>>("DataService-QueryMatchesBySite-expected.json");
            dataService.QueryMatchesBySite(Arg.Any<Site>()).Returns(data);
            mailDeliverService.Send(Arg.Any<MailRecord>()).Returns(false);

            // Act
            var service = new BalanceNotificationServiceImp(repository, dataService, fileService, recipientService, mailDeliverService);
            service.Execute(false);

            // Assert
            repository.DidNotReceive().UpdateMatchSent(Arg.Any<string>());
        }

        [Test]
        public void Execute_SendMailSuccessfully_MethodUpdateMatchSentShouldBeCalled()
        {
            // Arrange
            Initialize();
            repository.QuerySites(false).Returns(new List<Site> { new Site { LicenseeName = "12bet" } });
            var data = TestLoader.DataLoader<Tuple<List<Match>, string>>("DataService-QueryMatchesBySite-expected.json");
            dataService.QueryMatchesBySite(Arg.Any<Site>()).Returns(data);
            mailDeliverService.Send(Arg.Any<MailRecord>()).Returns(true);

            // Act
            var service = new BalanceNotificationServiceImp(repository, dataService, fileService, recipientService, mailDeliverService);
            service.Execute(false);

            // Assert
            repository.Received().UpdateMatchSent(Arg.Any<string>());
        }

        [Test]
        public void Execute_CheckValidData_ShouldBeCalled()
        {
            // Arrange
            Initialize();
            repository.QuerySites(false).Returns(new List<Site> { new Site { LicenseeName = "12bet" } });
            var data = TestLoader.DataLoader<Tuple<List<Match>, string>>("DataService-QueryMatchesBySite-expected.json");
            data.Item1[0].MatchInfo = new MatchInfo();
            dataService.QueryMatchesBySite(Arg.Any<Site>()).Returns(data);
            mailDeliverService.Send(Arg.Any<MailRecord>()).Returns(false);

            // Act
            var service = new BalanceNotificationServiceImp(repository, dataService, fileService, recipientService, mailDeliverService);
            service.Execute(false);

            // Assert
            repository.DidNotReceive().UpdateMatchSent(Arg.Any<string>());
        }
    }
}