﻿using System;
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

        public PrivateController(IConfiguration configuration, IMemoryCache cache)
        {
            authorizationManager = new AuthorizationManager(configuration);
            this.cache = cache;
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

        private IActionResult ActionToBeTaken(CheckIfActionNeededRequest checkIfActionNeededRequest)
        {
            if (cache.TryGetValue(LastStateChangeRequest, out PcStateChangeCacheEntry cacheEntry))
            {
                var entryIsRecent = (DateTimeOffset.Now - cacheEntry.Date).TotalSeconds < 180;
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
                return BadRequest($"Too soon since last {pcStateChangeRequest.Action} request.");
            }
            else
            {
                cache.Set(LastStateChangeRequest, new PcStateChangeCacheEntry(pcStateChangeRequest.Action, DateTimeOffset.Now));
                return Ok(pcStateChangeRequest);
            }
        }

        private bool IsTooSoonAfterLastStateChangeRequest(PcStateChangeRequest pcStateChangeRequest)
        {
            if (cache.TryGetValue(LastStateChangeRequest, out PcStateChangeCacheEntry cacheEntry))
            {
                return (DateTimeOffset.Now - cacheEntry.Date).TotalSeconds < 180;
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
            authorizationManager.SignIn(body.AuthorizationDetails);
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
