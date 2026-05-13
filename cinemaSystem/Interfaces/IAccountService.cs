using cinemaSystem.Models;
using cinemaSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cinemaSystem.Interfaces
{
    public interface IAccountService
    {
        bool IsLogined(ClaimsPrincipal User);
        Task SendMailAsync(ApplicationUser user, IUrlHelper url, HttpRequest request, EmailType emailType = EmailType.Register);
    }
}