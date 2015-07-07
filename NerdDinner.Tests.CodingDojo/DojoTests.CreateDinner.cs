using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NerdDinner.Events;
using NerdDinner.Models;

namespace NerdDinner.Tests.CodingDojo
{
    [TestFixture]
    partial class DojoTests {

        [Test]
        public void Create_Dinner_Should_Raise_DinnerCreated()
        {
            var dinnerID = CreateDinner(new Dinner { 
                Title = "TestDinner", 
                EventDate = new DateTime(2016, 1, 1), 
                Description = "TestDinner Description", 
                ContactPhone = "0123456789", 
                Address = "TestDinner Address", 
                Country = "Europe" });

            AssertEventPublished<DinnerCreated>(e=>true, e=> {
                Assert.AreEqual(dinnerID,e.Data.DinnerID);
                Assert.AreEqual("TestDinner",e.Data.Title);
                Assert.AreEqual(new DateTime(2016, 1, 1),e.Data.EventDate); 
                Assert.AreEqual("TestDinner Description",e.Data.Description);
                Assert.AreEqual("scottha", e.Data.HostedBy);
                Assert.AreEqual("scottha",e.Data.HostedById);
                Assert.AreEqual("0123456789", e.Data.ContactPhone);
                Assert.AreEqual("TestDinner Address",e.Data.Address);
                Assert.AreEqual("Europe", e.Data.Country);
            });
        }
    }
}
