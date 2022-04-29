using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using RiskAssessment.Api.Attributes;
using Microsoft.Extensions.Configuration;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("Common")]
    public class CommonController : BaseController
    {
        public CommonController(ILoggerManager logger
            , IUnitOfWork uow
            , IDatabase redisDb
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
        }
        [HttpPost]
        //[BaseAuthorize("", "Admin")]
        public IActionResult UpdateItem(SystemCategoryDto obj)
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
                var ex = _uow.SystemCategories.FirstOrDefault(o => o.ID == obj.ID);
                ex = obj.Map(ex);
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                _uow.SystemCategories.Update(ex);
                obj = _mapper.Map<SystemCategoryDto>(ex);
            }
            else
            {
                var item = _mapper.Map<SystemCategory>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                _uow.SystemCategories.Add(item);
                obj = _mapper.Map<SystemCategoryDto>(item);
            }

            return Ok(new { code = "001", msg = "success", data = obj });
        }
        [HttpGet]
        public IActionResult GetCategories(string gr)
        {
            var lst = _uow.SystemCategories.Find(o => o.ParentGroup == gr && o.Status && !o.Deleted).ToList();
            var dtos = lst.Select(o => _mapper.Map<SystemCategoryDto>(o)).ToList();
            dtos.ForEach(o => { o.UserCreate = null; o.ModifiedBy = null; });
            return Ok(new { code = "001", msg = "success", data = dtos, total = dtos.Count });
        }
        [HttpGet("GetCategoriesWithSort")]
        public IActionResult GetCategoriesWithSort(string gr)
        {
            var lst = _uow.SystemCategories.Find(o => o.ParentGroup == gr && o.Status && !o.Deleted).ToList();
            var dtos = lst.Select(o => _mapper.Map<SystemCategoryDto>(o)).OrderBy(a=>a.Code).ToList();
            dtos.ForEach(o => { o.UserCreate = null; o.ModifiedBy = null; });
            return Ok(new { code = "001", msg = "success", data = dtos, total = dtos.Count });
        }
        [HttpGet("RiskLevel")]
        public IActionResult GetRiskLevel()
        {
            var lst = _uow.SystemCategories.GetRiskLevelCategory().ToList();
            return Ok(new { code = "001", msg = "success", data = lst, total = lst.Count });
        }
        [HttpGet("Users")]
        public IActionResult GetUsers()
        {
            var lst = _uow.SystemCategories.GetUsersActive().ToList();
            return Ok(new { code = "001", msg = "success", data = lst, total = lst.Count });
        }
        [HttpGet("UnitTypes")]
        public IActionResult GetUnitTypes()
        {
            var lst = _uow.SystemCategories.GetUnitTypes().ToList();
            return Ok(new { code = "001", msg = "success", data = lst, total = lst.Count });
        }
    }
}
