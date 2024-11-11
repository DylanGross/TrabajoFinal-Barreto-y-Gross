using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly TicketsDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration, 
        TicketsDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }

    // Registro de usuarios
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "El usuario ya existe" });
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al crear usuario" });
        }

        return Ok(new { Message = "Usuario creado satisfactoriamente" });
    }

    // Login de usuarios y generación de JWT
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    [HttpPost("crear-rol")]
    public async Task<IActionResult> CrearRol([FromBody] string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return BadRequest(new { Message = "El rol ya existe" });
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

        if (result.Succeeded)
        {
            return Ok(new { Message = $"El rol '{roleName}' ha sido creado exitosamente." });
        }

        return BadRequest(new { Message = "Error al crear el rol", Errors = result.Errors });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(){
        List<ApplicationUser> users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("/users/{id}/roles")]
    public async Task<IActionResult> GetRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        return BadRequest();
    }
    // Obtener tareas asociadas al usuario
    [HttpGet("users/{id}/tareas")]
    public async Task<IActionResult> GetTareasByUsuario(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Message = "Usuario no encontrado" });
        }

        var tareas = await _context.Tareas
            .Where(t => t.UsuarioId == id)
            .ToListAsync();

        return Ok(tareas);
    }

    // Asignar un rol a un usuario
    [HttpPost("asignar-rol")]
    public async Task<IActionResult> AsignarRol([FromBody] RoleAssignmentDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return NotFound(new { Message = "Usuario no encontrado" });
        }

        var roleExists = await _userManager.IsInRoleAsync(user, model.Role);
        if (roleExists)
        {
            return BadRequest(new { Message = "El usuario ya tiene este rol" });
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Rol asignado correctamente" });
        }

        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al asignar el rol" });
    }
}
