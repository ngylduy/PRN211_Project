﻿using BusinessObject;
using DataAccess.Repository.MemberRepo;
using DataAccess.Repository.OrderDetailRepo;
using DataAccess.Repository.OrderRepo;
using dStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dStore.Controllers
{

    public class MemberController : Controller
    {
        IMemberRepository memberRepository = null;
        public MemberController()
        {
            memberRepository = new MemberRepository();
        }

        [Authorize(Roles = "Admin")]
        // GET: MemberController
        public async Task<IActionResult> Index(string search, int? page)
        {
            if (page == null)
            {
                page = 1;
            }

            IEnumerable<Member> members = memberRepository.GetMembersList().OrderByDescending(mem => mem.Fullname);
            if (!string.IsNullOrEmpty(search))
            {
                members = members.Where(mem => mem.Fullname.ToLower().Contains(search.ToLower()));
                ViewBag.Search = search;
            }

            int pageSize = 10;

            return View(await PaginatedList<Member>.CreateAsync(members.AsQueryable(), page ?? 1, pageSize));
        }

        [Authorize(Roles = "Admin")]
        // GET: MemberController/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Member ID is not found!!!");
                }
                Member member = memberRepository.GetMember(id.Value);
                if (member == null)
                {
                    throw new Exception("Member ID is not found!!!");
                }
                return View(member);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: MemberController/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: MemberController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member, [FromForm] string confirm)
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
                Member newMember = memberRepository.GetMember(member.Email);
                TempData["Create"] = "Create Member with the ID <strong>" + newMember.MemberId + "</strong> successfully!!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(member);
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: MemberController/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Member ID is not found!!!");
                }
                Member member = memberRepository.GetMember(id.Value);
                if (member == null)
                {
                    throw new Exception("Member ID is not found!!!");
                }
                return View(member);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: MemberController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Member member, [FromForm] string confirm)
        {
            try
            {
                if (id != member.MemberId)
                {
                    throw new Exception("Member ID is not matched!! Please try again");
                }
                if (ModelState.IsValid)
                {
                    if (!member.Password.Equals(confirm))
                    {
                        throw new Exception("Confirm and Password are not matched!!!");
                    }
                    memberRepository.UpdateMember(member);
                    TempData["Update"] = "Update Member with the ID <strong>" + id + "</strong> successfully!!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(member);
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: MemberController/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Member ID is not found!!!");
                }
                Member member = memberRepository.GetMember(id.Value);
                if (member == null)
                {
                    throw new Exception("Member ID is not found!!!");
                }
                return View(member);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: MemberController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                IOrderRepository orderRepository = new OrderRepository();
                IOrderDetailRepository orderDetailRepository = new OrderDetailRepository();

                IEnumerable<Order> orders = orderRepository.GetOrders(id);
                if (orders != null && orders.Any())
                {
                    foreach (var order in orders)
                    {
                        orderDetailRepository.DeleteOrderDetails(order.OrderId);
                    }
                }
                orderRepository.DeleteByMember(id);
                memberRepository.DeleteMember(id);
                TempData["Delete"] = "Delete Member with the ID <strong>" + id + "</strong> successfully!!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult Profile()
        {
            try
            {
                string strMemberId = User.Claims.SingleOrDefault(c => c.Type.Equals("MemberId")).Value;
                Member member = memberRepository.GetMember(int.Parse(strMemberId));
                return View(member);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Member member)
        {
            Request.Headers["Content-Type"] += ";charset=UTF-8";
            Response.Headers["Content-Type"] += ";charset=UTF-8";

            try
            {
                string strMemberId = User.Claims.SingleOrDefault(c => c.Type.Equals("MemberId")).Value;
                int id = int.Parse(strMemberId);
                if (id != member.MemberId)
                {
                    throw new Exception("Member ID is not matched!! Please try again");
                }
                if (ModelState.IsValid)
                {
                    memberRepository.UpdateMember(member);
                    ViewBag.Success = "Update your Profile successfully!!";
                }
                else
                {
                    ViewBag.Error = "Update not successfully!!";
                    throw new Exception("Update not successfully!!");
                }
                return View(member);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(member);
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult changePassword()
        {
            return View();
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult changePassword(Password password)
        {
            Request.Headers["Content-Type"] += ";charset=UTF-8";
            Response.Headers["Content-Type"] += ";charset=UTF-8";

            string oldPassword = password.OldPassword;
            string newPassword = password.NewPassword;
            string confirmPassword = password.ConfirmPassword;

            try
            {
                string strMemberId = User.Claims.SingleOrDefault(c => c.Type.Equals("MemberId")).Value;
                Member member = memberRepository.GetMember(int.Parse(strMemberId));

                if (!member.Password.Equals(oldPassword))
                {
                    throw new Exception("Old Password not pass!!!");
                }
                if (!newPassword.Equals(confirmPassword))
                {
                    throw new Exception("Confirm Password and New Password are not matched!!!");
                }
                member.Password = newPassword;
                memberRepository.UpdateMember(member);
                ViewBag.Success = "Password changed successfully!!";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

    }
}
