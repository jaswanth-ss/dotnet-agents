using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace outfitPlanner
{
    public class OutfitPlugin
{
    [KernelFunction("GetAvailableClothes")]
    [Description("Gets the list of clean clothes available in the user's closet.")]
    public string GetAvailableClothes()
    {
        Console.WriteLine("[Tool] GetAvailableClothes()");
        return """
        - Tops: White cotton t-shirt, Navy merino sweater, Charcoal hoodie
        - Bottoms: Dark blue jeans, Khaki chinos, Black joggers
        - Outerwear: Waterproof trench coat, Denim jacket, Heavy wool parka
        - Footwear: White leather sneakers, Waterproof Chelsea boots
        """;
    }
}
}