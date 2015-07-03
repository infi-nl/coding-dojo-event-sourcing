using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;
using NerdDinner.Controllers;
using NerdDinner.Models;
using NerdDinner.Tests.Fakes;
using NUnit.Framework;

namespace NerdDinner.Tests.EventSourcingDojo
{
    [TestFixture]
    public class RSVPControllerTest {

        [Test]
        public void RegisterAction_Should_Connect_User_To_Dinner() {
            RSVPForDinner("scottha", 1);

            AssertRSVPedForDinner("scottha", 1);
        }

        [Test]
        public void RegisterAction_Should_Be_Idempotent()
        {
            RSVPForDinner("scottha", 1);
            RSVPForDinner("scottha", 1);

            AssertRSVPedForDinnerCount("scottha", 1, expectedCount: 1);
        }

        [Test]
        public void RegisterAction_Should_Be_Able_To_Register_Two_Users()
        {
            RSVPForDinner("scottha", 1);
            RSVPForDinner("scotthb", 1);

            AssertRSVPedForDinner("scottha", 1);
            AssertRSVPedForDinner("scotthb", 1);
        }

        [Test]
        public void RegisterAction_Should_Register_User_For_Correct_Dinner()
        {
            RSVPForDinner("scottha", 1);

            AssertRSVPedForDinnerCount("scottha", 2, expectedCount: 0);
        }


        private void RSVPForDinner(string userName, int dinnerId) {
            var controller = CreateRSVPControllerAs(userName);
            controller.Register(dinnerId);
        }

        private void AssertRSVPedForDinner(string userName, int dinnerId)
        {
            var dinnerDetails = GetDinnerDetails(dinnerId);

            var actualRSVP = dinnerDetails.RSVPs.SingleOrDefault(rsvp => rsvp.AttendeeName == userName);

            Assert.IsNotNull(actualRSVP, "RSVP for user {0} not found", userName);
        }

        private void AssertRSVPedForDinnerCount(string userName, int dinnerId, int expectedCount)
        {
            var dinnerDetails = GetDinnerDetails(dinnerId);

            var actualRSVPCount = dinnerDetails.RSVPs.Count(rsvp => rsvp.AttendeeName == userName);

            Assert.AreEqual(expectedCount, actualRSVPCount, "RSVP count not valid");
        }

        private Dinner GetDinnerDetails(int dinnerId)
        {
            var dinnerController = CreateDinnersControllerAs("scotthb");
            var dinnerResult = dinnerController.Details(dinnerId);

            return GetViewModel<Dinner>(dinnerResult);
        }

        public T GetViewModel<T>(ActionResult result) where T : class{
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;

            Assert.IsInstanceOf<T>(viewResult.Model);
            return (T)viewResult.Model;
        }

        #region setup

        [SetUp]
        public void SetUp() {
            InitializeLocalDbWithTestData();

            this.testDinnerRepository = new DinnerRepository(new NerdDinners());
        }

        

        private static void InitializeLocalDbWithTestData() {
            var dbContext = new NerdDinners();
            dbContext.Database.CreateIfNotExists();
            dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Dinners]");
            dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Events]");
            
            var testData = FakeDinnerData.CreateTestDinners();

            foreach (var testDinner in testData.Item1) {
                dbContext.Dinners.Add(testDinner);
            }
            foreach (var testEvent in testData.Item2) {
                dbContext.Events.Add(testEvent);
            }

            dbContext.SaveChanges();
        }

        RSVPController CreateRSVPControllerAs(string userName)
        {
            var mock = new Mock<ControllerContext>();
            var nerdIdentity = FakeIdentity.CreateIdentity(userName);
            mock.SetupGet(p => p.HttpContext.User.Identity).Returns(nerdIdentity);

            var controller = new RSVPController(testDinnerRepository);
            controller.ControllerContext = mock.Object;

            return controller;
        }

        DinnersController CreateDinnersControllerAs(string userName)
        {

            var mock = new Mock<ControllerContext>();
            var nerdIdentity = FakeIdentity.CreateIdentity(userName);
            mock.SetupGet(p => p.HttpContext.User.Identity).Returns(nerdIdentity);

            var controller = new DinnersController(testDinnerRepository, nerdIdentity);
            controller.ControllerContext = mock.Object;

            return controller;
        }


        IDinnerRepository testDinnerRepository;

        #endregion

    }
}
