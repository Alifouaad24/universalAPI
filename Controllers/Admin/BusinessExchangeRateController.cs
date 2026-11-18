using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessExchangeRateController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public BusinessExchangeRateController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExchangeRates(int id)
        {
            var exchangeRates = await _context.BusinessExchangeRates
                .Include(e => e.ExchangrRate).ThenInclude(er => er.CurrencySource)
                .Include(e => e.ExchangrRate).ThenInclude(er => er.CurrencyTarget)
                .Where(bx => bx.Business_id == id)
                .ToListAsync();
            return Ok(exchangeRates);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExchangeRate([FromBody] BusinessExchangeRateDto exchangeRate)
        {
            // [جديد] تسعيرة واحدة فقط لكل بزنس
            var alreadyExists = await _context.BusinessExchangeRates
                .AnyAsync(b => b.Business_id == exchangeRate.Business_id);
            if (alreadyExists)
            {
                return Conflict("هذا النشاط لديه تسعيرة بالفعل، يمكنك تعديلها فقط");
            }

            int exRate;

            var xeRate = await _context.ExchangrRates
                .FirstOrDefaultAsync(x => x.CurrencySourceId == exchangeRate.SourceCurrencyId
                                       && x.CurrencyTargetId == exchangeRate.TargetCurrencyId);

            if (xeRate == null)
            {
                var newExchangeRate = new ExchangrRate
                {
                    CurrencySourceId = exchangeRate.SourceCurrencyId,
                    CurrencyTargetId = exchangeRate.TargetCurrencyId
                };

                _context.ExchangrRates.Add(newExchangeRate);
                await _context.SaveChangesAsync();
                exRate = newExchangeRate.ExchangrRateId;
            }
            else
            {
                exRate = xeRate.ExchangrRateId;
            }

            var exchangeRateEntity = new BusinessExchangeRate
            {
                Business_id = exchangeRate.Business_id,
                ExchangrRateId = exRate,
                Rate = exchangeRate.Rate
            };

            // [تصحيح] هاد السطر كان ناقص، فما كان ينحفظ شي بالجدول
            _context.BusinessExchangeRates.Add(exchangeRateEntity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                exchangeRateEntity.BusinessExchangeRateId,
                exchangeRateEntity.Business_id,
                exchangeRateEntity.ExchangrRateId,
                exchangeRateEntity.Rate
            });
        }

        // [تصحيح] كان ناقص الـ attribute، فالـ endpoint ما كان مربوط بـ PUT
        // وكان ممكن يتعارض مع الـ POST على نفس المسار
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExchangeRate(int id, [FromBody] BusinessExchangeRateDto exchangeRate)
        {
            var existingExchangeRate = await _context.BusinessExchangeRates.FindAsync(id);
            if (existingExchangeRate == null)
            {
                return NotFound();
            }
            existingExchangeRate.Rate = exchangeRate.Rate;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                existingExchangeRate.BusinessExchangeRateId,
                existingExchangeRate.Business_id,
                existingExchangeRate.ExchangrRateId,
                existingExchangeRate.Rate
            });
        }
    }
}