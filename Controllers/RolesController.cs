using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
     [Authorize(Roles ="Admin")]
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

        public async Task<IActionResult> Edit(string Id)
        {   
            var role = await _roleManager.FindByIdAsync(Id);

            if(Id != null)
            {
                return View(role);
            }

            return RedirectToAction("Index");
        } 

        [HttpPost]
        public async Task<IActionResult> Edit(string Id, IdentityRole model)
        {
            if(ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);

                if( role != null)
                {
                    role.Name = model.Name;
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {                    
                        return RedirectToAction("Index");         
                    }
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }   
            }
             return View(model);   
        }  

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id != null)
            {
                var record = await _roleManager.FindByIdAsync(Id);

                if(record != null)
                {
                    await _roleManager.DeleteAsync(record);
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        } 

    }
}