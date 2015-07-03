using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NerdDinner.Tests.CodingDojo
{
    partial class DojoTests
    {

        [Test]
        public void Cancel_RSVP_Should_Remove_User_From_RSVP_List()
        {
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
        public void Cancel_RSVP_For_Not_RSVPed_User_Should_Not_Fail()
        {
            AssertRSVPedForDinnerCount("scottha", 1, expectedCount: 0);

            CancelRSVP("scottha", 1);
        }

    }
}
