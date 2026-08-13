using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace App.Api.Models.Configuration;

/// <summary>
/// Defines the configuration values necessary for configuring worker auth.
/// </summary>
public class WorkerAuthConfiguration : AuthenticationSchemeOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Token { get; set; } = string.Empty;
}