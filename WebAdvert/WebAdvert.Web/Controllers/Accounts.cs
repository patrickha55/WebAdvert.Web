using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly CognitoUserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public Accounts(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager as CognitoUserManager<CognitoUser>;
            _pool = pool;
        }

        public async Task<IActionResult> SignUp()
        {
            var model = new SignUpModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);

                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists.");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

                var createdUser = await _userManager.CreateAsync(user, model.Password)
                    .ConfigureAwait(false);

                if (createdUser.Succeeded)
                    RedirectToAction("Confirm");
            }

            return View();
        }

        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found.");
                    return View(model);
                }

                var result = await _userManager.ConfirmSignUpAsync(user, model.Code, true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError(identityError.Code, identityError.Description);
                }
            }


            return View(model);
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(
                        model.Email,
                        model.Password, 
                        model.RememberMe,
                        false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                
                ModelState.AddModelError("LoginError", "Email or Password is incorrect.");
            }

            return View(model);
        }
    }
}