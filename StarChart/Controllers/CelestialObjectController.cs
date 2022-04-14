using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var founded = _context.CelestialObjects.Find(id);
            if (founded == null)
            {
                return NotFound();
            }
            founded.Satellites = GetSatellites(founded);
            return Ok(founded);
        }
        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var founded = _context.CelestialObjects
                .Where(c => c.Name == name)
                .ToList();
            if (founded.Count() == 0)
            {
                return NotFound();
            }
            founded.ForEach(c => c.Satellites = GetSatellites(c));
            return Ok(founded);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var founded = _context.CelestialObjects
                .ToList();
            founded.ForEach(c => c.Satellites = GetSatellites(c));

            return Ok(founded);
        }

        private List<CelestialObject> GetSatellites(CelestialObject parent)
        {
            return _context.CelestialObjects.Where(c => c.OrbitedObjectId == parent.Id).ToList();
            
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject model)
        {
            _context.CelestialObjects.Add(model);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {id=model.Id}, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject model)
        {
            var old = _context.CelestialObjects.Find(id);
            if (old == null)
            {
                return NotFound();
            }
            old.OrbitalPeriod=old.OrbitalPeriod;
            old.Name=model.Name;
            old.OrbitedObjectId = model.OrbitedObjectId;
            //_context.Update(old);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {

            var old = _context.CelestialObjects.Find(id);
            if (old == null)
            {
                return NotFound();
            }
            old.Name = name;
            _context.Update(old);
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var old = _context.CelestialObjects.Find(id);
            if (old == null)
            {
                return NotFound();
            }

            List<CelestialObject> forDeleting = 
                _context.CelestialObjects
                .Where(c=>c.OrbitedObjectId==id)
                .ToList();
            forDeleting.Add(old);

            _context.RemoveRange(forDeleting);
            _context.SaveChanges();
            return NoContent();

        }
    }
}
