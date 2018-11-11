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

            //var values = await _context.StageObject.Where(x => x.game_id == game_id && x.stage_id == stage_id).ToListAsync();
            if (gameId == 0 || stageId == 0)
                return BadRequest("Invalid Parameter. Please check game_id and stage_id parameter values.");

            stageInfo.game_id = gameId;
            stageInfo.stage_id = stageId;
            stageInfo.stage_time = 30;
            stageInfo.stage_difficulty = "Easy";

            List<String> stageObjects = new List<string>();
            stageObjects.Add("Tiger");
            stageObjects.Add("Apple");
            stageObjects.Add("Car");

            stageInfo.stage_objects = stageObjects;

            return Ok(stageInfo);
        }
    
        // POST api/stages
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StagePostImageDTO dto)
        {
            Console.WriteLine("PostImage entered.");

            StageScoreDTO stageScore = new StageScoreDTO();
            stageScore.game_id = dto.gameId;
            stageScore.stage_id = dto.stageId;

            string bucketName = "reinvent-lifeofroot";
            List<Label> labels = null;

            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            guidString = guidString.Replace("=","");
            guidString = guidString.Replace("+","");
            guidString = guidString.Replace("/","");

            // Retrieving image data
            // ex: test/guid.jpg
            string keyName = string.Format("test/{0}.jpg", guidString);
            byte[] imageByteArray = Convert.FromBase64String(dto.base64Image);
            if (imageByteArray.Length == 0)
                return BadRequest("Image length is 0.");

            bool found = false;
            int score = 0;
            using (MemoryStream ms = new MemoryStream(imageByteArray))
            {
                // call Rekonition API
                labels = await RekognitionUtil.GetObjectDetailFromStream(this.RekognitionClient, ms);   

                if (labels.Where(x => x.Name == "Car").Count() > 0)
                {
                    stageScore.object_name = "Car";
                    stageScore.object_score = 100;
                    stageScore.stage_score = 100;
                }
            }
            
            return Ok(stageScore);            
        }     
    }
}