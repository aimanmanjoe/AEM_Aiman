using AEM_Aiman.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AEM_Aiman.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json;

namespace AEM_Aiman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformWellController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly HttpClient _httpClient;

        public PlatformWellController(DataContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;                   
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://test-demo.aemenersol.com/");

        }


        [HttpGet("GetPlatformWellActual")]
        public async Task<IActionResult> SyncData()
        {
            try
            {
                // Get bearer token
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                                          
                if(!string.IsNullOrEmpty(token))
                {                  
                    // Call the API to get data
                    HttpResponseMessage response = await _httpClient.GetAsync("http://test-demo.aemenersol.com/api/PlatformWell/GetPlatformWellActual");
                    response.EnsureSuccessStatusCode();

                    // Deserialize the response
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var platforms = JsonConvert.DeserializeObject<List<Platform>>(apiResponse);

                    // Process the data
                    foreach (var platform in platforms)
                    {
                        var existingPlatform = await _context.Platforms.FirstOrDefaultAsync(p => p.Id == platform.Id);

                        if (existingPlatform != null)
                        {
                            // Update existing platform
                            existingPlatform.UniqueName = platform.UniqueName;
                            existingPlatform.Latitude = platform.Latitude;
                            existingPlatform.Longitude = platform.Longitude;
                            existingPlatform.CreatedAt = platform.CreatedAt;
                            existingPlatform.UpdatedAt = platform.UpdatedAt;

                            _context.Platforms.Update(existingPlatform);

                            // Process wells
                            if (platform.Well != null)
                            {
                                foreach (var well in platform.Well)
                                {
                                    var existingWell = await _context.Wells.FirstOrDefaultAsync(w => w.Id == well.Id);

                                    if (existingWell != null)
                                    {
                                        // Update existing well
                                        existingWell.UniqueName = well.UniqueName;
                                        existingWell.Latitude = well.Latitude;
                                        existingWell.Longitude = well.Longitude;
                                        existingWell.CreatedAt = well.CreatedAt;
                                        existingWell.UpdatedAt = well.UpdatedAt;
                                        existingWell.PlatformId = well.PlatformId;

                                        _context.Wells.Update(existingWell);
                                    }
                                    else
                                    {
                                        // Add new well
                                        var newWell = new Well();
                                        newWell.Id = well.Id;
                                        newWell.UniqueName = well.UniqueName;
                                        newWell.Latitude = well.Latitude;
                                        newWell.Longitude = well.Longitude;
                                        newWell.CreatedAt = well.CreatedAt;
                                        newWell.UpdatedAt = well.UpdatedAt;                                      
                                        newWell.PlatformId = well.PlatformId;

                                        _context.Wells.Add(newWell);
                                        //newPlatform.Well.Add(newWell);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Add new platform
                            var newPlatform = new Platform();   
                            newPlatform.Id = platform.Id;
                            newPlatform.UniqueName = platform.UniqueName;
                            newPlatform.Latitude = platform.Latitude;
                            newPlatform.Longitude = platform.Longitude;
                            newPlatform.CreatedAt = platform.CreatedAt;
                            newPlatform.UpdatedAt = platform.UpdatedAt;

                            // Process wells
                            if (platform.Well != null)
                            {
                                foreach (var well in platform.Well)
                                {
                                    var existingWell = await _context.Wells.FirstOrDefaultAsync(w => w.Id == well.Id);

                                    if (existingWell != null)
                                    {
                                        // Update existing well
                                        existingWell.UniqueName = well.UniqueName;
                                        existingWell.Latitude = well.Latitude;
                                        existingWell.Longitude = well.Longitude;
                                        existingWell.CreatedAt = well.CreatedAt;
                                        existingWell.UpdatedAt = well.UpdatedAt;
                                        existingWell.PlatformId = well.PlatformId;

                                        _context.Wells.Update(existingWell);
                                    }
                                    else
                                    {
                                        // Add new well
                                        var newWell = new Well();
                                        newWell.Id = well.Id;
                                        newWell.PlatformId = well.PlatformId;
                                        newWell.UniqueName = well.UniqueName;
                                        newWell.Latitude = well.Latitude;
                                        newWell.Longitude = well.Longitude;
                                        newWell.CreatedAt = well.CreatedAt;
                                        newWell.UpdatedAt = well.UpdatedAt;
                                        //newWell.Platform = well.Platform;
                                        
                                        _context.Wells.Add(newWell);
                                        //newPlatform.Well.Add(newWell);
                                    }
                                }
                            }
                            _context.Platforms.Add(newPlatform);

                        }

                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return Ok("Data synchronized successfully.");
                }
                else
                {
                    // Handle case where no token is provided
                    return Unauthorized("No access token provided.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
