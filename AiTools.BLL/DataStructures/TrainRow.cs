using CsvHelper.Configuration.Attributes;
using Microsoft.ML.Data;

namespace AiTools.BLL.DataStructures
{
    class TrainRow
    {
        [Name("store")]
        [LoadColumn(0)]
        public string Store { get; set; }
        [Name("item")]
        [LoadColumn(1)]
        public string Item { get; set; }
        [Name("week")]
        [LoadColumn(2)]
        public float Week { get; set; }
        [Name("year")]
        [LoadColumn(3)]
        public float Year { get; set; }
        [Name("motnh")]
        [LoadColumn(4)]
        public float Month { get; set; }
        [Name("sales")]
        [LoadColumn(5)]
        public float Sales { get; set; }
    }
}
