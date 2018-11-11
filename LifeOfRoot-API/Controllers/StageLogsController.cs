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
        public async Task<IActionResult> Post([FromBody] StagePostImageDTO dto)
        {
            Console.WriteLine("stagelogs POST entered.");

            // Getting random stage objects for the stage
            int stageTime = 30; // 30 seconds
            int stageObjectCount = GetStageobjectCount(dto.stageId);

            List<string> stageObjectList = new List<string>();

            // TODO : temporary stage objects list
            stageObjectList.Add("Tiger");
            stageObjectList.Add("Fish");
            stageObjectList.Add("Piano");

            // Add a stage log record
            StageLog newStageLog = new StageLog{
                    game_id = dto.gameId,
                    stage_id = dto.stageId,
                    completed_yn = "N",
                    start_date = DateTime.Now 
                };
            var value = _context.StageLog.Add(newStageLog);
            await _context.SaveChangesAsync();  

            // Add stage objects
            foreach (string stageObject in stageObjectList)
            {
                StageObject newStageObject = new StageObject{
                    game_id = dto.gameId,
                    stage_id = dto.stageId,
                    object_score = 10,
                    found_yn = "N",
                    log_date = DateTime.Now
                };
                _context.StageObject.Add(newStageObject);
                await _context.SaveChangesAsync();  
            }

            return Ok(new {newStageLog, stageObjectList});            
        }

        private int GetStageobjectCount(int stageId)
        {
            int stageObjectCount = 0;
            switch (stageId)
            {
                case 1:
                    stageObjectCount = 3;
                    break;
                case 2:
                    stageObjectCount = 5;
                    break;
                case 3:
                    stageObjectCount = 10;
                    break;
                case 4:
                    stageObjectCount = 15;
                    break;
                default:
                    stageObjectCount = 0;
                    break;
            }
            return stageObjectCount;
        }
        public static System.Drawing.Image GetCroppedFaceImage(System.Drawing.Image originalImage, BoundingBox box)
        {
            int left = Convert.ToInt32(originalImage.Width * box.Left);
            int top = Convert.ToInt32(originalImage.Height * box.Top);
            int width = Convert.ToInt32(originalImage.Width * box.Width);
            int height = Convert.ToInt32(originalImage.Height * box.Height);

            Rectangle rect = new Rectangle(left - (width*1/3), top - (height*2/5), width+(width*2/3), height+(height*2/3));
            Bitmap bmp = originalImage as Bitmap;
            Bitmap croppedImage = bmp.Clone(rect, bmp.PixelFormat);

            return croppedImage;
        }

        // PUT api/stagelogs/5
        [HttpPut("{game_id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/stagelogs/5
        [HttpDelete("{game_id}")]
        public void Delete(int id)
        {
        }       
    }
}