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
    public class TestPicturesController : ControllerBase
    {
        private DataContext _context;
        IAmazonS3 S3Client { get; set; }
        IAmazonRekognition RekognitionClient { get; set; }        

        public TestPicturesController(DataContext context, IAmazonS3 s3Client, IAmazonRekognition rekognitionClient)
        {
            _context = context;
            this.S3Client = s3Client;
            this.RekognitionClient = rekognitionClient;            
        }

        // GET api/testpictures
        [HttpGet]
        public async Task<IActionResult> GetTestPictures()
        {
            var values = await _context.TestPicture.ToListAsync();
            return Ok(values);
        }

        // GET api/testpictures
        [HttpGet("{picture_id}")]
        public async Task<IActionResult> UpdateTestPicture(int picture_id, [FromBody] string useYN)
        {
            TestPicture testPicture = await _context.TestPicture.Where(x => x.picture_id == picture_id).FirstOrDefaultAsync();
            testPicture.use_yn = useYN;

            var value = _context.TestPicture.Add(testPicture);
            await _context.SaveChangesAsync();
            
            return Ok(value);
        }

        // GET api/testpictures/5
        [HttpGet("{picture_id}")]
        public async Task<IActionResult> GetTestPicture(int picture_id)
        {
            var values = await _context.TestPicture.Where(x => x.picture_id == picture_id).ToListAsync();
            return Ok(values);
        }

        // POST api/testpicture
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TestPicturePostImageDTO dto)
        {
            Console.WriteLine("PostImage entered.");

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

            TestPicture newTestPicture = null;

            using (MemoryStream ms = new MemoryStream(imageByteArray))
            {
                // call Rekonition API
                labels = await RekognitionUtil.GetObjectDetailFromStream(this.RekognitionClient, ms);   

                // Database update
                newTestPicture = new TestPicture{
                    use_yn = "Y",
                    file_loc = keyName
                };
                
                _context.TestPicture.Add(newTestPicture);
                await _context.SaveChangesAsync(); 

                foreach (Label item in labels)
                {
                    TestPictureLabel newLabel = new TestPictureLabel{
                        picture_id = newTestPicture.picture_id,
                        label_name = item.Name,
                        confidence = item.Confidence
                    };
                    _context.TestPictureLabel.Add(newLabel);
                    await _context.SaveChangesAsync();  
                }
                
                // Upload image to S3 bucket
                await Task.Run(() => S3Util.UploadToS3(this.S3Client, bucketName, keyName, ms));
            }
            
            return Ok(new {newTestPicture, labels});            
        } 
    }
}