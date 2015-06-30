using System.Collections.Generic;
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



        

        private void RSVPForDinner(string userName, int dinnerId) {
            var controller = CreateRSVPControllerAs(userName);
            controller.Register(dinnerId);
        }

        private void AssertRSVPedForDinner(string userName, int dinnerId)
        {
            var dinnerDetails = GetDinnerDetails(dinnerId);

            var expectedRSVP = dinnerDetails.RSVPs.SingleOrDefault(rsvp => rsvp.AttendeeName == userName);

            Assert.IsNotNull(expectedRSVP, "RSVP for user {0} not found", userName);
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
        public void SetUp()
        {
            var testData = FakeDinnerData.CreateTestDinners();
            testDinnerRepository = new FakeDinnerRepository(testData);
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
