using System.Security.AccessControl;
using AspIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController:Controller
    {
        //Sayfa içinde UserManager'ı kullanmak için gerekli injection işlemi
        private UserManager<IdentityUser> _userManager;

        //Sayfa içinde RoleManager'ı kullanmak için gerekli injection işlemi

        private RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // UserManager ile gelen User'ları View'de görüntüleme işlemi
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new IdentityUser {UserName = model.UserName, Email = model.Email };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(model);
        }

        public  async Task<IActionResult> Edit(String Id)
        {
            if(Id == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(Id);

            if(user != null)
            {
                // Veri tabanına kayıtlı tüm rolleri viewbag ile sayfaya taşıma
                ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();

                return View(new EditViewModel{
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    SelectedRoles = await _userManager.GetRolesAsync(user) // user'a ilişkin seçilmiş rolü sayfaya çağırma
                });

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit (string Id, EditViewModel model)
        {
            if(Id != model.Id)
            {
               return RedirectToAction("Index"); 
            }

            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if(user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.UserName;

                    var result = await _userManager.UpdateAsync(user);

                    // Kullanıcı parola alanında veri giriş yapmışsa varolan paralayı silip modelden gelen yeni parolayı ekleyelim

                    if(result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user); // var olan parolayı siler

                        await _userManager.AddPasswordAsync(user, model.Password); // user'a modelden gelen parolayı ekler
                    }

                    // Update metodu çalıştı mı kontrol edelim
                    if(result.Succeeded)
                    {
                        // Var olan rolleri silme işlemi
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                        // modelden gönderilen rolleri ekleme işlemi

                        if(model.SelectedRoles != null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        }

                        return RedirectToAction("Index");
                    }

                    //Güncelleme sırasında oluşcak hatalar için 
                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description); // Burada "" bir key ile ilişklendirme için kullanılır. 
                    }

                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if(user != null)
            {
                await _userManager.DeleteAsync(user);
            }

           return RedirectToAction("Index");
        }

    }
}