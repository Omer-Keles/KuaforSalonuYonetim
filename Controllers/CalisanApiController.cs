using Microsoft.AspNetCore.Mvc;
using KuaforSalonuYonetim.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CalisanApiController : ControllerBase
{
    private readonly KuaforContext _context;

    public CalisanApiController(KuaforContext context)
    {
        _context = context;
    }

    // GET: api/CalisanApi
    [HttpGet]
    public IActionResult GetCalisanlar()
    {
        var calisanlar = _context.Calisanlar
            .Select(c => new
            {
                c.CalisanId,
                c.CalisanAdi,
                c.CalisanSoyadi,
                c.Telefon
            })
            .ToList();

        return Ok(calisanlar);
    }
}