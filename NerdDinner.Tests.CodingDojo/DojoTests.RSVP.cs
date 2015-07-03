using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NerdDinner.Tests.CodingDojo
{
    [TestFixture]
    partial class DojoTests {

        [Test]
        public void CreateDinner_Should_Add_Host_As_RSVP()
        {
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
        public void RegisterAction_Should_Connect_User_To_Dinner()
        {
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
        public void When_RSVPed_Dinner_Shows_In_MyDinners()
        {
            RSVPForDinner("scottha", 1);

            AssertDinnerInMyDinners("scottha", 1);
        }
    }
}
