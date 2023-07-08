using BusinessObject;
using DataAccess.Repository.MemberRepo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dStore.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            ClaimsPrincipal principal = HttpContext.User;
            return principal.Identity.IsAuthenticated ? RedirectToAction("Index", "Home") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] string username, [FromForm] string password)
        {
            try
            {
                IMemberRepository memberRepository = new MemberRepository();
                Member loginMember = memberRepository.Login(username, password);
                if (loginMember.Status == 0)
                {
                    throw new Exception("Member has been banned please contact admin!");
                }
                if (loginMember != null && loginMember.Status == 1)
                {
                    var memberClaims = new List<Claim>()
                    {
                        new Claim("MemberId", loginMember.MemberId.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, loginMember.Username),
                        new Claim(ClaimTypes.Name, loginMember.Fullname),
                        new Claim(ClaimTypes.Role, loginMember.Role == 0 ? "Admin" : "User")
                    };
                    var memberIdentity = new ClaimsIdentity(memberClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var memberPrincipal = new ClaimsPrincipal(memberIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, memberPrincipal);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Username = username;
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        public IActionResult UserAccessDenied()
        {
            return View();
        }
    }
}
