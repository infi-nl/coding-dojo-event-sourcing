using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
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
        public void CreateDinner_Should_Add_Host_As_RSVP() {
            var dinnerID = CreateDinner();

            AssertRSVPedForDinner("scottha", dinnerID);
        }

        [Test]
        public void CreateDinner_Twice_Should_Work()
        {
            CreateDinner();
            CreateDinner();
        }

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

        [Test]
        public void Cancel_RSVP_Should_Remove_User_From_RSVP_List() {
            RSVPForDinner("scottha", 1);

            CancelRSVP("scottha", 1);

            AssertRSVPedForDinnerCount("scottha", 1, expectedCount: 0);
        }

        [Test]
        public void Cancel_RSVP_Should_Be_Idempotent()
        {
            RSVPForDinner("scottha", 1);

            CancelRSVP("scottha", 1);
            CancelRSVP("scottha", 1);

            AssertRSVPedForDinnerCount("scottha", 1, expectedCount: 0);
        }

        [Test]
        public void Cancel_RSVP_For_Not_RSVPed_User_Should_Work()
        {
            AssertRSVPedForDinnerCount("scottha", 1, expectedCount: 0);

            CancelRSVP("scottha", 1);
        }


        [Test]
        public void When_RSVPed_Dinner_Shows_In_MyDinners() {
            RSVPForDinner("scottha", 1);

            AssertDinnerInMyDinners("scottha", 1);
        }

        [Test]
        public void DinnerHistory_Shows_RSVPed() {
            RSVPForDinner("scottha", 1);

            AssertRSVPedInDinnerHistory("scottha RSVPed", 1);
        }

        [Test]
        public void DinnerHistory_Shows_Canceled()
        {
            RSVPForDinner("scottha", 1);

            CancelRSVP("scottha", 1);

            AssertRSVPedInDinnerHistory("scottha canceled", 1);
        }

        [Test]
        public void DinnerHistory_Shows_RSVPed_First_Then_Canceled()
        {
            RSVPForDinner("scottha", 1);

            CancelRSVP("scottha", 1);

            AssertTextInDinnerHistoryAtIndex("scottha RSVPed", 1, 1); // Event '0' = Dinner host RSVPed
            AssertTextInDinnerHistoryAtIndex("scottha canceled", 1, 2);
        }


        #region helpers

        private int CreateDinner()
        {
            var testDinner = new Dinner { Title = "TestDinner", EventDate = new DateTime(2016, 1, 1), Description = "TestDinner Description", HostedBy = "scottha", HostedById = "scottha", ContactPhone = "0123456789", Address = "TestDinner Address", Country = "Europe" };

            var controller = CreateDinnersControllerAs("scottha");
            var result = controller.Create(testDinner);

            return (int)GetRedirectResultRouteValues(result)["id"];
        }

        private void CancelRSVP(string userName, int dinnerId)
        {
            var controller = CreateRSVPControllerAs(userName);
            controller.Cancel(dinnerId);
        }

        private void RSVPForDinner(string userName, int dinnerId) {
            var controller = CreateRSVPControllerAs(userName);
            controller.Register(dinnerId);
        }

        private void AssertRSVPedInDinnerHistory(string expectedDinnerHistoryText, int dinnerId)
        {
            var dinnerDetails = GetDinnerDetails(dinnerId);

            Assert.IsTrue(dinnerDetails.History.Any(h => h.EndsWith(expectedDinnerHistoryText)), "DinnerHistory text '{0}' not found", expectedDinnerHistoryText);
        }

        private void AssertTextInDinnerHistoryAtIndex(string expectedDinnerHistoryText, int dinnerId, int index)
        {
            var dinnerDetails = GetDinnerDetails(dinnerId);

            Assert.IsTrue(dinnerDetails.History.Count() > index, "DinnerHistory does not contain enough entries");
            Assert.IsTrue(dinnerDetails.History.ElementAt(index).EndsWith(expectedDinnerHistoryText), "DinnerHistory text '{0}' not found at index {1}", expectedDinnerHistoryText, index);
        }


        private void AssertDinnerInMyDinners(string userName, int dinnerId)
        {
            var myDinners = GetMyDinners(userName);

            var actualDinner = myDinners.SingleOrDefault(d => d.DinnerID == dinnerId);

            Assert.IsNotNull(actualDinner, "Dinner in MyDinners for User {0} not found", userName);
        
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

        private ICollection<Dinner> GetMyDinners(string userName)
        {
            var dinnerController = CreateDinnersControllerAs(userName);
            var dinnerResult = dinnerController.My();

            return GetViewModel<IEnumerable<Dinner>>(dinnerResult).ToList();
        }


        public RouteValueDictionary GetRedirectResultRouteValues(ActionResult result) 
        {
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            var redirectToRouteResult = result as RedirectToRouteResult;

            Assert.IsNotNull(redirectToRouteResult);
            return redirectToRouteResult.RouteValues;
        }

        public T GetViewModel<T>(ActionResult result) where T : class{
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;

            Assert.IsInstanceOf<T>(viewResult.Model);
            return (T)viewResult.Model;
        }

#endregion

        #region setup

        [SetUp]
        public void SetUp() {
            InitializeLocalDbWithTestData();
        }

        private static void InitializeLocalDbWithTestData() {
            Database.SetInitializer<NerdDinners>(null);

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

            var controller = new RSVPController(new DinnerRepository(new NerdDinners()));
            controller.ControllerContext = mock.Object;

            return controller;
        }

        DinnersController CreateDinnersControllerAs(string userName)
        {

            var mock = new Mock<ControllerContext>();
            var nerdIdentity = FakeIdentity.CreateIdentity(userName);
            mock.SetupGet(p => p.HttpContext.User.Identity).Returns(nerdIdentity);

            var controller = new DinnersController(new DinnerRepository(new NerdDinners()), nerdIdentity);
            controller.ControllerContext = mock.Object;

            return controller;
        }



        #endregion

    }
}
