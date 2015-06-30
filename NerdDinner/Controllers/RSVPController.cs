using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using NerdDinner.Helpers;
using NerdDinner.Models;

namespace NerdDinner.Controllers
{
    [HandleErrorWithELMAH]
    public class RSVPController : Controller
    {

        IDinnerRepository dinnerRepository;
        

        //
        // Dependency Injection enabled constructors
        public RSVPController()
            : this(new DinnerRepository())
        {
        }

        public RSVPController(IDinnerRepository repository)
        {
            dinnerRepository = repository;
        }

        //
        // AJAX: /Dinners/Register/1
        [Authorize, HttpPost]
        public ActionResult Register(int id)
        {
            Dinner dinner = dinnerRepository.Find(id);

            NerdIdentity nerd = (NerdIdentity)User.Identity;

            dinner.RSVP(nerd.Name, nerd.FriendlyName);

            dinnerRepository.StoreEventsForDinner(dinner);

            dinnerRepository.SubmitChanges();
            

            return Content("Thanks - we'll see you there!");
        }

        //
        // AJAX: /RSVP/Cancel/1

        [Authorize, HttpPost]
        public ActionResult Cancel(int id)
        {
            throw new NotImplementedException("Dojo ftw");
            //Dinner dinner = dinnerRepository.Find(id);

            //RSVP rsvp = dinner.RSVPs.SingleOrDefault(r => this.User.Identity.Name == (r.AttendeeNameId ?? r.AttendeeName));
            //if (rsvp != null)
            //{
            //    dinnerRepository.DeleteRsvp(rsvp);
            //    dinnerRepository.SubmitChanges();
            //}

            //return Content("Sorry you can't make it!");
        }

        
    }
}
