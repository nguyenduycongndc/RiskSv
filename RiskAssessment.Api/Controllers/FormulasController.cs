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
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("Formula")]
    public class FormulasController : BaseController
    {
        public FormulasController(ILoggerManager logger
            , IUnitOfWork uow
            , IDatabase redisDb
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<ModelSearch>(jsonData);
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            obj.DomainId = userInfo.DomainId;
            var lst = _uow.Formulas.Search(obj, out int count).ToList().Select(o => _mapper.Map<FormulaDto>(o)).ToList();

            return Ok(new { code = "1", msg = "success", data = lst, total = count });
        }
        [HttpPost]
        public IActionResult UpdateItem(FormulaDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            if (obj.ID > 0)
            {
                var ex = _uow.Formulas.FirstOrDefault(o => o.ID == obj.ID);
                ex = obj.Map(ex);
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                _uow.Formulas.Update(ex);
                obj = _mapper.Map<FormulaDto>(ex);
            }
            else
            {
                var item = _mapper.Map<Formula>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                _uow.Formulas.Add(item);
                obj = _mapper.Map<FormulaDto>(item);
            }

            return Ok(new { code = "1", msg = "success", data = obj });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(FormulaDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            var item = _uow.Formulas.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            if (item.UserCreate != userInfo.Id)
            {
                _logger.LogError($"{userInfo.UserName} delete 'formula' {item.ID}: don't have permission!");
                return BadRequest();
            }
            item.Status = false;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            item.DomainId = userInfo.DomainId;
            _uow.Formulas.Update(item);
            obj.Status = false;
            return Ok(new { code = "1", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(FormulaDto obj)
        {
            var item = _uow.Formulas.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            var map = _mapper.Map<FormulaDto>(item);
            return Ok(new { code = "1", msg = "success", data = map });
        }

    }
}
