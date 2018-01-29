using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using PersonalWebsite.Authorization;
using PersonalWebsite.Models;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace PersonalWebsite.Controllers
{
    [Produces("application/json")]
    [Route("api/Private")]
    public class PrivateController : Controller
    {
        private readonly AuthorizationManager authorizationManager;
        private readonly IMemoryCache cache;

        private const string LastStateChangeRequest = "last state change";
        private const string GifsCacheKey = "gifs";
        private const int MinimumWaitBetweenStateChangeRequests = 5;
        private const int ExpiryTimeOnStateChangeRequest = 180;

        public PrivateController(IConfiguration configuration, IMemoryCache cache)
        {
            authorizationManager = new AuthorizationManager(configuration);
            this.cache = cache;
        }

        [HttpGet]
        public IActionResult Private()
        {
            return View();
        }

        [Route("pcState")]
        [HttpPost]
        public IActionResult ChangePcStateIfAuthorized([FromBody] PcStateChangeRequest pcStateChangeRequest)
        {
            return ValidateThenAuthorizeThenExecute(pcStateChangeRequest, ChangePcState);
        }

        [Route("getPcState")]
        [HttpPost]
        public IActionResult GetLastStateChangeRequestIfAuthorized([FromBody] CheckIfActionNeededRequest checkIfActionNeededRequest)
        {
            return ValidateThenAuthorizeThenExecute(checkIfActionNeededRequest, ActionToBeTaken);
        }

        [Route("gifs")]
        [HttpPost]
        public IActionResult EmailGifMatchingTagsIfAuthorized([FromBody] GifRequest gifRequest)
        {
            return ValidateThenAuthorizeThenExecute(gifRequest, AddGifRequestToCache);
        }

        [Route("getGifs")]
        [HttpPost]
        public IActionResult GetGifRequests([FromBody] RequestForAllGifRequests requestForAllGifRequests)
        {
            if (cache.TryGetValue(GifsCacheKey, out IEnumerable<GifRequest> gifRequests))
            {
                lock (gifRequests)
                {
                    return Ok(gifRequests);
                }
            }
            else
            {
                return Ok();
            }
        }

        private IActionResult AddGifRequestToCache(GifRequest gifRequest)
        {
            if (cache.TryGetValue(GifsCacheKey, out IEnumerable<GifRequest> cacheEntry))
            {
                lock (cacheEntry)
                {
                    cacheEntry.Append(gifRequest);
                }
            }
            else
            {
                cache.Set(GifsCacheKey, new List<GifRequest>() { gifRequest });
            }
            return Ok();
        }

        private IActionResult ActionToBeTaken(CheckIfActionNeededRequest checkIfActionNeededRequest)
        {
            if (cache.TryGetValue(LastStateChangeRequest, out PcStateChangeCacheEntry cacheEntry))
            {
                var entryIsRecent = (DateTimeOffset.Now - cacheEntry.Date).TotalSeconds < ExpiryTimeOnStateChangeRequest;
                var entryIsRelevant = checkIfActionNeededRequest.Actions.Contains(cacheEntry.Action);

                if (entryIsRecent && entryIsRelevant)
                {
                    cache.Remove(LastStateChangeRequest);
                    return Ok(cacheEntry.Action);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return Ok();
            }
        }

        private IActionResult ChangePcState(PcStateChangeRequest pcStateChangeRequest)
        {
            if (IsTooSoonAfterLastStateChangeRequest(pcStateChangeRequest))
            {
                return StatusCode(429);
            }
            else
            {
                cache.Set(LastStateChangeRequest, new PcStateChangeCacheEntry(pcStateChangeRequest.Action, DateTimeOffset.Now));
                return Ok();
            }
        }

        private bool IsTooSoonAfterLastStateChangeRequest(PcStateChangeRequest pcStateChangeRequest)
        {
            if (cache.TryGetValue(LastStateChangeRequest, out PcStateChangeCacheEntry cacheEntry))
            {
                return (DateTimeOffset.Now - cacheEntry.Date).TotalSeconds < MinimumWaitBetweenStateChangeRequests;
            }
            else
            {
                return false;
            }
        }

        private IActionResult ValidateThenAuthorizeThenExecute<T>(T body, Func<T, IActionResult> action)
            where T : IPrivateApiRequest, IValidatableObject
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            authorizationManager.SignIn(body.Password);
            if (authorizationManager.Authorized)
            {
                return action(body);
            }
            else
            {
                return StatusCode(401);
            }
        }
    }
}
