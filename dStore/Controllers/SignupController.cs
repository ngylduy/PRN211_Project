﻿using DataAccess.Repository.MemberRepo;
using Microsoft.AspNetCore.Mvc;

namespace dStore.Controllers
{
    public class SignupController : Controller
    {
        IMemberRepository memberRepository = new MemberRepository();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BusinessObject.Member member, [FromForm] string confirm)
        {
            try
            {
                if (!member.Password.Equals(confirm))
                {
                    throw new Exception("Confirm and Password are not matched!!!");
                }
                if (ModelState.IsValid)
                {
                    memberRepository.AddMember(member);
                }
                BusinessObject.Member newMember = memberRepository.GetMember(member.Email);
                TempData["Create"] = "Create Member with the ID <strong>" + newMember.MemberId + "</strong> successfully!!";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index", member);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
