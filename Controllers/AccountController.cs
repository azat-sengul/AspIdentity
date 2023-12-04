using AspIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AspIdentity.Controllers
{
    public class AccountController:Controller
    {

        private readonly UserManager<IdentityUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly SignInManager<IdentityUser> _signManager; 

        public AccountController
        (
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signManager = signManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user != null)
                {
                    await _signManager.SignOutAsync(); // Var olan cookie silinir

                    //Yeni Cookie ekleme işlemi
                    //True program.cs'deki Access Attemps'i dikkate almak için. Aksi durumda false yazılıp bu adım geçilebilir
                    var result = await _signManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true); 

                    if(result.Succeeded)
                    {
                        //Access Attemps Deneme adet ve süre Sıfırlama işlemleri yapılır
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null); // null yerine geçmiş bir tarih yazılabilir

                        return RedirectToAction("Index", "Home");

                    }
                    // Hesap kitlenirse
                    else if(result.IsLockedOut)
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var leftTime = lockoutDate.Value - DateTime.UtcNow;

                        ModelState.AddModelError("", $"Fazla Deneme! Burayı {leftTime.Minutes} sonra tekrar ziyaret edebilirsin");
                    }

                    else
                    {
                        ModelState.AddModelError("", "Hatalı Email ya da Parola");
                    }

                }

                 else
                {
                    ModelState.AddModelError("", "Hatalı Email ya da Parola");
                }

            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {   
            await _signManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

    }
}