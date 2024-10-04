using DeployMe.Domain;
using DeployMe.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DeployMe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly StudentDbContext _studentDbContext;

        public StudentController(ILogger<StudentController> logger, StudentDbContext studentDbContext)
        {
            _logger = logger;
            _studentDbContext = studentDbContext;
        }

        [HttpGet(Name = "GetStudents")]
        public IEnumerable<Student> Get()
        {
            return _studentDbContext.Student.ToArray();
        }
    }
}
