using System;
using System.ComponentModel.DataAnnotations;

namespace CowRegister.Model
{
    internal class Cattle
    {
        [Key]
        public long Enar { get; set; }
        public int? EarLetter { get; set; }
        public long MotherEnar { get; set; }
        public DateTime Birth { get; set; }
        public Gender Gender { get; set;}
        public Color Breed { get; set; }
        public DateTime ArrivedStock { get; set; }
        public DateTime? AbandonedStock { get; set; }
        public string? NoteOfAbandon { get; set; }
    }

    public enum Gender
    {
        Bika = 0,
        Üsző = 1
    }

    public enum Color
    {
        Fehér = 0,
        Szürke = 1,
        Vörös = 2,
        Vöröstarka = 3,
        Zsemleszínű = 4,
        Zsemletarka = 5
    }
}
