using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ASG_Leaderboard_Project.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly MongoRepository _repo;

        public SimulationController(MongoRepository repo)
        {
            _repo = repo;
        }

        // ------------ SIMULATION FUNCTIONS----------------------
    }
}