namespace GotTalent_API.DTOs
{
    public class GameStagePostImageDTO
    {
        public int gameId { get; set; }
        public int stageId { get; set; }
        public string base64Image { get; set; }
    }
}