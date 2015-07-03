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
        public void DinnerHistory_Shows_RSVPed()
        {
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
        public void DinnerHistory_Is_Sorted_Chronologically()
        {
            RSVPForDinner("scottha", 1);

            CancelRSVP("scottha", 1);

            AssertTextInDinnerHistoryAtIndex("scottha RSVPed", 1, 1); // Event '0' = Dinner host RSVPed
            AssertTextInDinnerHistoryAtIndex("scottha canceled", 1, 2);
        }

    }
}
