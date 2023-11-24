using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
    public class RolesController:Controller
    {
        //Sayfa içinde UserRole'ı kullanmak için gerekli injection işlemi
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View(_roleManager.Roles);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if(ModelState.IsValid)
            {
                // var role =  new IdentityRole {Name=model.Name};  Aşağdaki kod ile doğrudan da eklenebilir.
                var result = await _roleManager.CreateAsync(model);

                if (result.Succeeded)
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




    }
}