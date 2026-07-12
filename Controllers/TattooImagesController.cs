
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xema.Models;
using xema.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class TattooImagesController : Controller
{
    private readonly ApplicationDbContext _context;

    public TattooImagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TATTOOIMAGES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.TattooImages
                                .Include(t => t.Category)
                                .ToListAsync());
    }

    // GET: TATTOOIMAGES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages
            .Include(t => t.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (tattooimage == null)
        {
            return NotFound();
        }

        return View(tattooimage);
    }

    // GET: TATTOOIMAGES/Create
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(
            _context.Categories,
            "Id",
            "Name");

        return View();
    }

    // POST: TATTOOIMAGES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,ImageUrl,CategoryId")] TattooImage tattooimage)
    {
        if (ModelState.IsValid)
        {
            tattooimage.UploadedAt = DateTime.UtcNow;

            _context.Add(tattooimage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = new SelectList(
            _context.Categories,
            "Id",
            "Name",
            tattooimage.CategoryId);
        return View(tattooimage);
    }

    // GET: TATTOOIMAGES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages.FindAsync(id);
        if (tattooimage == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(
            _context.Categories,
            "Id",
            "Name",
            tattooimage.CategoryId);
        return View(tattooimage);
    }

    // POST: TATTOOIMAGES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("Id,Title,Description,ImageUrl,CategoryId")] TattooImage tattooimage)
    {
        if (id != tattooimage.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                tattooimage.CategoryId);

            return View(tattooimage);
        }

        var existingTattoo = await _context.TattooImages.FindAsync(id);

        if (existingTattoo == null)
        {
            return NotFound();
        }

        existingTattoo.Title = tattooimage.Title;
        existingTattoo.Description = tattooimage.Description;
        existingTattoo.ImageUrl = tattooimage.ImageUrl;
        existingTattoo.CategoryId = tattooimage.CategoryId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TattooImageExists(tattooimage.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: TATTOOIMAGES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages
            .Include(t => t.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (tattooimage == null)
        {
            return NotFound();
        }

        return View(tattooimage);
    }

    // POST: TATTOOIMAGES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var tattooimage = await _context.TattooImages.FindAsync(id);
        if (tattooimage != null)
        {
            _context.TattooImages.Remove(tattooimage);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TattooImageExists(int? id)
    {
        return _context.TattooImages.Any(e => e.Id == id);
    }
}
