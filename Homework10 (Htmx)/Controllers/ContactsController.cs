using Homework10__Htmx_.Models;
using Homework10__Htmx_.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework10__Htmx_.Controllers;

public class ContactsController(ContactsAppContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var contacts = await dbContext
            .Contacts
            .OrderByDescending(c => c.Name)
            .ToListAsync();
        
        ViewData["ContactCount"] = contacts.Count;
        
        return View(contacts.AsReadOnly());
    }

    public async Task<IActionResult> Search(string? query)
    {
        IQueryable<Contact> contacts = dbContext.Contacts;

        if (query is not null) 
            contacts = contacts.Where(c => EF.Functions.Like(c.Name, $"%{query}%"));
        
        var result = await contacts
            .OrderByDescending(c => c.Name)
            .ToListAsync();
        
        return PartialView("_ContactRows", result.AsReadOnly());
    }
    
    [HttpGet("Contacts/{id:int}/Row")]
    public async Task<IActionResult> GetRow(int id)
    {
        var contact = await dbContext.Contacts.FindAsync(id);
        if (contact == null) return NotFound();

        return PartialView("_ContactRows", new List<Contact> { contact }.AsReadOnly());
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Contact contact)
    {
        if (!ModelState.IsValid) return UnprocessableEntity();

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync();
        
        var count = await dbContext.Contacts.CountAsync();
        
        return PartialView("_CreateContactResponse", (contact, count));
    }

    [HttpDelete("/Contacts/{id:int}/Delete")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();

        await dbContext.Contacts
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
        
        await dbContext.SaveChangesAsync();
        var count = await dbContext.Contacts.CountAsync();

        return PartialView("_DeleteContactResponse", count);
    }
    
    [HttpGet("/Contacts/{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var contact = await dbContext.Contacts.FindAsync(id);
        
        if (contact == null) return NotFound();

        return PartialView("_ContactEditRow", contact);
    }

    [HttpPut("/Contacts/{id:int}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] Contact updatedContact)
    {
        var contact = await dbContext.Contacts.FindAsync(id);
        
        if (contact == null) return NotFound();

        contact.Name = updatedContact.Name;
        contact.Email = updatedContact.Email;
        contact.Phone = updatedContact.Phone;
    
        await dbContext.SaveChangesAsync();

        return PartialView("_ContactRows", new List<Contact> { contact }.AsReadOnly());
    }
}
