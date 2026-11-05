
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using xema.Data;
using xema.Models;
using xema.Services;
using xema.ViewModels;

namespace xema.Areas.Admin.Controllers;


[Area("Admin")]
[Authorize(Roles = "Admin")]
public class GalleryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    private const int MaxFeaturedImages = 3;
    private const long MaxImageSize = 10 * 1024 * 1024;

    public GalleryController(
        ApplicationDbContext context,
        IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    // GET: Gallery
    public async Task<IActionResult> Index()    
    {
        return View(await _context.TattooImages
                                .Include(t => t.Media)
                                .Include(t => t.Category)
                                .ToListAsync());
    }

    // GET: Gallery/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages
            .Include(t => t.Media)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (tattooimage == null)
        {
            return NotFound();
        }

        return View(tattooimage);
    }

    // GET: Gallery/Create
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(
            _context.Categories,
            "Id",
            "Name");

        return View();
    }

    // POST: Gallery/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TattooFormViewModel model)
    {
        if (model.ImageFile == null)
        {
            ModelState.AddModelError(
                nameof(model.ImageFile),
                "Please select an image.");
        }
        else
        {
            await ValidateImage(model.ImageFile, ModelState);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                model.CategoryId);

            return View(model);
        }

        if (!await ValidateFeaturedImages(model.IsFeatured))
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                model.CategoryId);

            return View(model);
        }

        var fileName = GenerateFileName(
            model.ImageFile!);

        string mediaUrl;

        await using (var stream =
            model.ImageFile!.OpenReadStream())
        {
            mediaUrl = await _fileStorage.UploadAsync(
                stream,
                fileName,
                model.ImageFile.ContentType);
        }

        var media = new Media
        {
            Url = mediaUrl,
            AltText = model.Title,
            Type = MediaType.Image
        };

        _context.Media.Add(media);

        await _context.SaveChangesAsync();

        var tattoo = new TattooImage
        {
            Title = model.Title,
            Description = model.Description,
            UploadedAt = DateTime.UtcNow,
            CategoryId = model.CategoryId,
            IsFeatured = model.IsFeatured,
            MediaId = media.Id
        };

        _context.TattooImages.Add(tattoo);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Gallery/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages
            .Include(t => t.Media)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (tattooimage == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(
            _context.Categories,
            "Id",
            "Name",
            tattooimage.CategoryId);
        var vm = new TattooFormViewModel
        {
            Id = tattooimage.Id,
            Title = tattooimage.Title,
            Description = tattooimage.Description,
            CategoryId = tattooimage.CategoryId,
            IsFeatured = tattooimage.IsFeatured
        };
        return View(vm);
    }

    // POST: Gallery/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    TattooFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (model.ImageFile != null)
        {
            await ValidateImage(model.ImageFile, ModelState);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                model.CategoryId);

            return View(model);
        }

        var existingTattoo = await _context.TattooImages
            .Include(t => t.Media)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTattoo == null)
        {
            return NotFound();
        }

        if (!await ValidateFeaturedImages(
                model.IsFeatured,
                existingTattoo.Id))
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                model.CategoryId);

            return View(model);
        }

        string? oldUrl = null;

        existingTattoo.Title = model.Title;
        existingTattoo.Description = model.Description;
        existingTattoo.CategoryId = model.CategoryId;
        existingTattoo.IsFeatured = model.IsFeatured;
        existingTattoo.Media.AltText = model.Title;

        if (model.ImageFile != null)
        {
            oldUrl = existingTattoo.Media.Url;

            var fileName = GenerateFileName(model.ImageFile);

            string mediaUrl;

            await using (var stream =
                model.ImageFile.OpenReadStream())
            {
                mediaUrl = await _fileStorage.UploadAsync(
                    stream,
                    fileName,
                    model.ImageFile.ContentType);
            }

            existingTattoo.Media.Url = mediaUrl;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TattooImageExists(id))
            {
                return NotFound();
            }

            throw;
        }

        // Only delete the old image after the database update succeeds.
        if (oldUrl != null)
        {
            try
            {
                await _fileStorage.DeleteAsync(oldUrl);
            }
            catch (Exception)
            {
                TempData["WarningMessage"] =
                    "The changes were saved, but the old image could not be removed from storage.";
            }
        }

        return RedirectToAction(nameof(Index));
    }


    // GET: Gallery/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages
            .Include(t => t.Media)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (tattooimage == null)
        {
            return NotFound();
        }

        return View(tattooimage);
    }

    // POST: Gallery/Delete/5

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var tattooimage = await _context.TattooImages
            .Include(t => t.Media)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tattooimage == null)
        {
            return NotFound();
        }

        try
        {
            // Delete the actual image from R2 first.
            await _fileStorage.DeleteAsync(tattooimage.Media.Url);
        }
        catch (Exception)
        {
            // Keep the database records if R2 deletion fails.
            TempData["ErrorMessage"] =
                "The image could not be deleted from storage. Please try again.";

            return RedirectToAction(nameof(Index));
        }

        _context.TattooImages.Remove(tattooimage);
        _context.Media.Remove(tattooimage.Media);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    private async Task<bool> ValidateFeaturedImages(
        bool isFeatured,
        int? tattooId = null)
    {
        if (!isFeatured)
        {
            return true;
        }

        var featuredCount = await _context.TattooImages
            .CountAsync(t =>
                t.IsFeatured &&
                t.Id != tattooId);

        if (featuredCount >= MaxFeaturedImages)
        {
            ModelState.AddModelError(
                nameof(TattooFormViewModel.IsFeatured),
                $"You can only have {MaxFeaturedImages} featured images.");

            return false;
        }

        return true;
    }



    private static async Task<bool> ValidateImage(
        IFormFile file,
        ModelStateDictionary modelState)
    {
        var field = nameof(TattooFormViewModel.ImageFile);

        if (file.Length == 0)
        {
            modelState.AddModelError(field, "The uploaded file is empty.");
            return false;
        }

        if (file.Length > MaxImageSize)
        {
            modelState.AddModelError(field, "The image must be 10 MB or smaller.");
            return false;
        }

        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        var allowedTypes = new Dictionary<string, string>
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

        if (!allowedTypes.TryGetValue(extension, out var expectedType) ||
            !string.Equals(file.ContentType, expectedType,
                StringComparison.OrdinalIgnoreCase))
        {
            modelState.AddModelError(
                field,
                "Upload a JPG, PNG, or WebP image.");
            return false;
        }

        var header = new byte[12];

        await using var stream = file.OpenReadStream();

        var bytesRead = await stream.ReadAsync(header.AsMemory());

        var isJpeg = bytesRead >= 3 &&
                     header[0] == 0xFF &&
                     header[1] == 0xD8 &&
                     header[2] == 0xFF;

        var isPng = bytesRead >= 8 &&
                    header.Take(8).SequenceEqual(
                        new byte[] { 0x89, 0x50, 0x4E, 0x47,
                                 0x0D, 0x0A, 0x1A, 0x0A });

        var isWebp = bytesRead >= 12 &&
                     header.Take(4).SequenceEqual(
                         new byte[] { 0x52, 0x49, 0x46, 0x46 }) &&
                     header.Skip(8).Take(4).SequenceEqual(
                         new byte[] { 0x57, 0x45, 0x42, 0x50 });

        var signatureMatches = extension switch
        {
            ".jpg" or ".jpeg" => isJpeg,
            ".png" => isPng,
            ".webp" => isWebp,
            _ => false
        };

        if (!signatureMatches)
        {
            modelState.AddModelError(
                field,
                "The file does not appear to be a valid supported image.");
            return false;
        }

        return true;
    }

    private static string GenerateFileName(IFormFile file)
    {
        var extension = Path.GetExtension(
            file.FileName).ToLowerInvariant();

        return $"{Guid.NewGuid():N}{extension}";
    }
    private bool TattooImageExists(int? id)
    {
        return _context.TattooImages.Any(e => e.Id == id);
    }
}
