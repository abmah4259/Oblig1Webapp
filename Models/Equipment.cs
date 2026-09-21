using System;

namespace StudyRoom.Models
{
    [Flags]
    public enum Equipment
    {
        None = 0,
        Data = 1,
        Tavle = 2,
        Krittavle = 4,
        Mikrofon = 8,
        Storskjerm = 16,
        Projektor = 32
    }
}