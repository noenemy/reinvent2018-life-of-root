using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using GotTalent_API.Data;
using GotTalent_API.DTOs;
using GotTalent_API.Models;
using GotTalent_API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GotTalent_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StageLogsController : ControllerBase
    {
        private DataContext _context;
        IAmazonS3 S3Client { get; set; }
        IAmazonRekognition RekognitionClient { get; set; }        

        public StageLogsController(DataContext context, IAmazonS3 s3Client, IAmazonRekognition rekognitionClient)
        {
            _context = context;
            this.S3Client = s3Client;
            this.RekognitionClient = rekognitionClient;            
        }

        // GET api/stagelogs
        [HttpGet]
        public async Task<IActionResult> GetStageLogs()
        {
            var values = await _context.StageLog.ToListAsync();
            return Ok(values);
        }

        // GET api/stagelogs/5
        [HttpGet("{game_id}")]
        public async Task<IActionResult> GetStageLog(int game_id)
        {
            var values = await _context.StageLog.Where(x => x.game_id == game_id).ToListAsync();
            return Ok(values);
        }

        // POST api/stagelogs
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StageLogPostDTO dto)
        {
            Console.WriteLine("stagelogs POST entered.");

            // Add a stage log record
            StageLog newStageLog = new StageLog{
                    game_id = dto.game_id,
                    stage_id = dto.stage_id,
                    objects_score = 0,
                    time_score = 0,
                    clear_score = 0,
                    stage_score = 0,
                    total_score = 0,
                    completed_yn = "N",
                    start_date = DateTime.Now 
                };
            var value = _context.StageLog.Add(newStageLog);
            await _context.SaveChangesAsync();  

            return Ok(dto.game_id);            
        }

        // PUT api/stagelogs/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] StageLogPutDTO dto)
        {
            Console.WriteLine("stagelogs PUT entered.");

            var stageLog = _context.StageLog.Where(x => x.game_id == dto.game_id && x.stage_id == dto.stage_id).FirstOrDefault();
            if (stageLog == null)
            {
                return NotFound();
            }

            stageLog.objects_score = dto.objects_score;
            stageLog.time_score = dto.time_score;
            stageLog.clear_score = dto.clear_score;
            stageLog.stage_score = dto.stage_score;
            stageLog.total_score = dto.total_score;
            stageLog.completed_yn = dto.completed_yn;
            stageLog.end_date = DateTime.Now;

            var value = _context.StageLog.Update(stageLog);
            await _context.SaveChangesAsync();  

            return Ok(dto.game_id);     
        }

        // DELETE api/stagelogs/5
        [HttpDelete("{game_id}")]
        public void Delete(int id)
        {
        }       
    }
}