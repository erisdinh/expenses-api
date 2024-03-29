﻿using ExpensesAPI.Data;
using ExpensesAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ExpensesAPI.Controllers
{
    [EnableCors("http://localhost:4200", "*", "*")]
    [RoutePrefix("auth")] // Set a new route for a shorter authentication url
                          // instead of (localhost/API/authentication) -> (localhost/API/auth)
    public class AuthenticationController : ApiController
    {
        [Route("login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] User user) {
            try
            {
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password)) { return BadRequest("Enter your User Name and Password"); }

                try
                {
                    using (var context = new AppDbContext()) {
                        var exists = context.Users.Any(n => n.UserName.Equals(user.UserName) && n.Password.Equals(user.Password));

                        if (exists) return Ok(CreateToken(user));

                        return BadRequest("Wrong Credentials");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [Route("Register")]
        [HttpPost]
        public IHttpActionResult Register([FromBody] User user) {
            try
            {
                using (var context = new AppDbContext()) {
                    var exists = context.Users.Any(n => n.UserName == user.UserName);

                    if (exists) return BadRequest("User already exists");

                    context.Users.Add(user);
                    context.SaveChanges();

                    return Ok(CreateToken(user));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Create a token for the user
        private JwtPackage CreateToken(User user) {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Email, user.UserName)
            });

            const string secretKey = "secretkey goes here";

            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken)tokenHandler.CreateJwtSecurityToken(
                    subject: claims, 
                    signingCredentials: signingCredentials
                );

            var tokenString = tokenHandler.WriteToken(token);

            return new JwtPackage() {
                UserName = user.UserName,
                Token = tokenString
            };
        }
    }
}

public class JwtPackage {
    public string Token { get; set; }
    public string UserName { get; set; }
}
