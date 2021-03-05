using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers {
    [Route(""), ApiController]
    public class CelestialObjectController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context) {
        _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int Id) {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(item => item.Id == Id);
            if (celestialObject == null) {
                return NotFound();
            }
            celestialObject.Satellites = _context.CelestialObjects.Where<CelestialObject>(item => item.OrbitedObjectId == celestialObject.Id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name) {
            var celestialObjects = _context.CelestialObjects.Where<CelestialObject>(item => item.Name == name).ToList();
            if (celestialObjects.Count == 0) {
                return NotFound();
            }
            for (int i = 0; i < celestialObjects.Count; ++i) {
                celestialObjects[i].Satellites = _context.CelestialObjects.Where<CelestialObject>(item => item.OrbitedObjectId == celestialObjects[i].Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll() {
            var celestialObjects = _context.CelestialObjects.ToList();
            for (int i = 0; i < celestialObjects.Count; ++i) {
                celestialObjects[i].Satellites = _context.CelestialObjects.Where<CelestialObject>(item => item.OrbitedObjectId == celestialObjects[i].Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject) {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new{celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject) {
            var check = _context.CelestialObjects.FirstOrDefault(item => item.Id == id);
            if (check == null) {
                return NotFound();
            }
            check.Name = celestialObject.Name;
            check.OrbitalPeriod = celestialObject.OrbitalPeriod;
            check.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(check);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name) {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(item => item.Id == id);
            if (celestialObject == null) {
                return NotFound();
            }
            celestialObject.Name = name;
            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            var celestialObjects = _context.CelestialObjects.Where<CelestialObject>(item => item.Id == id || item.OrbitedObjectId == id).ToList();
            if (celestialObjects.Count == 0) {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}