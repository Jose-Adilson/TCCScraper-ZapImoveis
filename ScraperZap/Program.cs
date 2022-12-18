using System;
using ScraperZap.Scripts;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        ZapImoveis zap = new();
        VivaReal viva = new();
        KondorImoveis kondor = new();
        Habitec habitec = new();
        zap.ScriptZap(2, 2);
        viva.ScriptViva(1, 1);
        kondor.ScriptKondor(1);
        habitec.ScriptHabitec(1, 1);
    }

}
    





