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
    [Route("AssessmentStage")]
    public class AssessmentStagesController : BaseController
    {
        public AssessmentStagesController(ILoggerManager logger
            , IUnitOfWork uow
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<AssessmentStageSearch>(jsonData);
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            obj.DomainId = userInfo.DomainId;
            var lst = _uow.AssessmentStages.Search(obj, out int count).Select(o => _mapper.Map<AssessmentStageDto>(o)).ToList();

            return Ok(new { code = "001", msg = "success", data = lst, total = count });
        }
        [HttpPost]
        public IActionResult UpdateItem(AssessmentStageDto obj)
        {
            var current_id = obj.ID;
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            if (!obj.IsValid())
                return Ok(new { code = "301", msg = "success", data = obj });

            var hasConfig = _uow.AssessmentStages.FirstOrDefault(o => !o.Deleted && o.DomainId == userInfo.DomainId
            && o.ID != obj.ID && o.Year == obj.Year && ((obj.Stage != 3 && o.Stage == obj.Stage) || (obj.Stage == 3 && o.StageValue == obj.StageValue)));
            if (hasConfig != null)
                return Ok(new { code = "302", msg = "success", data = obj });

            if (obj.StageState == 1)
                return Ok(new { code = "103", msg = "success", data = obj });

            if (obj.ID > 0)
            {
                var ex = _uow.AssessmentStages.FirstOrDefault(o => o.ID == obj.ID);
                if (!_uow.AssessmentStages.CheckPermission(ex, userInfo.DomainId))
                    return Forbid();
                obj.Status = ex.Status;

                ex = obj.Map(ex);
                ex.Year = obj.Year;
                ex.Stage = obj.Stage;
                ex.StageValue = obj.StageValue;
                ex.StageState = obj.StageState;
                ex.DomainId = userInfo.DomainId;
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                _uow.AssessmentStages.Update(ex);
                obj = _mapper.Map<AssessmentStageDto>(ex);
            }
            else
            {
                var item = _mapper.Map<AssessmentStage>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                item.Status = true;
                _uow.AssessmentStages.Add(item);
                obj = _mapper.Map<AssessmentStageDto>(item);
            }

            return Ok(new { code = "001", msg = "success", data = obj, id = current_id });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(AssessmentStageDto obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var item = _uow.AssessmentStages.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            ////if (item.UserCreate != userInfo.Id)
            ////{
            ////    _logger.LogError($"{userInfo.UserName} delete 'Assessment Stage' {item.ID}: don't have permission!");
            ////    return BadRequest();
            ////}
            if (item.StageState == 1)
                return Ok(new { code = "303", msg = "success", data = obj });
            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            item.DomainId = userInfo.DomainId;
            _uow.AssessmentStages.Update(item);
            obj.Status = false;

            var scoreBoard = _uow.ScoreBoard.Find(o => o.AssessmentStageId == item.ID).ToList();
            if (scoreBoard.Any())
            {
                scoreBoard.ForEach(o =>
                {
                    o.Deleted = true;
                    o.ModifiedBy = userInfo.Id;
                    o.LastModified = DateTime.Now;
                    _uow.ScoreBoard.Update(o);
                });
            }

            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var item = _uow.AssessmentStages.FirstOrDefault(o => o.ID == id);
            if (item == null || !_uow.AssessmentStages.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var result = _mapper.Map<AssessmentStageDto>(item);
            if (result.UserCreate != null)
            {
                result.NameCreate = _uow.SystemCategories.GetUserFullName(result.UserCreate.Value);
            }
            if (result.ModifiedBy != null)
            {
                result.NameModified = _uow.SystemCategories.GetUserFullName(result.ModifiedBy.Value);
            }
            if (result.CreateDate.HasValue)
                result.CreateDateStr = result.CreateDate.Value.ToString("dd/MM/yyyy HH:mm:ss");
            if (result.LastModified.HasValue)
                result.LastModifiedStr = result.LastModified.Value.ToString("dd/MM/yyyy HH:mm:ss");
            return Ok(new { code = "001", msg = "success", data = result });
        }

    }
}
