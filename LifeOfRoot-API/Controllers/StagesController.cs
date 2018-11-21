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
    public class StagesController : ControllerBase
    {
        private DataContext _context;
        IAmazonS3 S3Client { get; set; }
        IAmazonRekognition RekognitionClient { get; set; }        

        public StagesController(DataContext context, IAmazonS3 s3Client, IAmazonRekognition rekognitionClient)
        {
            _context = context;
            this.S3Client = s3Client;
            this.RekognitionClient = rekognitionClient;            
        }

        [HttpGet]
        public async Task<IActionResult> GetStageInfo([FromQuery(Name="game_id")] int gameId, [FromQuery(Name="stage_id")] int stageId)
        {
            StageInfoDTO stageInfo = new StageInfoDTO();

            if (gameId == 0 || stageId == 0)
                return BadRequest("Invalid Parameter. Please check game_id and stage_id parameter values.");

            stageInfo.game_id = gameId;
            stageInfo.stage_id = stageId;

            // set gaming rule
            int difficulty = 0;
            int objectCount = 0;
            int objectScore = 0;
            switch (stageId)
            {
                case 1:
                    stageInfo.stage_time = 60;
                    stageInfo.stage_difficulty = "Easy";
                    difficulty = 1;
                    objectCount = 3;
                    objectScore = 50;
                    break;
                case 2:
                    stageInfo.stage_time = 80;
                    stageInfo.stage_difficulty = "Medium";
                    difficulty = 2;
                    objectCount = 5;
                    objectScore = 100;
                    break;
                case 3:
                    stageInfo.stage_time = 100;
                    stageInfo.stage_difficulty = "Hard";
                    difficulty = 2;
                    objectCount = 10;
                    objectScore = 100;
                    break;
                default:
                    // need exception handling logic for bad stageId
                    break;
            }
            
            // get object list randomly
            List<string> objectList = GetRandomStageObjectList(difficulty, objectCount);
            stageInfo.stage_objects = objectList;

            // Add object list to StageObject table
            List<StageObject> stageObjectList = new List<StageObject>();
            foreach (string item in objectList)
            {
                StageObject stageObject = new StageObject()
                {
                    game_id = gameId,
                    stage_id = stageId,
                    object_name = item,
                    object_score = objectScore,
                    found_yn = "N",
                    log_date = DateTime.Now
                };
                stageObjectList.Add(stageObject);
                _context.StageObject.Add(stageObject);
            }
            await _context.SaveChangesAsync();

            return Ok(stageInfo);
        }
    
        private List<string> GetRandomStageObjectList(int difficulty, int objectCount)
        {
            List<string> objectList = new List<String>();

            int recordCount = _context.Object.Where(x => x.difficulty == difficulty).Count();
            var records = _context.Object.Where(x => x.difficulty == difficulty);

            for (int i = 0; i < objectCount && i < recordCount; i++)
            {
                bool loop = true;
                while (loop)
                {
                    int randomRecord = new Random().Next() % recordCount;
                    var record = records.Skip(randomRecord).Take(1).First();

                    if (objectList.Contains(record.object_name) == false)
                    {
                        objectList.Add(record.object_name);
                        loop = false;
                    }
                }
            }

            return objectList;
        }   
    }
}