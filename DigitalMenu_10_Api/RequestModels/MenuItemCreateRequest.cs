namespace DigitalMenu_10_Api.RequestModels
{
    public class MenuItemCreateRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public List<IngredientRequestModel> Ingredients { get; set; }
/*        public IFormFile Image { get; set; }*/
    }
}
