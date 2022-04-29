using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.DAO;
using StackExchange.Redis;
using RiskAssessment.Api.Common;
using RiskAssessment.Entity.DTO;
using AutoMapper;
using RiskAssessment.Api.Validator;
using RiskAssessment.Api.Models;
using RiskAssessment.Entity.CommonModel;
using Microsoft.Extensions.Configuration;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("AuditActivity")]
    public class AuditActivitiesController : BaseController
    {
        public AuditActivitiesController(ILoggerManager logger
            , IUnitOfWork uow
            , IDatabase redisDb
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
        }
        [HttpGet("Search")]
        public IActionResult Search(ModelSearch obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            var lst = _uow.AuditActivities.Search(obj, out int count).ToList().Select(o => _mapper.Map<AuditActivityDto>(o)).ToList();

            return Ok(new { code = "1", msg = "success", data = lst, total = count });
        }

        [HttpPost]
        public IActionResult UpdateItem(AuditActivityDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid name!" });

            if (obj.ID > 0)
            {
                var ex = _uow.AuditActivities.FirstOrDefault(o => o.ID == obj.ID);
                ex = obj.Map(ex);
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                ex.DomainId = userInfo.DomainId;
                _uow.AuditActivities.Update(ex);
                obj = _mapper.Map<AuditActivityDto>(ex);
            }
            else
            {
                var item = _mapper.Map<AuditActivity>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.AuditActivities.Add(item);
                obj = _mapper.Map<AuditActivityDto>(item);
            }

            return Ok(new { code = "1", msg = "success", data = obj });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(AuditActivityDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            var item = _uow.AuditActivities.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            if (item.UserCreate != userInfo.Id)
            {
                _logger.LogError($"{userInfo.UserName} delete 'business activity' {item.ID}: don't have permission!");
                return BadRequest();
            }
            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.AuditActivities.Update(item);
            obj.Status = false;
            return Ok(new { code = "1", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(AuditActivityDto obj)
        {
            var item = _uow.AuditActivities.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            var map = _mapper.Map<AuditActivityDto>(item);
            return Ok(new { code = "1", msg = "success", data = map });
        }

    }
}
